# Changelog

## v0.5.4 (2026-01-11) - Process Cleanup
### Fixed
- **Isolation Engine**: Added `crashpad_handler.exe` to the process cleanup list to prevent accumulation of zombie processes that may hinder game launching.

## v0.5.3 (2026-01-04) - Documentation & Localization
### Added
- **UI Guide**: Added detailed visual interface guide to README (Bilingual).
- **Localization**: Added missing localization for "User Created" success popup.
- **Localization**: Fixed "Create Windows User" window labels not translating correctly.

### Changed
- **Documentation**: Refactored README to split Chinese and English content into distinct sections.
- **Documentation**: Updated "Game Path" explanation to clarify Virtual vs Real path behavior in taskbar.
- **Project**: Updated release version info.

## v0.5.2 (2026-01-02) - Dark Mode & UI Refinement

### Features
*   **Dark Mode Support**: Introduced a global Dark/Light theme toggle (Sun/Moon icon).
    *   **Theme Persistence**: Automatically remembers your last used theme on startup.
    *   **UI Polish**: Refined button styles, input backgrounds, and dropdowns for optimal visibility in dark environments.
    *   **Consistent Experience**: Extended theming to all secondary windows (Create User, Link User).

## v0.5.1 (2026-01-02) - Donation & Community

### Features
*   **Donation Window**: Added "Donate & Boost Luck" feature to support development.
    *   Supported methods: WeChat Pay, Alipay, Paypal.
    *   Accessible via the header heart icon (💜).

## v0.5.0 (2026-01-02) - Localization

### Features
*   **Multi-Language Support**:
    *   Added support for **English (Default), Simplified Chinese, Traditional Chinese, Japanese, and Korean**.
    *   Refactored UI to use a dynamic `LocalizationManager` for runtime language switching.
    *   Updated all documentation (README) to reflect bilingual changes.

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
