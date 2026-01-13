# Release Info / 发布信息
- **Tag version**: `v0.5.6`
- **Release title**: `v0.5.6 - Safety & Persistence Update`

---

# Release Notes - v0.5.6

This update focuses on user safety and improved usability based on community feedback.
本次更新主要针对社区反馈，增强了用户操作安全性并改进了易用性。

## 🟢 New Features (新功能)

### 🛡️ Safety (安全性)
*   **User Deletion Protection (用户删除保护)**:
    *   **Self-Deletion Blocked**: Prevents accidental deletion of the currently logged-in Windows user.
    *   **Double Confirmation**: Deleting a System/Admin user now requires a second, explicit confirmation step to prevent data loss.
    *   **自我保护**：程序现在会强制拦截删除“当前登录Windows用户”的操作。
    *   **双重确认**：涉及删除系统/管理员账户时，会弹出二次确认框，防止手滑误删重要数据。

### 💾 Usability (易用性)
*   **Language Persistence (语言设置记忆)**:
    *   The application now remembers your language selection (`English` / `简体中文`) across restarts.
    *   **自动记忆语言**：无需每次启动都手动切换语言了，程序会自动保存并加载您上次选择的语言设置。
*   **Password Hint (密码提示)**:
    *   Added a hint below the password box: "No password required for Current User".
    *   **密码提示**：在密码输入框下方增加了提示：“当前登录的 Windows 用户无需填写密码”。

---

## 📦 Downloads (下载说明)

| File | Description | 说明 |
| :--- | :--- | :--- |
| **D2RMultiplay_v0.5.6_Green.zip** | **Recommended**. Folder-based, fast launch. | **推荐**。绿色版，解压即用，启动速度最快。 |
| **D2RMultiplay_v0.5.6_Portable.zip** | Single-file executable (Self-contained). | 单文件版。只有一个 EXE，启动时需释放临时文件（稍慢）。 |
| **D2RMultiplay_v0.5.6.zip** | Dependent version (Requires .NET 10). | 依赖版。体积最小，但需要您自行安装 .NET 10 运行库。 |
