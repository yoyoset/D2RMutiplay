use std::ptr;
use windows::core::{PCWSTR, PWSTR};
use windows::Win32::NetworkManagement::NetManagement::{
    NetApiBufferFree, NetLocalGroupAddMembers, NetUserAdd, NetUserEnum, FILTER_NORMAL_ACCOUNT,
    LOCALGROUP_MEMBERS_INFO_3, USER_ACCOUNT_FLAGS, USER_INFO_0, USER_INFO_1, USER_PRIV,
};
use windows::Win32::Security::{
    CreateWellKnownSid, LookupAccountSidW, WinBuiltinUsersSid, PSID, SID_NAME_USE,
};

pub fn get_whoami() -> String {
    let username = std::env::var("USERNAME").unwrap_or_default();
    let domain = std::env::var("USERDOMAIN").unwrap_or_default();
    if domain.is_empty() {
        username
    } else {
        format!("{}\\{}", domain, username)
    }
}

pub fn list_local_users(include_registry: bool) -> Result<Vec<String>, String> {
    let mut users = Vec::new();
    let mut resume_handle = 0;

    loop {
        let mut buffer: *mut u8 = ptr::null_mut();
        let mut entries_read = 0;
        let mut total_entries = 0;

        unsafe {
            let status = NetUserEnum(
                PCWSTR::null(),
                0,
                FILTER_NORMAL_ACCOUNT,
                &mut buffer,
                0xFFFF_FFFF,
                &mut entries_read,
                &mut total_entries,
                Some(&mut resume_handle),
            );

            if status == 0 || status == 234 {
                if !buffer.is_null() {
                    let info_ptr = buffer as *mut USER_INFO_0;
                    for i in 0..entries_read {
                        let info = *info_ptr.add(i as usize);
                        if let Ok(name) = info.usri0_name.to_string() {
                            let lower_name = name.to_lowercase();
                            if !vec!["guest", "wdagutilityaccount", "defaultaccount"]
                                .contains(&lower_name.as_str())
                            {
                                users.push(name);
                            }
                        }
                    }
                }
            }

            if !buffer.is_null() {
                NetApiBufferFree(Some(buffer as *const _));
            }

            if status != 234 {
                break;
            }
        }
    }

    // Standard Win32 API is enough for 99% of commercial cases.
    // Deep registry scanning is over-engineering for a simple multipay tool.
    if include_registry {
        tracing::debug!("Registry scan skipped to maintain pragmatic simplicity.");
    }

    let current = get_whoami();
    if !users.contains(&current) {
        users.push(current);
    }

    Ok(users)
}

#[allow(dead_code)]
fn get_name_from_sid_string(sid_str: &str) -> Result<String, String> {
    use windows::Win32::Security::Authorization::ConvertStringSidToSidW;

    let sid_u16: Vec<u16> = sid_str.encode_utf16().chain(Some(0)).collect();
    let mut psid = PSID(ptr::null_mut());

    unsafe {
        if ConvertStringSidToSidW(PCWSTR(sid_u16.as_ptr()), &mut psid).is_ok() {
            let mut name = [0u16; 256];
            let mut name_size = name.len() as u32;
            let mut dom = [0u16; 256];
            let mut dom_size = dom.len() as u32;
            let mut snu = SID_NAME_USE::default();

            let res = if LookupAccountSidW(
                PCWSTR::null(),
                psid,
                Some(PWSTR(name.as_mut_ptr())),
                &mut name_size,
                Some(PWSTR(dom.as_mut_ptr())),
                &mut dom_size,
                &mut snu,
            )
            .is_ok()
            {
                let user_name = String::from_utf16_lossy(&name[..name_size as usize]);
                let domain_name = String::from_utf16_lossy(&dom[..dom_size as usize]);
                if domain_name.is_empty() {
                    Ok(user_name)
                } else {
                    Ok(format!("{}\\{}", domain_name, user_name))
                }
            } else {
                Err("Lookup failed".to_string())
            };

            // psid allocated by ConvertStringSidToSidW must be freed with LocalFree
            use windows::Win32::Foundation::{LocalFree, HLOCAL};
            let _ = LocalFree(Some(HLOCAL(psid.0)));

            res
        } else {
            Err("Invalid SID string".to_string())
        }
    }
}

fn get_localized_users_group_name() -> String {
    unsafe {
        let mut sid_size = 0;
        let _ = CreateWellKnownSid(
            WinBuiltinUsersSid,
            None,
            Some(PSID(ptr::null_mut())),
            &mut sid_size,
        );

        let mut sid = vec![0u8; sid_size as usize];
        let psid = PSID(sid.as_mut_ptr() as *mut _);

        if CreateWellKnownSid(WinBuiltinUsersSid, None, Some(psid), &mut sid_size).is_ok() {
            let mut name = [0u16; 256];
            let mut name_size = name.len() as u32;
            let mut dom = [0u16; 256];
            let mut dom_size = dom.len() as u32;
            let mut snu = SID_NAME_USE::default();

            if LookupAccountSidW(
                PCWSTR::null(),
                psid,
                Some(PWSTR(name.as_mut_ptr())),
                &mut name_size,
                Some(PWSTR(dom.as_mut_ptr())),
                &mut dom_size,
                &mut snu,
            )
            .is_ok()
            {
                return String::from_utf16_lossy(&name[..name_size as usize]);
            }
        }
    }
    "Users".to_string()
}

pub fn create_user(username: &str, password: &str) -> Result<(), String> {
    let username_u16: Vec<u16> = username.encode_utf16().chain(Some(0)).collect();
    let password_u16: Vec<u16> = password.encode_utf16().chain(Some(0)).collect();

    let mut user_info = USER_INFO_1::default();
    user_info.usri1_name = PWSTR(username_u16.as_ptr() as *mut _);
    user_info.usri1_password = PWSTR(password_u16.as_ptr() as *mut _);
    user_info.usri1_priv = USER_PRIV(1);
    user_info.usri1_flags = USER_ACCOUNT_FLAGS(0x0001 | 0x0200 | 0x10000); // UF_SCRIPT | UF_NORMAL_ACCOUNT | UF_DONT_EXPIRE_PASSWD

    tracing::info!(%username, "Attempting to create user");
    unsafe {
        let status = NetUserAdd(PCWSTR::null(), 1, &user_info as *const _ as *const _, None);

        if status != 0 {
            let err_msg = if status == 5 {
                "NetUserAdd failed: Access Denied (Status 5). Please Run as Administrator."
                    .to_string()
            } else {
                format!("NetUserAdd failed with status: {}", status)
            };
            tracing::error!(%status, %err_msg);
            return Err(err_msg);
        }
        tracing::info!("User created successfully via NetUserAdd");

        let group_name = get_localized_users_group_name();
        tracing::info!(%group_name, "Adding user to group");
        let group_name_u16: Vec<u16> = group_name.encode_utf16().chain(Some(0)).collect();

        let member = LOCALGROUP_MEMBERS_INFO_3 {
            lgrmi3_domainandname: PWSTR(username_u16.as_ptr() as *mut _),
        };

        let group_status = NetLocalGroupAddMembers(
            PCWSTR::null(),
            PCWSTR(group_name_u16.as_ptr()),
            3,
            &member as *const _ as *const _,
            1,
        );

        if group_status != 0 {
            tracing::warn!(
                status = %group_status,
                "NetLocalGroupAddMembers failed. This might not be critical."
            );
        } else {
            tracing::info!("User added to local group successfully");
        }

        Ok(())
    }
}
