# D2RMultiplay v0.5.4 Release Notes

## 🛠 Fixes & Improvements

### Process Cleanup (Isolation Engine)
- **Automatic Crashpad Cleanup**: Added `crashpad_handler.exe` to the automatic process cleanup list.
    - **Issue**: Previously, `crashpad_handler.exe` processes (the crash reporting tool used by Battle.net) would accumulate in the background as "zombie processes" after closing games, potentially causing Battle.net Agent to refuse starting new instances.
    - **Fix**: The Isolation Engine now forcefully terminates all instances of `crashpad_handler.exe` alongside `Battle.net.exe` and `Agent.exe` during the "One-Click Launch" sequence, ensuring a completely clean environment for every new game instance.

## 📦 Downloads
| File | Description |
| :--- | :--- |
| **D2RMultiplay_v0.5.4_Green.zip** | **Recommended**. Folder-based, ready to run. |
| **D2RMultiplay_v0.5.4_Portable.zip** | Single executable file. |
| **D2RMultiplay_v0.5.4.zip** | Framework-dependent (requires .NET 10). |
