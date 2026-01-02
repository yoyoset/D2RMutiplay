# Architecture Decision Records (ADR)

## ADR-001: Windows Multi-User Account Isolation

*   **Status**: [Accepted]
*   **Date**: 2025-12-29

### Context
D2R restricts single instances. VM solutions are heavy. We need a lightweight, stable isolation method.

### Decision
Use **Windows Local Multi-User** mechanism:
1.  Create independent Windows accounts per instance.
2.  Use `CreateProcessWithLogonW` to launch.
3.  Combine with `Appdata` isolation.

### Consequences
*   **Pros**: System-level isolation, stable, low resource usage.
*   **Cons**: Requires Admin privileges; first launch needs profile load.

---

## ADR-002: Separation of User Management & Launch Logic

*   **Status**: [Accepted]
*   **Date**: 2025-12-29

### Background
Initial "One-Click" included implicit user user creation, which was opaque and unsafe.

### Decision
Separate **Configuration** from **Execution**:
1.  **Management Area**: Explicit [New User] / [Link User] buttons. No launch trigger.
2.  **Launch Area**: [Auto Launch] (Clean + Start) and [Direct Launch] (Start only).

### Consequences
*   **Pros**: Transparent, safer, flexible.
*   **Cons**: Initial setup requires more clicks.

---

## ADR-003: Targeted Process Handle Scanning

*   **Status**: [Accepted]
*   **Date**: 2025-12-29

### Background
Scanning all system handles caused crashes when unrelated processes exited during scan.

### Decision
**Strictly limit scan to target processes**:
1.  Get PIDs for `D2R` / `DiabloII`.
2.  Get all system handles.
3.  Filter in memory for target PIDs.
4.  Only query names for filtered handles.

### Consequences
*   **Pros**: Zero risk of unrelated crashes, high performance.
*   **Cons**: Requires code update if process name changes.

---

## ADR-004: NTFS Junction for Game Isolation

*   **Status**: [Accepted]
*   **Date**: 2025-12-29

### Background
Battle.net detects "Game Running" based on execution path.

### Decision
Use **NTFS Junctions** to create virtual mirrors:
*   Source: `F:\Games\D2R`
*   Mirror: `F:\Games\D2R_Clone_UserA` -> Source.
*   Point Battle.net to Mirror.

### Consequences
*   **Pros**: Zero disk space usage, bypasses detection.
*   **Cons**: deletion requires care (handled by OS usually).

---

## ADR-005: Modal Dialogs for User Management

*   **Status**: [Accepted]
*   **Date**: 2025-12-29

### Decision
Use **Modal Windows** (`CreateUserWindow`) for sensitive inputs to prevent accidental edits in the main UI.
