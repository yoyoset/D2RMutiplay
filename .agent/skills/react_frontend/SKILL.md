---
name: REACT_FRONTEND_IMPERIAL
description: Standards for the "Imperial Gold" React frontend, including ShadCN, Tailwind, and Zustand usage.
---

# React Frontend Standards: The "Imperial Gold" Interface

## 1. Visual Language ("Imperial Gold")
The UI must feel premium, dark, and game-native.

-   **Color Palette**:
    -   `bg-void`: `#09090b` (Deepest background)
    -   `bg-card`: `#18181b` (Card surface)
    -   `border-gold`: `#d4af37` (Imperial Gold borders)
    -   `text-gold`: `#fce8a3` (Highlighted text)
-   **Effects**:
    -   Glassmorphism: `backdrop-blur-md bg-black/40`
    -   Glow: `shadow-[0_0_15px_rgba(212,175,55,0.15)]` on hover.

## 2. Architecture (Feature-Sliced Lite)
Avoid deep nesting. Group by feature.

-   `src/components/ui/`: Atomic components (Button, Input) - likely ShadCN UI.
-   `src/features/dashboard/`:
    -   `hooks/`: `useAccountGrid.ts`
    -   `components/`: `AccountCard.tsx`, `LaunchButton.tsx`
-   `src/features/settings/`: Code related to configuration.
-   `src/store/`: Zustand stores (`useAppStore.ts`).

## 3. State Management (Zustand)
-   Single store for global app state (`AppStore`).
-   Actions must be colocated with state.
-   **Persistence**: Use `persist` middleware to save non-sensitive UI settings to localStorage.

## 4. Tauri Integration
-   **Strict Typing**: NEVER call `result.payload`. Define TypeScript interfaces for all Rust return types.
-   **API Layer**: Wrap `invoke` calls in `src/api/rust.ts`.
    ```typescript
    // src/api/rust.ts
    export const launchGame = async (id: string): Promise<Result<void, AppError>> => {
        return invoke('launch_game', { id });
    }
    ```
