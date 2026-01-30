---
name: CSS_MASTER_TAILWIND
description: Advanced visual design and Tailwind CSS standards for the D2R Multi project. Focuses on "Imperial Gold" aesthetics, glassmorphism, and performance-aware animations.
---

# CSS Master & Visual Design Standards

## 1. Core Aesthetic: "Imperial Gold & Void"

The project uses a strict, high-contrast theme combining deep void backgrounds with premium gold accents.

### Color Palette (Tailwind Config)

- **Void (Backgrounds)**: `zinc-950` (#09090b) to `black`. Avoid pure `#000` unless for contrast.
- **Imperial Gold (Primary)**:
  - Base: `#d4af37` (Standard Gold)
  - Glow: `rgba(212, 175, 55, 0.5)`
  - Text: `text-gold` / `text-yellow-600` (for darker backgrounds)
- **Status Colors**:
  - Success: `emerald-500` (muted green)
  - Error: `red-500` to `red-900` gradient (blood red)
  - Neutral: `zinc-400` (secondary text), `zinc-600` (borders)

## 2. Tailwind Best Practices

### Utility-First, Component-Second

- **DO**: Use utility classes directly in JSX for layout and spacing.
- **DO**: Use `cn()` (clsx + tailwind-merge) for conditional classes.
- **DON'T**: Create CSS classes (`.btn-primary`) using `@apply` unless the component is reused >5 times.

### Linear Style Standards (Clean & Minimal)

Move away from heavy luxury effects. Use subtle, high-precision lines and glass effects for a professional feel.

```tsx
// Linear Glass
className="bg-black/40 backdrop-blur-xl border border-white/5 shadow-2xl transition-all"

// Subtle Border Glow
className="shadow-[0_0_20px_rgba(59,130,246,0.1)] hover:shadow-[0_0_30px_rgba(59,130,246,0.2)] transition-shadow duration-500"
```

## 3. Topography & Layout

- **Font Stack**: System Sans (`Inter` logic) for UI, `Monospace` (`JetBrains Mono` logic) for logs/versions.
- **Tracking**: Use `tracking-wide` or `tracking-widest` for uppercase headers.
- **Whitespace**: "Neo-Clean" demands ample padding. `p-6` or `p-8` for cards.

## 4. Animation Guidelines

- **Performance**: Animate only `transform`, `opacity`, `filter`. Avoid `width`, `margin`.
- **Durations**:
  - Micro-interactions (Hover): `duration-200` to `duration-300`
  - Modals/Panels: `duration-500` (smooth entry)
  - Backgrounds: `duration-1000` (subtle shifting)

## 5. Mobile vs Desktop

- **Desktop First**: This is a purely desktop application.
- **Responsive**: Ensure grids `grid-cols-1 md:grid-cols-2` work for resize, but prioritize 1080p+ layouts.
