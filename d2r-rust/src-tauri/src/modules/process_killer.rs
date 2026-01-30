use sysinfo::{ProcessRefreshKind, ProcessesToUpdate, System};

const TARGET_PROCESSES: &[&str] = &[
    "Battle.net.exe",
    "Agent.exe",
    "crashpad_handler.exe",
    "Battle.net Launcher.exe",
    "Blizzard Browser.exe",
    "BlizzardError.exe",
    "Battle.net Helper.exe",
];

/// Kills all Battle.net related processes and returns the count of killed processes.
pub fn kill_battle_net_processes() -> usize {
    let mut system = System::new_all();
    system.refresh_processes_specifics(ProcessesToUpdate::All, true, ProcessRefreshKind::nothing());

    let mut killed_count = 0;

    for process in system.processes().values() {
        let p_name = process.name().to_string_lossy();
        if TARGET_PROCESSES
            .iter()
            .any(|&t| p_name.eq_ignore_ascii_case(t))
        {
            if process.kill() {
                killed_count += 1;
                tracing::info!(process = %p_name, pid = %process.pid(), "Killed target process");
            }
        }
    }

    killed_count
}

/// Dynamic wait until all target processes are completely gone.
/// Returns true if everything is clean, false if timed out.
pub fn wait_for_cleanup(system: &mut System, max_attempts: u32, interval_ms: u64) -> bool {
    for attempt in 1..=max_attempts {
        system.refresh_processes_specifics(
            ProcessesToUpdate::All,
            true,
            ProcessRefreshKind::nothing(),
        );

        let still_running = system.processes().values().any(|p| {
            let p_name = p.name().to_string_lossy();
            TARGET_PROCESSES
                .iter()
                .any(|&t| p_name.eq_ignore_ascii_case(t))
        });

        if !still_running {
            tracing::info!(
                attempts = attempt,
                "Environment cleanup verified: All target processes exited."
            );
            return true;
        }

        if attempt < max_attempts {
            std::thread::sleep(std::time::Duration::from_millis(interval_ms));
        }
    }

    tracing::warn!("Environment cleanup timed out: Some target processes may still be active.");
    false
}
