
## 1. Document Control
| Version | Date | Author | Description |
| :--- | :--- | :--- | :--- |
| v0.4.1 | 2026-01-02 | User/AI | Update: Verified Snapshot/Restore Workflow, Session Persistence |

## 2. Background & Goals
### 2.1 Problem Statement
Players of *Diablo II: Resurrected* (D2R) face difficulties when managing multiple accounts:
1.  **Tedious Login**: Battle.net client makes switching accounts cumbersome, requiring re-entry of passwords or 2FA.
2.  **Multi-Instance Restrictions**: Both the game and Battle.net client have restrictions preventing multiple instances on the same machine.
    *   *Directory Sync Conflict*: Battle.net forces game path sync, causing "Game is running" errors.
    *   *Handle Detection*: Game process includes Mutex handles that prevent multiple instances.
3.  **Browser Login**: New Battle.net versions force browser-based OAuth login, complicating automation.

### 2.2 Objectives
*   **Primary Goal**: Enable stable multi-boxing for D2R with one-click launch for multiple accounts.
*   **Core Experience**: "No Frequent Login" - Utilize independent Windows user profiles to persist login sessions naturally.
*   **Technical Breakthrough**: Resolve Battle.net directory sync conflicts via **Config Snapshot/Restore**.

## 3. User Personas
*   **Multi-boxer**: Owns multiple D2R accounts, wants to play them simultaneously or run bots, requires a minimalist launch workflow.

## 4. User Stories
| ID | Persona | As a... | I want to... | So that... | Priority |
| :--- | :--- | :--- | :--- | :--- | :--- |
| US-001 | Player | Multi-boxer | Save info for multiple Battle.net accounts | I don't have to re-enter passwords | P0 |
| US-002 | Player | Multi-boxer | One-click launch specific accounts | I can quickly enter the game | P0 |
| US-003 | Player | Multi-boxer | Auto-handle browser login redirects | I can skip manual verification | P1 |
| US-004 | Player | Multi-boxer | Run games without interference | I avoid "Game is already running" errors | P0 |
| US-005 | Player | Multi-boxer | Auto-clean game mutex handles | I can open a second window | P0 |

## 5. Functional Requirements

### 5.1 Module A: Windows Account Manager
> **Responsibility**: Manage underlying Windows OS user accounts.
*   **FR-A01**: **Account Management**: Automatically create new accounts (e.g., `D2R_User_1`) or **allow adding existing Windows local accounts**.
*   **FR-A02**: **Environment Init (User Profile Load)**: **Critical**. After creating a user, must call Windows API (`LoadUserProfile`) to programmatically load the profile, ensuring `C:\Users\<Username>` and AppData are created without manual login.
*   **FR-A03**: **Credential Management**: Securely store passwords for `LogonUser` calls.
*   **FR-A04**: **Account Cleanup (Smart Delete)**: Provide function to completely delete Windows accounts (`net user /delete`) and remove from app list.

### 5.2 Core Functions

#### 5.2.1 User Management & Mapping
*   **Management Area**: Side panel for maintaining "Game Account" <-> "Windows User" mapping.
*   **Modal Create (Create Dialog)**: "New Windows User" dialog to input username, password, BatteTag, and remarks.
*   **Modal Link (Link Dialog)**: "Link Existing User" dialog to bind local users.
*   **BattleTag Support**: Display BattleTags in the list.
*   **Path Binding & Mirroring (Junction)**:
    *   Support independent `D2R.exe` path per account.
    *   **Mirror Function**: "Create Mirror" button to create an NTFS Junction (`D2R_Clone_Username`) pointing to the source directory for physical path isolation.
*   **Path Conflict Detection**: Warn if a path is already in use by another account.
*   **Persistence**: Auto-save to `accounts.json`.
*   **Multi-language**: UI supports English/Chinese switching.

#### 5.2.2 Launch Operations (Snapshot/Restore Strategy)
*   **Auto Launch Flow**:
    1.  **Backup**: Backup the *previous* user's Battle.net config (`product.db`) to their private snapshot.
    2.  **Clean**: Terminate all Battle.net/Agent processes and close `DiabloII Check For Other Instances` handles.
    3.  **Restore**: Overwrite the shared `product.db` with the *current* user's private snapshot (restoring their specific Game Path).
    4.  **Launch**: Start Battle.net Client as the target Windows User (Shadow User).
*   **Result**: Battle.net launches with the correct Game Path pre-loaded, avoiding "Update" loops or path conflicts.
*   **Direct Launch**:
    *   Launch Battle.net (Shadow User) directly without Snapshot/Restore steps (for debugging).
*   **Manual Tools**:
    *   Buttons: `Kill Battle.net`, `Delete product.db`, `Close Handles`.

## 6. Non-Functional Requirements
*   **Stability**: Handle operations must not crash the game.
*   **Security**: Account data must be encrypted locally.
*   **Performance**: Launcher should have minimal resource footprint.

## 7. Open Questions
*   [ ] Will handle names change with game updates?
*   [x] Best isolation scheme: **NTFS Junction** (Implemented).
