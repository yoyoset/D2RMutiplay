---
name: RUST_TAURI_WIN32
description: Best practices for building high-performance, secure Windows apps with Rust, Tauri, and React.
---

# Rust + Tauri + Win32 Development Guidelines

## 1. Architectural Patterns
### Backend (Rust)
- **Modularity**: Group logic by domain in `src-tauri/src/modules/`.
  - `modules/process/`: Win32 process creation, token manipulation.
  - `modules/mutex/`: Handle hunting and closing logic.
  - `modules/user/`: User profile loading and validation.
- **Error Handling**: Use `thiserror` for library-level errors and `anyhow` for application-level errors. ALWAYS propagate errors to the specific Tauri command wrapper.
- **Async**: Win32 APIs are mostly synchronous. Wrap long-running blocking calls (like handle enumeration) in `tauri::async_runtime::spawn_blocking` to avoid freezing the UI.
- **Safety**:
  - Encapsulate all `unsafe` Win32 calls in safe wrapper functions.
  - Add comments explaining *why* a block is unsafe and how invariants are upheld.

### Frontend (React + Tailwind)
- **Glassmorphism**: Use `backdrop-filter: blur()` and semi-transparent backgrounds (`bg-black/80`) heavily.
- **State**: Use `zustand` for global app state (accounts list, active processes).
- **Communication**: Create a strictly typed `api.ts` layer that wraps all `invoke()` calls. Never call `invoke` directly in components.

## 2. Win32 Interop Standards
- **Crate**: Use `windows` (official Microsoft crate) over `winapi` (legacy).
- **Strings**:
  - Windows APIs use wide strings (`PCWSTR`). Use a helper trait or function to convert specific Rust `&str` to `Vec<u16>` (null-terminated).
- **Resources**: Always implement `Drop` for structs wrapping Handles (HANDLE, HMODULE) to ensure `CloseHandle` is called automatically.

## 3. UI/UX "Imperial Gold" Philosophy
- **Colors**:
  - Background: Deep Void (`#050505` to `#121212`)
  - Accent: Imperial Gold (`#D4AF37`)
  - Text: Off-white (`#E5E5E5`) for readability, Pure White (`#FFFFFF`) for headers.
- **Motion**:
  - All hover states must have `transition-all duration-200`.
  - Buttons should have active scale effects (`active:scale-95`).

## 4. Code Structure Example
```rust
// src-tauri/src/modules/mutex.rs
use windows::Win32::Foundation::HANDLE;

pub struct HandleGuard(HANDLE);

impl Drop for HandleGuard {
    fn drop(&mut self) {
        unsafe { windows::Win32::Foundation::CloseHandle(self.0); }
    }
}
```
