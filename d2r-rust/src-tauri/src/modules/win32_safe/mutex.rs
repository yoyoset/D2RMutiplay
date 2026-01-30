use std::ffi::c_void;
use std::mem::size_of;

use sysinfo::{ProcessRefreshKind, ProcessesToUpdate, System, UpdateKind};
use windows::core::PCWSTR;
use windows::Win32::Foundation::{
    CloseHandle, DuplicateHandle, DUPLICATE_CLOSE_SOURCE, DUPLICATE_HANDLE_OPTIONS, HANDLE, LUID,
    NTSTATUS, STATUS_INFO_LENGTH_MISMATCH, STATUS_SUCCESS,
};
use windows::Win32::Security::{
    AdjustTokenPrivileges, LookupPrivilegeValueW, LUID_AND_ATTRIBUTES, SE_PRIVILEGE_ENABLED,
    TOKEN_ADJUST_PRIVILEGES, TOKEN_PRIVILEGES, TOKEN_QUERY,
};
use windows::Win32::System::Threading::{
    CreateMutexW, GetCurrentProcess, OpenProcess, OpenProcessToken, PROCESS_DUP_HANDLE,
};

#[link(name = "ntdll")]
extern "system" {
    fn NtQuerySystemInformation(
        SystemInformationClass: i32,
        SystemInformation: *mut c_void,
        SystemInformationLength: u32,
        ReturnLength: *mut u32,
    ) -> NTSTATUS;

    fn NtQueryObject(
        Handle: HANDLE,
        ObjectInformationClass: i32,
        ObjectInformation: *mut c_void,
        ObjectInformationLength: u32,
        ReturnLength: *mut u32,
    ) -> NTSTATUS;
}

const SYSTEM_EXTENDED_HANDLE_INFORMATION: i32 = 64;
const OBJECT_NAME_INFORMATION: i32 = 1;
const D2R_MUTEX_NAME: &str = "DiabloII Check For Other Instances";

#[repr(C)]
#[derive(Copy, Clone, Debug)]
struct SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX {
    object: *mut c_void,
    unique_process_id: usize,
    handle_value: usize,
    granted_access: u32,
    creator_back_trace_index: u16,
    object_type_index: u16,
    handle_attributes: u32,
    reserved: u32,
}

#[repr(C)]
struct SYSTEM_HANDLE_INFORMATION_EX {
    number_of_handles: usize,
    reserved: usize,
    handles: [SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX; 1],
}

// Windows-rs UNICODE_STRING is a bit different, define a compatible one for ptr casting
#[repr(C)]
struct UNICODE_STRING_COMPAT {
    length: u16,
    maximum_length: u16,
    buffer: *mut u16,
}

/// Enable SeDebugPrivilege for the current process
fn enable_debug_privilege() -> anyhow::Result<()> {
    unsafe {
        let mut token = HANDLE::default();
        if OpenProcessToken(
            GetCurrentProcess(),
            TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY,
            &mut token,
        )
        .is_err()
        {
            return Err(anyhow::anyhow!("OpenProcessToken failed"));
        }

        let mut luid = LUID::default();
        // "SeDebugPrivilege\0" encoded as UTF-16
        let priv_name: Vec<u16> = "SeDebugPrivilege"
            .encode_utf16()
            .chain(std::iter::once(0))
            .collect();

        if LookupPrivilegeValueW(
            PCWSTR::null(),
            PCWSTR::from_raw(priv_name.as_ptr()),
            &mut luid,
        )
        .is_err()
        {
            let _ = CloseHandle(token);
            return Err(anyhow::anyhow!("LookupPrivilegeValueW failed"));
        }

        let tp = TOKEN_PRIVILEGES {
            PrivilegeCount: 1,
            Privileges: [LUID_AND_ATTRIBUTES {
                Luid: luid,
                Attributes: SE_PRIVILEGE_ENABLED,
            }],
        };

        if AdjustTokenPrivileges(token, false, Some(&tp), 0, None, None).is_err() {
            let _ = CloseHandle(token);
            return Err(anyhow::anyhow!("AdjustTokenPrivileges failed"));
        }

        let _ = CloseHandle(token);
        Ok(())
    }
}

/// Scans all system handles and closes those matching the D2R mutex name.
pub fn close_d2r_mutexes() -> Result<usize, anyhow::Error> {
    // Enable SeDebugPrivilege to ensure we can OpenProcess target games
    if let Err(e) = enable_debug_privilege() {
        crate::modules::logger::warn(&format!("Failed to enable SeDebugPrivilege: {}", e));
    }

    unsafe {
        // 1. Get the "Mutant" type index by creating a dummy mutex and finding it in the handle list
        let my_pid = std::process::id() as usize;
        let temp_name: Vec<u16> = format!("D2R_TMP_{}\0", my_pid).encode_utf16().collect();
        let h_temp = CreateMutexW(None, false, PCWSTR(temp_name.as_ptr()))
            .map_err(|e| anyhow::anyhow!("CreateMutex failed: {}", e))?;
        let temp_handle_val = h_temp.0 as usize;

        // 2. Fetch all system handles (Class 64 - Extended)
        let mut size: u32 = 0x100000; // 1MB initial
        let mut buffer: Vec<u8> = vec![0; size as usize];
        let mut return_length: u32 = 0;

        loop {
            let status = NtQuerySystemInformation(
                SYSTEM_EXTENDED_HANDLE_INFORMATION,
                buffer.as_mut_ptr() as *mut c_void,
                size,
                &mut return_length,
            );

            // 0xC0000004 is STATUS_INFO_LENGTH_MISMATCH
            if status == STATUS_INFO_LENGTH_MISMATCH || status == NTSTATUS(0xC0000004u32 as i32) {
                size = if return_length == 0 {
                    size * 2
                } else {
                    return_length + 0x1000
                };
                buffer.resize(size as usize, 0);
            } else if status == STATUS_SUCCESS {
                break;
            } else {
                let _ = CloseHandle(h_temp);
                return Err(anyhow::anyhow!(
                    "NtQuerySystemInformation(64) failed: 0x{:X}",
                    status.0
                ));
            }
        }

        let info = &*(buffer.as_ptr() as *const SYSTEM_HANDLE_INFORMATION_EX);

        // Offset of 'handles' is 16 bytes on x64 (2 x usize)
        let handle_ptr =
            buffer.as_ptr().add(size_of::<usize>() * 2) as *const SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX;
        let handles = std::slice::from_raw_parts(handle_ptr, info.number_of_handles);

        // 3. Find our temp mutex's type index
        let mut mutant_type_index = 0u16;
        let mut found_index = false;
        for entry in handles {
            if entry.unique_process_id == my_pid && entry.handle_value == temp_handle_val {
                mutant_type_index = entry.object_type_index;
                found_index = true;
                break;
            }
        }
        let _ = CloseHandle(h_temp);

        // 0. Identify D2R PIDs for "Brute Force" targeting
        let mut d2r_pids = std::collections::HashSet::new();
        {
            let mut sys = System::new_all();
            sys.refresh_processes_specifics(
                ProcessesToUpdate::All,
                true,
                ProcessRefreshKind::nothing().with_exe(UpdateKind::OnlyIfNotSet),
            );
            for (pid, process) in sys.processes() {
                if let Some(exe_path) = process.exe() {
                    if let Some(exe_name) = exe_path.file_name() {
                        let name = exe_name.to_string_lossy().to_lowercase();
                        if name == "d2r.exe" || name == "diabloii.exe" {
                            d2r_pids.insert(pid.as_u32() as usize);
                        }
                    }
                }
            }
        }
        crate::modules::logger::info(&format!("Target D2R PIDs identified: {:?}", d2r_pids));

        if !found_index {
            crate::modules::logger::error(&format!("FATAL: Failed to find our own temp mutex handle in the system list. Handles scanned: {}", info.number_of_handles));
            return Err(anyhow::anyhow!("Could not determine Mutant type index"));
        }

        crate::modules::logger::info(&format!(
            "System-wide Scan: {} handles. Mutant TypeIndex identified as: {}",
            info.number_of_handles, mutant_type_index
        ));

        // 4. Filter and Close - TARGETED + BRUTE FORCE (Silent)
        // User Request: "Find PID -> Find Handle -> Kill".
        // Strategy: We MUST brute-force scan ALL handles in D2R because the Mutex might not match our detected 'Mutant' type index
        // (as seen in previous failure where generic filter missed the handle).
        // However, we will SILENCE the "Not Supported" errors for non-duplicatable handles.

        let mut closed_count = 0;
        let mut scanned_count = 0;
        let mut name_query_count = 0;

        for entry in handles {
            // Strict Filter: Only look at handles belonging to D2R processes
            if !d2r_pids.contains(&entry.unique_process_id) {
                continue;
            }

            // NOTE: Do NOT filter by object_type_index for D2R.
            // Previous attempt to filter by 'Mutant' reduced scan count from ~4000 to 26 and MISSED the target.
            // We must scan everything, but we will suppress errors in get_handle_name.

            scanned_count += 1;
            name_query_count += 1;

            if let Some(name) =
                get_handle_name(entry.unique_process_id as u32, entry.handle_value as u32)
            {
                let name_lower = name.to_lowercase();

                if name_lower.contains(&D2R_MUTEX_NAME.to_lowercase()) {
                    crate::modules::logger::info(&format!(
                        "TARGET FOUND: D2R Mutex identified in PID {} (Handle 0x{:X}). Closing...",
                        entry.unique_process_id, entry.handle_value
                    ));
                    if close_remote_handle(
                        entry.unique_process_id as u32,
                        entry.handle_value as u32,
                    ) {
                        closed_count += 1;
                        crate::modules::logger::info(&format!(
                            "SUCCESS: Handle closed for PID {}",
                            entry.unique_process_id
                        ));
                    } else {
                        crate::modules::logger::error(&format!(
                            "FAILURE: Failed to close handle in PID {}",
                            entry.unique_process_id
                        ));
                    }
                }
            }
        }

        crate::modules::logger::info(&format!(
            "Scan Summary: Total Handles: {}, Scanned: {}, NameQueries: {}, Closed: {}",
            info.number_of_handles, scanned_count, name_query_count, closed_count
        ));

        Ok(closed_count)
    }
}

unsafe fn get_handle_name(pid: u32, handle_val: u32) -> Option<String> {
    // Suppress OpenProcess errors for cleaner logs
    let h_process = match OpenProcess(PROCESS_DUP_HANDLE, false, pid) {
        Ok(h) => h,
        Err(_) => return None,
    };

    if h_process.is_invalid() {
        return None;
    }

    let mut h_dup: HANDLE = HANDLE::default();
    let current_process = windows::Win32::System::Threading::GetCurrentProcess();

    let res = DuplicateHandle(
        h_process,
        HANDLE(handle_val as *mut c_void),
        current_process,
        &mut h_dup,
        0,
        false,
        DUPLICATE_HANDLE_OPTIONS(0),
    );

    let _ = CloseHandle(h_process);

    if let Err(e) = res {
        // Suppress common expected errors for non-duplicatable handles to avoid log spam
        // 0x80070032 = ERROR_NOT_SUPPORTED
        // 0x80070005 = ACCESS_DENIED
        // 0x80070006 = INVALID_HANDLE
        let err_code = e.code().0;
        if err_code == 0x80070032u32 as i32
            || err_code == 0x80070005u32 as i32
            || err_code == 0x80070006u32 as i32
        {
            return None;
        }

        // Only log unusual errors
        // crate::modules::logger::warn(&format!("DuplicateHandle failed for PID {}: {}", pid, e));
        return None;
    }

    // Increased buffer size to 0x2000 (8KB) to prevent STATUS_INFO_LENGTH_MISMATCH on long object paths
    // Legacy C# used 0x2000.
    let mut buffer = vec![0u8; 0x2000];
    let mut return_len = 0;
    let status = NtQueryObject(
        h_dup,
        OBJECT_NAME_INFORMATION,
        buffer.as_mut_ptr() as *mut c_void,
        buffer.len() as u32,
        &mut return_len,
    );

    let _ = CloseHandle(h_dup);

    if status == STATUS_SUCCESS {
        let info = &*(buffer.as_ptr() as *const UNICODE_STRING_COMPAT);
        if info.length > 0 && !info.buffer.is_null() {
            let slice = std::slice::from_raw_parts(info.buffer, (info.length / 2) as usize);
            return Some(String::from_utf16_lossy(slice));
        }
    }

    None
}

unsafe fn close_remote_handle(pid: u32, handle_val: u32) -> bool {
    if let Ok(h_process) = OpenProcess(PROCESS_DUP_HANDLE, false, pid) {
        let mut h_dup_dummy: HANDLE = HANDLE::default();
        let res = DuplicateHandle(
            h_process,
            HANDLE(handle_val as *mut c_void),
            windows::Win32::System::Threading::GetCurrentProcess(),
            &mut h_dup_dummy,
            0,
            false,
            DUPLICATE_CLOSE_SOURCE,
        );

        let _ = CloseHandle(h_process);
        if res.is_ok() {
            let _ = CloseHandle(h_dup_dummy);
            return true;
        }
    }
    false
}
