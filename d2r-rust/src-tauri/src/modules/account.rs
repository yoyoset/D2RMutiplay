use crate::modules::file_swap;
use crate::modules::process_killer;
use crate::modules::win32_safe::{mutex, process};
use std::collections::HashMap;
use std::path::PathBuf;
use std::process::Command;
use tauri::{AppHandle, Manager};

#[derive(serde::Deserialize, serde::Serialize, Debug, Clone)]
pub struct Account {
    pub id: String,               // UUID
    pub win_user: String,         // The bound Windows Username
    pub win_pass: Option<String>, // Optional (for auto-launch)
    pub bnet_account: String,     // Display only
    pub note: Option<String>,     // Role remarks
    pub avatar: Option<String>,   // Base64 encoded image or library icon ID
}

#[derive(serde::Serialize, Debug, Default, Clone)]
pub struct AccountStatus {
    pub bnet_active: bool,
    pub d2r_active: bool,
    pub exists: bool,
}

#[derive(thiserror::Error, Debug)]
pub enum AccountError {
    #[error("Launch failed: {0}")]
    LaunchError(#[from] anyhow::Error),
    #[error("Game path not found")]
    InvalidPath,
    #[error("File Swap Error: {0}")]
    FileSwap(#[from] file_swap::FileSwapError),
}

pub fn launch_game(
    app: &AppHandle,
    account: &Account,
    _game_path: &str,
) -> Result<u32, AccountError> {
    // 1. Cleanup Environment (Kill Bnet and Mutexes)
    process_killer::kill_battle_net_processes();
    let _ = mutex::close_d2r_mutexes();

    // Dynamically wait for processes to exit (Max 3s)
    let state = app.state::<crate::state::AppState>();
    let mut system = state.sys.lock().unwrap();
    if !process_killer::wait_for_cleanup(&mut system, 10, 300) {
        tracing::warn!("Proceeding with environment swap despite potential file locks.");
    }

    // 2. Load Config to check Last Active Account
    let mut config = crate::modules::config::AppConfig::load(app)
        .map_err(|e| anyhow::anyhow!("Config load failed: {}", e))?;

    // 3. Backup (Save Previous): Save CURRENT product.db to Previous Account snapshot
    let mut rotated = false;
    if let Some(last_id) = &config.last_active_account {
        if last_id != &account.id {
            if let Err(e) = file_swap::rotate_save(app, last_id) {
                // If backup fails, we might still want to proceed if the file is missing/corrupt,
                // but let's be safe and log it.
                tracing::warn!("Rotation backup failed for {}: {}", last_id, e);
            } else {
                rotated = true;
            }
        }
    }

    // 4. Delete (Clean Slate): Delete current product.db
    file_swap::delete_config()?;

    // 5. Restore (Target): Load target account snapshot
    if let Err(e) = file_swap::restore_snapshot(app, &account.id) {
        // Rollback: If restore fails and we rotated the previous one, try to put it back
        if rotated {
            if let Some(last_id) = &config.last_active_account {
                let _ = file_swap::restore_snapshot(app, last_id);
            }
        }
        return Err(e.into());
    }

    // 6. Update Last Active Account
    config.last_active_account = Some(account.id.clone());
    let _ = config.save(app);

    // 7. Launch Battle.net
    let bnet_path_buf = get_bnet_path().ok_or(AccountError::InvalidPath)?;
    let bnet_path = bnet_path_buf
        .to_str()
        .ok_or_else(|| anyhow::anyhow!("Non-UTF8 path detected"))?;

    let working_dir = bnet_path_buf
        .parent()
        .map(|p| p.to_string_lossy().to_string());

    // Logic: Identify Current User vs Sandbox User
    // Normalize comparison: extract user part if domain\user is provided
    let get_user_only = |u: &str| u.split('\\').last().unwrap_or(u).to_lowercase();

    let current_user_raw = crate::modules::win_user::get_whoami();
    let current_user_normalized = get_user_only(&current_user_raw);
    let target_user_normalized = get_user_only(&account.win_user);

    tracing::info!(
        current = %current_user_normalized,
        target = %target_user_normalized,
        "Identity verification for launch"
    );

    if target_user_normalized == current_user_normalized {
        // Direct launch for current user (No password needed)
        let mut cmd = std::process::Command::new(bnet_path);
        #[cfg(windows)]
        {
            use std::os::windows::process::CommandExt;
            cmd.creation_flags(0x08000000); // CREATE_NO_WINDOW
        }
        if let Some(wd) = working_dir {
            cmd.current_dir(wd);
        }
        let child = cmd
            .spawn()
            .map_err(|e| anyhow::anyhow!("Failed to spawn process: {}", e))?;
        Ok(child.id())
    } else {
        // Sandbox launch with credentials
        let (domain, user) = if let Some(pos) = account.win_user.find('\\') {
            (Some(&account.win_user[..pos]), &account.win_user[pos + 1..])
        } else {
            (None, account.win_user.as_str())
        };

        let result = process::create_process_with_logon(
            user,
            domain,
            account.win_pass.as_deref().unwrap_or(""),
            bnet_path,
            None,
            working_dir.as_deref(),
        )?;
        Ok(result.process_id)
    }
}

fn get_bnet_path() -> Option<PathBuf> {
    // 1. Try standard hardcoded path
    let standard = PathBuf::from(r"C:\Program Files (x86)\Battle.net\Battle.net.exe");
    if standard.exists() {
        return Some(standard);
    }

    // 2. Try Registry Lookup (Auto-detect)
    // HKLM\SOFTWARE\WOW6432Node\Blizzard Entertainment\Battle.net\Capabilities -> ApplicationIcon
    // Value format: "C:\Path\To\Battle.net.exe",0
    let mut cmd = Command::new("reg");
    #[cfg(windows)]
    {
        use std::os::windows::process::CommandExt;
        cmd.creation_flags(0x08000000); // CREATE_NO_WINDOW
    }
    if let Ok(output) = cmd
        .args([
            "query",
            r"HKLM\SOFTWARE\WOW6432Node\Blizzard Entertainment\Battle.net\Capabilities",
            "/v",
            "ApplicationIcon",
        ])
        .output()
    {
        let stdout = String::from_utf8_lossy(&output.stdout);
        // Robust regex-free parsing: find path between quotes or after REG_SZ
        if let Some(line) = stdout.lines().find(|l| l.contains("ApplicationIcon")) {
            let path_part = line.split("REG_SZ").last().unwrap_or("").trim();
            // Remove quotes and trailing comma sequence if present
            let clean_path = path_part
                .replace('"', "")
                .split(',')
                .next()
                .unwrap_or("")
                .trim()
                .to_string();

            if !clean_path.is_empty() {
                let p = PathBuf::from(clean_path);
                if p.exists() {
                    return Some(p);
                }
            }
        }
    }

    None
}

#[tauri::command]
pub fn get_accounts_process_status(
    state: tauri::State<'_, crate::state::AppState>,
    usernames: Vec<String>,
) -> HashMap<String, AccountStatus> {
    let sys = state.sys.lock().unwrap();
    let users = state.users.lock().unwrap();

    // Simplified: Just use the cached/passed usernames.
    // We don't need to scan registry every 10 seconds for user existence.
    // That's what caused the flash.

    let mut status_map = HashMap::new();
    for u in &usernames {
        let _normalized_requested = if let Some(pos) = u.find('\\') {
            &u[pos + 1..]
        } else {
            u.as_str()
        }
        .to_lowercase();

        status_map.insert(
            u.clone(),
            AccountStatus {
                bnet_active: false,
                d2r_active: false,
                exists: true, // Assume exists, or handle elsewhere
            },
        );
    }

    for process in sys.processes().values() {
        let name = process.name().to_string_lossy();
        let is_bnet = name.eq_ignore_ascii_case("Battle.net.exe");
        let is_d2r = name.eq_ignore_ascii_case("D2R.exe");

        if is_bnet || is_d2r {
            if let Some(user_id) = process.user_id() {
                if let Some(user) = users.get_user_by_id(user_id) {
                    let proc_user = user.name().to_lowercase();

                    for requested_user in &usernames {
                        let normalized_requested = if let Some(pos) = requested_user.find('\\') {
                            &requested_user[pos + 1..]
                        } else {
                            requested_user.as_str()
                        }
                        .to_lowercase();

                        if proc_user == normalized_requested {
                            let entry = status_map.get_mut(requested_user).unwrap();
                            if is_bnet {
                                entry.bnet_active = true;
                            }
                            if is_d2r {
                                entry.d2r_active = true;
                            }
                        }
                    }
                }
            }
        }
    }

    status_map
}
