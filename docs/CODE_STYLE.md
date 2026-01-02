# Coding Standards

## 1. General
All contributors must follow these standards for consistency.

## 2. Naming Conventions
*   **Variables**: `camelCase` (e.g., `userName`).
*   **Classes**: `PascalCase` (e.g., `UserManager`).
*   **Constants**: `UPPER_SNAKE_CASE` (e.g., `MAX_RETRIES`).

## 3. Directory Structure
```
/src
  /D2RMultiplay.Core              # Core Interfaces & Models
  /D2RMultiplay.Modules
     /ModuleA_AccountManager      # Windows Account API
     /ModuleB_LaunchController    # Launch Logic
     /ModuleC_IsolationEngine     # Config & Handle Cleaning
  /D2RMultiplay.UI                # WPF MVVM
```

## 4. Commenting
*   Code should be self-documenting.
*   Comments explain "Why", not "What".
*   **Language**: English is recommended.

## 5. Git Workflow
*   `main`: Stable production.
*   `develop`: Development.
*   `feature/xxx`: New features.
*   **Commits**: `feat: ...`, `fix: ...`, `docs: ...`.
