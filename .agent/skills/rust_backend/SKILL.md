---
name: RUST_BACKEND_WIN32
description: Standards for Rust backend development with a focus on Win32 interop, safety, and performance.
---

# Rust Backend Standards: The "Iron Core"

## 1. Safety First (Win32 Interop)
When interacting with Windows APIs (`windows-rs` crate), strict encapsulation is required.

-   **Isolation**: NEVER call `unsafe` code directly in business logic.
-   **Wrapper Pattern**: Create a safe wrapper for every raw Win32 handle or pointer.
    ```rust
    // GOOD: RAII Pattern
    pub struct HandleGuard(HANDLE);
    impl Drop for HandleGuard { ... }
    
    // BAD: Passing raw HANDLE around
    fn do_work(h: HANDLE) { ... }
    ```
-   **Error Propagation**: Map all `windows::core::Error` to a custom `AppError` enum using `thiserror`.

## 2. Module Structure
Organize `src-tauri/src` by domain, not technical layer.

-   `modules/win32_safe/`: The ONLY place where `unsafe` block is allowed.
    -   `process.rs`: `CreateProcess` wrappers.
    -   `security.rs`: Token/Permissions wrappers.
    -   `mutex.rs`: Handle enumeration and duplication.
-   `modules/game_logic/`: Pure Rust logic using the safe wrappers.
-   `commands/`: Tauri command handlers (The Bridge).

## 3. Logging & diagnostics
-   Use `tracing` and `tracing-subscriber` for structured logging.
-   Log all Win32 failures with error codes (HRESULT).
-   **Context**: Always include `pid` and `handle_value` in logs for debugging.

## 4. Performance
-   **Async I/O**: Use `tokio` (via Tauri) for file I/O.
-   **Blocking Tasks**: Offload Win32 enumeration (e.g., getting all system handles) to `tauri::async_runtime::spawn_blocking`.
