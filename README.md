# D2R Multiplay Agent | 暗黑2多开助手

**D2R Multiplay Agent** is a tool designed to help *Diablo II: Resurrected* players manage multiple accounts and automate the login process. It aims to provide a safe and efficient multi-boxing experience through a simple interface.

**暗黑2多开助手** (D2RMultiplay) 是一个旨在帮助《暗黑破坏神2：重制版》玩家实现多账户管理和自动登录的辅助工具。本项目致力于提供安全、高效的多开体验，通过简单的界面管理多个游戏账户。

---

## Key Features (主要功能)

*   **Account Management (账号管理)**: Centrally manage multiple Battle.net accounts with remarks and status tags. (集中管理多个战网账号，支持账号备注与状态标记。)
*   **One-Click Multi-boxing (一键多开)**: Automatically configure and launch multiple game clients. (自动配置并启动多个游戏客户端，实现多开。)
*   **Session Persistence (会话保持)**: Utilizes isolated Windows User Profiles to maintain independent Battle.net login sessions. Log in once, and stay logged in. (利用隔离的 Windows 用户配置文件来保持独立的战网登录会话。只需登录一次，即可保持长久在线。)
*   **Game Path Mirroring (游戏路径镜像)**: Creates NTFS Junctions (Virtual Mirrors) of the game folder to trick Battle.net into seeing unique paths for each account without duplicating game data. (创建游戏目录的 NTFS 软链接镜像，在不占用额外硬盘空间的情况下，欺骗战网认为每个账号有独立的安装路径，彻底解决“游戏正在运行”错误。)
*   **Window Management (窗口管理)**: Easily switch and arrange multiple game windows. (方便地切换和排列多个游戏窗口。)
*   **Localization (多语言支持)**: Supports **English, Simplified Chinese (简中), Traditional Chinese (繁中), Japanese (日语), Korean (韩语)**. Switch instantly via the dropdown. (支持5种语言实时切换。)
*   **Dark Mode (深色模式)**: Built-in Dark/Light theme toggle for comfortable usage in any environment. (内置深色/浅色主题切换，适应不同环境。)
*   **Community Support (社区支持)**: "Donate & Boost Luck" feature to support the developer. (捐赠与支持功能。)

## Getting Started (快速开始)

### Prerequisites (环境要求)
*   Windows 10 or later (Windows 10 或更高版本)
*   [Visual Studio 2022](https://visualstudio.microsoft.com/)
*   .NET 8 SDK or project specified version (.NET 8 SDK 或项目指定的其他版本)
*   *Diablo II: Resurrected* Game Client (《暗黑破坏神2：重制版》游戏客户端)

### Build & Run (构建与运行)
1.  Clone this repository. (克隆本仓库到本地。)
2.  Open `src/D2RMultiplay.sln` with Visual Studio 2022. (使用 Visual Studio 2022 打开 `src/D2RMultiplay.sln` 解决方案文件。)
3.  Restore NuGet packages. (还原 NuGet 程序包。)
4.  Build in `Debug` or `Release` mode. (选择 `Debug` 或 `Release` 模式进行生成。)
5.  Run the `D2RMultiplay.UI` project. (运行 `D2RMultiplay.UI` 项目。)

## Documentation (文档)

Detailed documentation is available in the `docs/` directory. (详细文档请参阅 `docs/` 目录。)

*   [**Product Requirements / 产品需求文档 (PRD)**](docs/PRD.md)
*   [**Technical Architecture / 技术架构设计**](docs/ARCHITECTURE.md)
*   [**User Manual / 用户使用手册**](docs/USER_MANUAL.md)
*   [**Changelog / 更新日志**](docs/CHANGELOG.md)

## Notes (注意事项)

*   This project is for educational purposes only. Please do not use it for activities that violate the game's Terms of Service. (本项目仅供学习交流使用，请勿用于违反游戏服务条款的用途。)
*   **Sensitive Information (敏感信息)**: Do NOT commit `accounts.json` containing real passwords to public repositories. (请勿将包含真实账号密码的 `accounts.json` 提交到公开仓库。)

## License

[TODO: Add License Information]
