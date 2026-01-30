# Changelog / 更新日志

All notable changes to **D2RMultiplay** will be documented in this file.
本项目的所有重大更改都将记录在此文件中。

## [v0.2.0] - 2026-01-30 - Modern Linear Rewrite

### 🎨 Modern Linear 设计语言 (Design Language)

- **全新 UI 架构**：基于 React 19 + Zustand + Tailwind 的现代化前端重写。
  - **New UI Architecture**: Modern frontend rewrite based on React 19 + Zustand + Tailwind.
- **Modern Linear 风格**：极简、线性、克制的视觉语言，告别旧版帝国金外观。
  - **Modern Linear Style**: Minimalist, linear, and restrained visual language.
- **自定义主题**：支持 6 种主题色（极简蓝/紫罗兰/翡翠绿/琥珀金/蔷薇红/金属灰）。
  - **Custom Themes**: 6 theme color options with persistent storage.

### 🌍 100% 国际化 (Internationalization)

- **五语支持**：简体中文/繁體中文/English/日本語/한국어。
  - **Five Languages**: Full support for 5 languages with automatic fallback.
- **语言持久化**：应用会记忆用户的语言选择，启动时自动恢复。
  - **Language Persistence**: Selection saved to localStorage and auto-restored.
- **系统托盘同步**：托盘菜单语言随 UI 语言动态切换。
  - **Tray Language Sync**: Tray menu language changes dynamically with UI.

### ⚡ 性能与稳定性 (Performance & Stability)

- **零闪烁渲染**：通过 Zustand 细粒度订阅与 React.memo 消除了全局重绘。
  - **Zero-Flicker Rendering**: Granular Zustand subscriptions eliminate global repaints.
- **白屏问题修复**：窗口默认隐藏，待 LCP 就绪后再显示。
  - **White Flash Fix**: Window hidden by default, shown only after LCP is ready.
- **单实例保障**：集成 tauri-plugin-single-instance 防止多开主程序。
  - **Single Instance**: Integrated single-instance plugin to prevent duplicate launches.

### 🛠️ 功能增强 (Feature Enhancements)

- **调试日志开关**：可在设置中开启/关闭 debug.log 并一键清理。
  - **Debug Log Toggle**: Enable/disable logging and one-click cleanup from settings.
- **检查更新入口**：版本号旁新增"检查更新"可点击链接。
  - **Check Updates Entry**: Clickable update link next to the version number.
- **账户有效性校验**：后端自动比对绑定的 Windows 账户是否真实存在。
  - **Account Validity Check**: Backend verifies if bound Windows accounts exist.

## [v0.1.2] - 2026-01-28

### 🛡️ Account Safety & UX (账户安全与交互)

- **Account Existence Verification**: The backend now checks if bound Windows accounts exist in the system.
  - **账户存在性校验**：后端现在会自动比对已绑定的 Windows 账户是否在系统中真实存在。
- **UI "Not Found" State**: Added clear visual warnings (breathing light effect) if a linked account is missing.
  - **“账号未找回”状态**：如果绑定的系统用户被删除，界面将显示警示状态。
- **Lifecycle Guidenance**: Added hints to clarify that the app manages mappings, not system users.
  - **逻辑澄清**：增加了交互提示，明确说明程序仅管理映射，不负责系统用户的生命周期。

### 🧹 Cleanup Logic (清理逻辑更新)

- **Extended Process List**: Added `Battle.net Helper.exe`, `Blizzard Browser.exe`, and `BlizzardError.exe` to the cleanup target list.
  - **清理名单扩容**：将战网辅助进程全面纳入清理范围，确保“一键启动”环境绝对纯净。
- **Accuracy over Count**: Refined logic to prioritize cleanup necessity over simple process counts.
  - **逻辑审计**：以准确性和必要性为准，优化了清理链路。

### ⚡ Performance & Stability (性能与稳定)

- **Zero-Flicker UI**: Localized account polling and implemented `React.memo` to eliminate global UI flickers.
  - **零闪烁 UI**：将刷新逻辑局部化并应用组件缓存，彻底解决了后台刷新导致的界面跳动。
- **Static Background**: Solidified background rendering to ensure visual stability during data updates.
  - **背景固化**：优化了背景纹理渲染层级，确保在 Webview 中拥有更稳健的视觉表现。

### 📝 Log Management (日志管理)

- **Logging Toggle**: Added a global switch to enable or disable debug logging in Settings.
  - **日志开关**：在设置中增加了全局日志开关，可根据需要开启/关闭 `debug.log`。
- **One-Click Clear**: Added a "Clear Log File" button to easily maintain disk space.
  - **一键清理**：支持从 UI 直接物理删除日志文件，方便维护。

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
