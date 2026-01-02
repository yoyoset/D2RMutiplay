# Technical Architecture

## 1. System Overview
The system uses a **Controller-Agent Pattern**, leveraging Windows Multi-User mechanisms for environment isolation.

### 1.1 Architecture Diagram
```mermaid
graph TD
    Main[Main Controller (Admin)] -->|Manage| UserManager[Windows User Manager]
    Main -->|Launch| HandleKiller[Handle Cleaner]
    Main -->|Monitor| FileWatcher[Config Cleaner]
    
    UserManager -->|CreateProcessWithLogonW| Instance1[Battle.net Instance 1 (User A)]
    UserManager -->|CreateProcessWithLogonW| Instance2[Battle.net Instance 2 (User B)]
    
    Instance1 --> Game1[D2R Game Process 1]
    Instance2 --> Game2[D2R Game Process 2]
    
    subgraph Isolation [Isolation Environment]
        UserA_Data[User A AppData/Registry]
        UserB_Data[User B AppData/Registry]
    end
    
    Instance1 -.-> UserA_Data
    Instance2 -.-> UserB_Data
```

## 2. Module Design

### 2.1 Module A: Windows Shadow Account Manager
*   **Responsibilities**:
    1.  Check, create, or **load** existing Windows accounts.
    2.  **Critical**: Call `LoadUserProfile` API to initialize user environment (AppData).
    3.  Launch using Windows API (`LogonUser`, `CreateProcessWithLogonW`).
*   **Interface**:
    ```csharp
    public interface IWindowsUserManager {
        bool UserExists(string username);
        void EnsureUserExists(string username, string password); // Create if not exists
        bool LoadUserProfile(string username); 
        void DeleteUser(string username); 
        int LaunchProgramAsUser(string username, string password, string programPath, string args);
        List<string> GetLocalUsers(); // v0.3.5g
    }
    ```

### 2.2 Module B & C: Launch & Isolation
*   **Sub-modules**:
    *   **ConfigManager (Snapshot/Restore)**: Manages `product.db` swapping to ensure correct Game Path per user.
    *   **HandleKiller**: Closes `DiabloII Check For Other Instances` mutex.
    *   **JunctionManager**: Creates physically isolated game paths.
*   **Key Logic (The Launch Flow)**:
    1.  **Snapshot**: Backup public `product.db` to `{PreviousUser}.json`.
    2.  **Clean**: Kill Battle.net/Agent processes.
    3.  **Restore**: specific `{CurrentUser}.json` -> public `product.db`.
    4.  **Launch**: Start Battle.net.

### 2.3 Module D: UI & State Monitor
*   **MVVM Design**:
    *   **ViewModel**: `MainViewModel` handles business logic.
    *   **Localization**: `IsChinese` property creates real-time language switching.
    *   **Interaction**: Modal dialogs (`CreateUserWindow`) for complex inputs.
*   **View**: `MainWindow.xaml`, `CreateUserWindow.xaml`.

## 3. Data Flow
1.  **Add Account**: 
    *   UI Dialog -> VM calls `UserManager.EnsureUserExists` -> OS creates user.
    *   UI calls `IsolationEngine.CreateGameJunction` -> Generates path -> Saves mapping.
2.  **Launch**:
    *   **Backup**: Previous user's config saved.
    *   **Clean**: Processes terminated.
    *   **Restore**: Current user's config applied.
    *   **Launch**: `WindowsUserManager` launches Battle.net.
    *   **Isolation**: Unique Windows Profile prevents session conflicts.

## 4. Tech Stack
*   **Language**: C# / .NET 6+ (WPF).
*   **Privilege**: `requireAdministrator`.
