---
name: PROJECT_ARCHITECTURE_RUST
description: High-level architectural guidelines for the D2RMultiplay Rust Project, focused on separation of concerns and build pipeline.
---

# Project Architecture: D2R Rust Edition

## 1. Directory Structure (Side-by-Side)
We maintain a clean separation between the legacy C# code and the new Rust system during the transition.

```text
/ (Root)
├── .agent/skills/         # YOU ARE HERE
├── src/                   # Legacy C# Source (Reference Only)
├── d2r-rust/              # NEW ROOT
│   ├── src-tauri/         # Backend (Rust)
│   │   ├── src/
│   │   │   ├── modules/   # Domain Logic
│   │   │   ├── commands/  # IPC Handlers
│   │   │   └── main.rs    # Entry Point
│   │   ├── Cargo.toml
│   │   └── build.rs
│   ├── src/               # Frontend (React)
│   ├── public/
│   ├── package.json
│   └── tsconfig.json
```

## 2. IPC Pattern (The Bridge)
Communication between Rust and React follows a strict Request/Response model.

-   **Commands**: All actions initiated by the user (e.g., "Launch Game") are Tauri Commands.
-   **Events**: Background state changes (e.g., "Process 1234 Exited") are sent via `window.emit('process-exit', payload)`.
-   **Payloads**: All payloads must be Serializable (Rust `Serde`) and typed in TypeScript.

## 3. Build & CI
-   **Dev Mode**: `tauri dev` runs the Rust backend + Vite HMR server.
-   **Release**: `tauri build` compiles Rust to a single `.exe` and bundles the React assets.
-   **Resources**: Any external assets (icons, configs) must be registered in `tauri.conf.json` resources.
