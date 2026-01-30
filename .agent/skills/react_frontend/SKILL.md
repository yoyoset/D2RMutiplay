---
name: REACT_FRONTEND_IMPERIAL
description: Standards for the "Imperial Gold" React frontend, including ShadCN, Tailwind, and Zustand usage.
---

# React Frontend Standards: The "Imperial Gold" Interface

## 1. Visual Language ("Imperial Gold")

The UI must feel premium, dark, and game-native.

- **Color Palette**:
  - `bg-void`: `#09090b` (Deepest background)
  - `bg-card`: `#18181b` (Card surface)
  - `border-zinc`: `#27272a` (Subtle borders)
  - `text-primary`: `#3b82f6` (Linear Blue highlights)
- **Effects (Linear Style)**:
  - Glass: `backdrop-blur-xl bg-black/40 border-white/5`
  - Glow: `shadow-[0_0_15px_rgba(59,130,246,0.15)]` on hover.

## 2. Architecture (Feature-Sliced Lite)

Avoid deep nesting. Group by feature.

- `src/components/ui/`: Atomic components (Button, Input) - likely ShadCN UI.
- `src/features/dashboard/`:
  - `hooks/`: `useAccountGrid.ts`
  - `components/`: `AccountCard.tsx`, `LaunchButton.tsx`
- `src/features/settings/`: Code related to configuration.
- `src/store/`: Zustand stores (`useAppStore.ts`).

## 3. State Management (Zustand)

- Single store for global app state (`AppStore`).
- Actions must be colocated with state.
- **Persistence**: Use `persist` middleware to save non-sensitive UI settings to localStorage.

## 4. Tauri Integration

- **Strict Typing**: NEVER call `result.payload`. Define TypeScript interfaces for all Rust return types.
- **API Layer**: Wrap `invoke` calls in `src/api/rust.ts`.

    ```typescript
    // src/api/rust.ts
    export const launchGame = async (id: string): Promise<Result<void, AppError>> => {
        return invoke('launch_game', { id });
    }
    ```
