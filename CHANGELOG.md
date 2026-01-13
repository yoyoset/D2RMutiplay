# Changelog / 更新日志

All notable changes to **D2RMultiplay** will be documented in this file.
本项目的所有重大更改都将记录在此文件中。

## [v0.5.6] - 2026-01-13
### 🛡️ Safety (安全性)
- **User Deletion Protection**: Added a safety block to prevent deleting the currently logged-in Windows user.
  - **用户删除保护**：增加了安全拦截，防止误删当前正在登录的 Windows 用户。
- **Double Confirmation**: Added a secondary confirmation dialog when attempting to delete System/Admin users.
  - **双重确认**：在尝试删除系统/管理员用户时，增加了二次确认对话框。

### 💾 Usability (易用性)
- **Language Persistence**: The application now saves the selected language (`English`/`简体中文`) to `settings.json` and automatically loads it on startup.
  - **语言记忆**：程序现在会将选择的语言保存到 `settings.json`，并在启动时自动加载。
- **Settings**: Introduced `settings.json` for persisting user preferences (Theme, Language).
  - **配置文件**：引入 `settings.json` 用于持久化保存用户偏好（主题、语言）。
- **Password Hint**: Added a hint below the password input box ("No password required for Current User").
  - **密码提示**：在密码输入框下方增加了提示：“当前登录的 Windows 用户无需填写密码”。

## [v0.5.5] - 2026-01-13
### 🟢 New Features (新功能)
- **Minimize to System Tray**: The application now minimizes to the Windows notification area (System Tray) instead of the taskbar.
  - **最小化到托盘**：程序现在会最小化到 Windows 通知区域（系统托盘），而不是占用任务栏。
- **Tray Icon**: Added a tray icon with a context menu (Show/Exit) and double-click restore functionality.
  - **托盘图标**：添加了托盘图标，支持右键菜单（显示/退出）和双击还原功能。
- **Resource Management**: Added `app.ico` to build output for proper icon display.
  - **资源管理**：修复了 `app.ico` 图标资源问题，确保图标正确显示。

### 🔧 Improvements (改进)
- **UI Polish**: Moved version number from Window Title to the status bar footer.
  - **界面优化**：将版本号从窗口标题栏移至状态栏底部。
- **Code Stability**: Fixed `System.Windows.Forms` namespace ambiguities and improved disposal logic for the NotifyIcon.
  - **代码稳定性**：修复了命名空间冲突，并优化了托盘图标的资源释放逻辑。

## [v0.5.4] - 2026-01-12
### 🛠 Fixes & Improvements (修复与改进)
- **Automatic Crashpad Cleanup**: Added `crashpad_handler.exe` to the automatic process cleanup list.
  - **自动清理 Crashpad**：将 `crashpad_handler.exe` 加入了自动清理列表。
- **Issue**: Previously, `crashpad_handler.exe` processes would accumulate as "zombie processes", causing Battle.net Agent to refuse starting.
  - **问题**：此前，`crashpad_handler.exe`（战网崩溃报告工具）可能会残留为僵尸进程，导致战网代理无法启动。
- **Fix**: The Isolation Engine now forcefully terminates `crashpad_handler.exe` alongside Battle.net/Agent during launch, ensuring a clean environment.
  - **修复**：隔离引擎现在会在“一键启动”时强制终结该进程，确保环境彻底干净。

## [v0.5.3] - 2026-01-12
### 📖 Documentation (文档)
- **Bilingual README**: Split Chinese and English documentation into distinct sections.
  - **双语 README**：将中英文文档拆分为独立部分。
- **UI Guide**: Added visual interface guide (`interface_mockup.png`).
  - **界面指南**：添加了可视化界面指南图片。

### 🌍 Localization (本地化)
- **Fix**: Resolved hardcoded English text in "Success" popups.
  - **修复**：修复了成功弹窗中的硬编码英文文本。
- **Fix**: Localized labels in the "Create User" window.
  - **修复**：本地化了“创建用户”窗口的标签。
