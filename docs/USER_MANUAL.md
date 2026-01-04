# D2R Multiplay User Manual

**Version**: v0.5.3
**Updated**: 2026-01-02

---

## 1. Introduction
This tool is designed for *Diablo II: Resurrected* (D2R) players to manage multiple accounts and instances.
**Core Principle**: Uses Windows Multi-User mechanisms for isolation combined with **"Config Snapshotting"** to manage Game Paths, and "Scorched Earth" mutex cleaning to achieve seamless single-PC multi-boxing.

## 2. Prerequisites
1.  **Run as Administrator**: Required for user creation and handle manipulation.
2.  **Installation**: Ensure D2R and Battle.net are installed.
3.  **Settings**: Uncheck "Exit Battle.net when launching game" in Battle.net settings.

---

## 3. Account Management
Configure mappings between "Game Accounts" and "Windows System Users" in the left panel.

### 3.1 Interface Overview
*   **Top Buttons**: [Link Existing User], [New Windows User].
*   **List**: Displays configured accounts.
*   **Bottom Tools**: [Save Changes], [Delete System User], [Remove from List].
*   **Language**: Top-right dropdown to switch between English / Simplified Chinese.

### 3.2 Managing Accounts

#### 3.2.1 Adding Accounts
1.  **New Windows User (Recommended)**:
    *   Click **[New Windows User]**.
    *   Enter **Windows Username** (e.g., `D2R_01`) and **Password**.
    *   **BattleTag**: Your game tag (for display only).
    *   **Remarks**: Any notes.
    *   Click **[Create]**.
    *   **Important**: You must manually login to this new Windows account **once** to initialize the desktop environment and browser settings.

2.  **Link Existing User**:
    *   Click **[Link Existing User]**.
    *   Select a local user and enter the password.
    *   Click **[Link]**.

#### 3.2.2 Path Binding & Mirroring
1.  Select an account.
2.  **Option A (Manual)**: Browse to an existing `D2R.exe`.
3.  **Option B (Auto Mirror - Recommended)**:
    *   Select your source game path.
    *   Click **[Mirror]**.
    *   Tool creates a `D2R_Clone_{Username}` virtual folder.
    *   **Note**: Each account must use a unique path to avoid Battle.net conflicts.

### 3.3 Maintenance
*   **Remove from List**: Removes from tool only.
*   **Delete System User** (Red Button): Executes `net user /delete`. **Irreversible** - deletes all save data for that Windows user.

---

## 4. Launch Operations
Use the right panel to launch the game.

### 4.1 One-Click Launch (Recommended)
Green Button: **[One-Click Launch]**
*   **Logic (Snapshot/Restore)**:
    1.  **Backup**: Saves the previous user's `product.db` (Battle.net config) to a snapshot.
    2.  **Clean**: Kills all Battle.net processes and closing game handles.
    3.  **Restore**: Restores the *current* user's `product.db` snapshot (ensuring Battle.net points to the correct Game Path).
    4.  **Launch**: Starts Battle.net as the selected user.
*   **Next Steps**: 
    *   **First time**: Manually log in to Battle.net.
    *   **Subsequent times**: You will be auto-logged in thanks to Windows Session Persistence.
    *   Click "Play".

### 4.2 Direct Launch
Grey Button: **[Direct Launch]**
*   **Actions**: Starts Battle.net as the selected user **without** cleanup.
*   **Use Case**: Debugging or opening a second instance when cleanup is already done.

### 4.3 Setup Note
If the Battle.net login redirects to a browser and hangs:
1.  Log out of your main Windows account.
2.  Log in to the shadow account (e.g., `D2R_01`).
3.  Open Battle.net manually and set the default browser.
4.  Log out and switch back.

---

## 5. FAQ

**Q: Why does it open Battle.net instead of the game directly?**
A: Battle.net login tokens are complex. We launch a "Shadow Battle.net" to handle authentication reliability.

**Q: "Process has exited" error?**
A: Fixed in v0.3.3. We now use targeted handle scanning.

**Q: Why are there two Battle.net icons?**
A: Each runs under a different Windows user. This is normal and desired.
