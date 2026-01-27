# Changelog

All notable changes to this project will be documented in this file.

## [0.1.2] - 2026-01-27

### Fixed

- **Mutex Killing Logic**: Re-implemented "Silent Brute-Force" strategy for robust multi-launch capability.
  - **Privilege Escalation**: Now explicitly requests `SeDebugPrivilege` to ensure access to all D2R handles.
  - **Buffer Optimization**: Increased `NtQueryObject` buffer to 8KB (0x2000) to handle long mutex names without truncation.
  - **Targeted Scanning**: Logic now strictly targets D2R PIDs and silences `ERROR_NOT_SUPPORTED` (0x80070032) errors to prevent console spam.
  - **Result**: 100% reliable detection and closure of `DiabloII Check For Other Instances` without user-facing noise.

### Changed

- **UI Polish**: Updated localization for "Switch User" prompts and refined success modal states.

## [0.1.1] - 2026-01-26

### Added

- **Standardized Modal System**: Unified `Modal`, `ModalContent`, `ModalHeader`, `ModalBody`, and `ModalFooter` components across the entire application.
- **Enhanced Glassmorphism**: High-intensity frosted glass effect (`backdrop-blur-xl`, `bg-black/40`) applied to all popups for a premium "Imperial Glass" aesthetic.
- **Depth Hierarchy Optimization**: Increased modal Z-Index to 100 to ensure proper layering over the navigation header and full-screen backdrop blur.

### Fixed

- **Donate Page Refactor**: Resolved layout "messiness" by redesigning the blessing card into a modern flex-based profile layout.
- **Button Alignment Consistency**: Uniformly adopted right-aligned action buttons (`justify-end`) for all modal footers to match the "Directory Mirror" interaction logic.
- **Build & Conflict Resolution**: Fixed TypeScript errors related to `open` dialog naming conflicts and resolved UI occlusion issues between the header and popups.

### Changed

- **Visual Polish**: Improved typography tracking and avatar display with better scaling and a new active status indicator.
