---
name: PRODUCT_DESIGN_MASTER
description: High-level product interaction design standards focusing on fluidity, minimalism, and professional aesthetics.
---

# Master Product Interaction Design

> "Good design is as little design as possible." — Dieter Rams

This skill embodies the principles of top-tier product design (reminiscent of Linear, Arc, or Apple). Use this when the user demands professional, sophisticated, and "magical" user experiences.

## 1. Core Philosophy: The Invisible Flow

The current utility-based UI (Grid -> Click -> Action) is functional but "dumb". A Master UI anticipates intent.

- **Anticipation**: The interface should know what the user likely wants to do next. (e.g., If I just added an account, I probably want to configure it. If I verified a path, I probably want to launch.)
- **State Fluidity**: Never jump-cut. Use micro-interactions to show state changes (e.g., "Idle" morphs into "Launching" which morphs into "Running").
- **Contextual Disclosure**: Hide complexity until needed. A "Snapshot" button shouldn't clutter the card unless there's a *reason* to snapshot (e.g., "Unsaved Changes" detected).

## 2. Interaction Patterns

### 2.1 The "Commander" Model

Instead of static lists, treat the app as a Command Center.

- **Keyboard First**: Global shortcuts (`Ctrl+K` palette) for everything. Power users hate clicking.
- **Focus Mode**: When launching, the specific account should take center stage (Hero View), dimming others.

### 2.2 Direct Manipulation

- **Drag & Drop**: Don't use "Move Up/Down" buttons. Let users drag accounts to reorder their launch sequence.
- **Scrubbing**: Click-and-drag parameters instead of typing numbers.

### 2.3 Visual Feedback

- **Cinematic Logs**: Don't dump text logs. Use visual indicators (Green pulse for success, subtle shake for error).
- **Live Previews**: If possible, show a snapshot of the game window or a high-res class avatar instead of a generic icon.

## 3. Redesign Directions for D2RMultiplay

### A. The "Lobby" Metaphor

Treat the app like the game lobby itself.

- **Visuals**: Use high-fidelity Diablo 2 artwork assets as backgrounds for account cards.
- **Party Mode**: Allow grouping accounts into "Parties" (e.g., "Rush Team A", "Mule Team B"). One click launches the entire party in sequence.

### B. The "Smart Action" Button

Replace specific buttons (Launch/Kill/Snapshot) with a single Context Button that changes based on state:

- **State: Empty** -> Button: "Setup Config"
- **State: Configured** -> Button: "Launch Game"
- **State: Running** -> Button: "Focus Window" or "Kill" (on hover)
- **State: Game Closed** -> Button: "Sync Snapshot" (if changes detected)

## 4. Implementation Rules

1. **Motion is Mandatory**: No state change happens without a transition (Framer Motion / Tailwind Animate).
2. **Sound Design**: Subtle clicks and hums (optional but recommended for "Premium" feel).
3. **Typography**: Use `Inter` or `Geist Mono` for technical details, but extensive tracking/leading for readability.

---
**Checklist for Review**:

- [ ] Does it feel "alive"?
- [ ] Is the primary action obvious?
- [ ] Did we remove at least one unnecessary click?
- [ ] Is it aesthetically stunning (Premium Dark Mode)?
