use once_cell::sync::Lazy;
use std::fs::{self, OpenOptions};
use std::io::Write;
use std::path::PathBuf;
use std::sync::Mutex;
use tauri::Manager;

static LOG_FILE: Lazy<Mutex<Option<PathBuf>>> = Lazy::new(|| Mutex::new(None));

pub fn init(app_handle: &tauri::AppHandle) {
    // User requested log file in the same directory as the executable
    let log_path = std::env::current_exe()
        .map(|mut p| {
            p.pop(); // Remove exe name to get directory
            p.push("debug.log");
            p
        })
        .unwrap_or_else(|_| {
            // Fallback to app cache dir if current_exe fails
            app_handle
                .path()
                .app_cache_dir()
                .unwrap_or_else(|_| PathBuf::from("."))
                .join("debug.log")
        });

    // Ensure directory exists (useful if fallback usage or weird path)
    if let Some(parent) = log_path.parent() {
        if !parent.exists() {
            let _ = fs::create_dir_all(parent);
        }
    }

    println!("[LOGGER] Log file path: {:?}", log_path);

    let mut log = LOG_FILE.lock().unwrap();
    *log = Some(log_path);
}

pub fn log(level: &str, message: &str) {
    let log_path_lock = LOG_FILE.lock().unwrap();
    if let Some(ref path) = *log_path_lock {
        let timestamp = chrono::Local::now().format("%Y-%m-%d %H:%M:%S");
        let log_line = format!("[{}] [{}] {}\n", timestamp, level, message);

        if let Ok(mut file) = OpenOptions::new().create(true).append(true).open(path) {
            let _ = file.write_all(log_line.as_bytes());
        }
    }
}

pub fn info(message: &str) {
    log("INFO", message);
}
pub fn error(message: &str) {
    log("ERROR", message);
}
pub fn warn(message: &str) {
    log("WARN", message);
}
