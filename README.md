# D2RMultiplay (v0.5.2)

[English](#english) | [简体中文](#chinese)

---

<a name="english"></a>

## 🇺🇸 English Guide

### 📂 Versions Explained
* **Green (Recommended):** Standard folder (EXE + DLLs). Fastest and most stable.
* **Portable:** Single EXE file. Convenient but slightly slower startup.
* **Lite:** Requires **.NET 10** environment.

### ⚙️ How it Works
1.  **Native Isolation:** Uses Windows user management for secure instance separation.
2.  **Handle & Directory Management:** * **Handles:** Clears the D2R mutex handle to allow multiple windows.
    * **Directory Isolation:** Battle.net blocks multiple logins from the same path. Our tool automates a "Backup -> Clear Process -> Data Swap -> Restore" cycle to ensure each instance thinks it’s in a unique environment.
3.  **Automation:** Provides a seamless one-click login after the initial setup.

### ✍️ Author's Note
A milestone of my first collaboration with AI. If you're interested in the logic, feel free to explore or build your own version!

---

<a name="chinese"></a>

## 🇨🇳 中文说明

### 📂 版本说明
* **绿色版 (推荐):** 标准文件夹形式，最稳定，启动最快。
* **单文件版:** 只有一个 EXE，运行后自动释放环境，适合追求极致清爽的用户。
* **依赖版:** 体积最小，但需要手动安装 **.NET 10**。

### ⚙️ 核心逻辑
1.  **底层隔离:** 采用 Windows 原生用户隔离，保证多开的安全与独立。
2.  **多开突破:** * **句柄处理:** 自动杀掉 D2R 进程中的句柄检测。
    * **目录区隔:** 针对战网“同路径无法多开”的限制，通过自动化的“备份-数据交换-还原”流程，为每个实例模拟独立的运行环境。
3.  **全自动流程:** 解决了无法通过命令直接启动游戏的问题，实现一键登录。

### ✍️ 开发者寄语
本项目是我与 AI 合作的纪念作品。核心逻辑如上所述，如果你有 AI 工具，也可以参考这些思路去实现自己的工具。感谢支持！