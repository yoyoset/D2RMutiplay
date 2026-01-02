# Changelog

## v0.4.1 (2026-01-02)

### Core Logic Updates
*   **Config Snapshot/Restore**: Implemented robust Battle.net configuration management.
    *   **Backup**: Automatically saves the previous user's `product.db` to prevent data loss.
    *   **Restore**: Restores the current user's specific `product.db` before launch, ensuring the correct **Game Path** is loaded.
*   **Documentation**:
    *   Synchronized all documentation to **v0.4.1**.
    *   Clarified "One-Click Launch" workflow (Snapshot -> Clean -> Restore -> Launch).
    *   Corrected "Auto Login" to "Session Persistence" (utilizing Windows Profile Isolation).

## v0.3.5t (2025-12-29)

### Features & Refinements
*   **UI/UX**: 
    *   Completed 14 rounds of refinement.
    *   Implemented "Must/Suggest/Exception" guidance system.
    *   Moved Create/Link user actions to Modal Dialogs.
    *   Added Language Switcher (English/Chinese).
    *   Standardized button heights and layout.
*   **Safety**:
    *   "Delete User" now requires confirmation and has a red high-risk styling.
    *   Added duplicate path detection warnings.
*   **Core**:
    *   Decoupled "Path Selection" and "Create Mirror" logic.

### Performance Metrics
*   **Codebase**: ~4,000 LOC across 4 modules.
*   **Efficiency**: AI-assisted development reduced estimated 4.5 man-days of work to ~3 hours.
