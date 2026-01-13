<span id="cn"></span>

[ 简体中文 ](#cn) | [ English ](#en)

# D2RMultiplay v0.5.5

D2R 自动化多开与免验证登录工具。

## 🚀 核心方案

### 1. 免重复登录与验证
基于 **Windows 原生用户隔离**。不改文件，实现多账号免验证稳定登录。

### 2. D2R 多开限制突破
- **句柄 (Handle)**: 使用 C# 底层 API 自动清理 Mutex 互斥句柄。
- **目录 (Directory)**: 逻辑流程解决路径占用限制 —— **快照当前设置 ➔ 关闭战网 ➔ 杀掉 Mutex ➔ 还原设置**。实现单路径多开。

### 3. 用户体验
- **一键启动**: 登录一次，设置一次路径，后续直接一键丝滑启动。
- **暗黑模式**: 支持深色主题 (Dark Theme)，护眼且更沉浸。
- **多语言**: 界面支持中英文切换。

## ⚠️ 技术限制 (必读)

为了保证多开功能的稳固运行，本程序有以下严格的技术限制：

1.  **战网路径 (Battle.net Path)**
    *   **必须** 安装在默认路径：`C:\Program Files (x86)\Battle.net\`。
    *   程序目前硬编码了此路径，不支持自定义战网位置。如果未能在该位置找到战网，程序将报错。

2.  **安装权限 (Installation Scope)**
    *   安装战网时，**必须** 确保它是为“**所有用户 (All Users)**”安装的（通常是默认选项）。
    *   **严禁** 选择“仅为当前用户安装”或安装在 `AppData` 等个人目录下。
    *   **原理**：程序创建的隔离子账号（Ghost User）必须有权限读取战网的核心文件。如果战网安装在您的个人隐私目录下，子账号将因权限不足（Access Denied）而无法启动，导致多开失败。

3.  **配置隔离 (Config Isolation)**
    *   程序依赖于切换 `ProgramData` 下的共享配置文件 (`product.db`) 来实现账号隔离。非标准安装可能导致此机制失效。

## 📖 界面指南

![Interface Mockup](docs/images/interface_mockup.png)

### 功能区域
- **左侧列表 (Left Panel)**: **账号管理**。用于创建和关联 Windows 隔离账号。每个账号对应一个独立的 Battle.net 实例。
- **右侧操作 (Right Panel)**: **启动控制**。包含游戏路径设置、配置文件快照管理以及启动按钮。
- **状态栏 (Status Bar)**: **系统状态**。显示是否具有管理员权限 (Admin Rights) 以及系统消息。

### 关键字段
- **Windows User**: **系统账号**。工具依赖 Windows 原生多用户机制来实现隔离。你需要为每个游戏账号创建一个对应的 Windows 账号。
- **Password**: **账号密码**。用于工具自动创建 Windows 账号或进行登录切换。密码仅加密存储在本地 `accounts.json` 中。
- **Game Path**: **游戏路径**。指定 `D2R.exe` 的位置。这里只是作为备注，没有任何实际意义，就是怕你忘了这个账户用的是什么路径的游戏。你也可以使用 "Mirror" 功能为每个账号创建虚拟路径。虚拟路径和真实路径的区别就是虚拟路径会让你所有的窗口都汇集在任务栏的一个窗口图标下面，真实路径会让你的窗口分散在任务栏的多个图标下面，你可以自主拉动窗口到你想要的顺序。
- **BattleTag**: **战网昵称**。仅作为列表显示的备注，方便你区分账号。
- **One-Click Launch**: **一键启动**。执行标准流程：**备份当前配置 -> 清理战网进程 -> 还原目标配置 -> 启动游戏**。

## 📦 版本说明

| 版本 | 说明 |
| :--- | :--- |
| **D2RMultiplay_v0.5.5_Green.zip** | **推荐**。绿色版，解压即用，启动速度最快。 |
| **D2RMultiplay_v0.5.5_Portable.zip** | 单文件版。只有一个 EXE，启动时需释放临时文件（稍慢）。 |
| **D2RMultiplay_v0.5.5.zip** | 依赖版。体积最小，但需要您自行安装 .NET 10 运行库。 |

## 💡 提示与免责

- **权限**: 请以 **管理员权限** 运行，否则无法清理系统句柄。
- **误报**: 涉及句柄操作，如遇杀毒软件拦截请添加白名单。
- **风险**: 本工具仅供技术交流。使用产生的后果（如封号、损失）由用户自行承担。本项目与 Blizzard Entertainment 无关。

## 💰 支持

如果您觉得本工具有帮助，欢迎请作者喝杯咖啡！
- **捐赠**: 内置捐赠入口 (Alipay / WeChat / PayPal)。可点击界面右上角 ❤️ 图标查看。

---

<span id="en"></span>

[ 简体中文 ](#cn) | [ English ](#en)

# D2R Multiplay Tool (v0.5.5)

Simplest D2R Multi-Opening Tool | Lightweight & Open Source | Safe Process Isolation

[简体中文](README.md) | [English](README_EN.md)

## Core Features

- **Minimize to System Tray**: Hide window to tray, double-click to restore. (New!)
- **True Process Isolation**: Each game instance runs with independent environment variables, preferences, and permissions.
- **One-Click Launch**: Automates switching Windows users and launching Battle.net.
- **No Virtual Machines**: Uses native Windows `CreateProcessWithLogonW` API.
- **Safe & Clean**: No modifications to game memory, no DLL injection. 100% legal.
- **Handle Killer**: Automatically closes "Check for Other Instances" handle (Mutex).
- **Config Snapshot**: Automatically backups and restores `product.db` for auto-login.
- **Mutex**: Automatically cleared via low-level C# API.
- **Directory**: Single-path multi-boxing via automated workflow: **Snapshot config ➔ Close Bnet ➔ Kill Mutex ➔ Restore config**.

### 3. User Experience
- **One-Click**: Set up login and directory once. Seamless one-click launch thereafter.
- **Dark Mode**: Native dark theme support.

## ⚠️ Technical Constraints (Read Carefully)

To ensure stable multi-boxing, this tool has strict technical requirements:

1.  **Battle.net Path**
    *   **MUST** be installed in the default location: `C:\Program Files (x86)\Battle.net\`.
    *   The tool currently hardcodes this path. Custom Battle.net locations are not supported and will cause errors.

2.  **Installation Scope**
    *   Battle.net **MUST** be installed for "**All Users**" (usually the default).
    *   **DO NOT** install for "Current User Only" or in private directories like `AppData`.
    *   **Reason**: The isolated sub-accounts (Ghost Users) created by this tool need read permissions for Battle.net core files. If installed in a private directory, sub-accounts will be denied access, causing launch failures.

3.  **Config Isolation**
    *   The tool relies on swapping the shared configuration file (`product.db`) in `ProgramData` to achieve account isolation. Non-standard installations may break this mechanism.

## 📖 Interface Guide

![Interface Mockup](docs/images/interface_mockup.png)

### Interface Areas
- **Left Panel**: **User Management**. Create and link Windows isolation accounts here.
- **Right Panel**: **Launch Controls**. Game path settings, config snapshots, and launch buttons.
- **Status Bar**: **System Status**. Shows Admin status and system messages.

### Key Fields
- **Windows User**: **System Account**. The tool relies on Windows native multi-user mechanism for isolation. You need one Windows user per game account.
- **Password**: **Account Password**. Used for auto-creation or login switching. Stored encrypted locally in `accounts.json`.
- **Game Path**: **Game Path**. Location of `D2R.exe`. This is primarily a reference note to remind you which path this account uses. You can also use the "Mirror" function to create virtual paths. **Difference**: "Virtual Paths" group all windows under one taskbar icon, while "Real Paths" separate windows into individual icons, allowing you to rearrange window order manually.
- **BattleTag**: **BattleTag**. Display remark only, helping you identify accounts in the list.
- **One-Click Launch**: **Standard Workflow**: Backup config -> Clean processes -> Restore target config -> Launch game.

## 📦 Releases

| Version | Description |
| :--- | :--- |
| **D2RMultiplay_v0.5.5_Green.zip** | **Recommended**. Folder-based, fast launch. |
| **D2RMultiplay_v0.5.5_Portable.zip** | Single-file executable (Self-contained). |
| **D2RMultiplay_v0.5.5.zip** | Dependent version (Requires .NET 10). |

## 💡 Tips & Disclaimer

- **Privilege**: Run as **Admin** required to clear system handles.
- **False Positive**: Anti-virus software might flag handle operations; please whitelist if necessary.
- **Risk**: Use at your own risk. Not affiliated with Blizzard Entertainment.

## 💰 Support

If you find this tool helpful, consider buying me a coffee!
- **Donation**: Built-in support (Alipay / WeChat / PayPal). Click the ❤️ icon in the top right.

---

**GitHub:** [https://github.com/yoyoset/D2RMutiplay](https://github.com/yoyoset/D2RMutiplay)