# Lithium Design Guidelines

## Core Philosophy
- **Monochrome & High Contrast**: The design relies strictly on the Lithium color scale (dark grays/blacks) and white. No colored accents like indigo or blue.
- **Symmetry & Balance**: Layouts should be symmetrical where possible. The documentation layout uses equal-width sidebars to perfectly center the content.
- **Content-First**: UI elements are minimal. Text legibility is paramount.
- **Important Links**: All links (`<a>`) must be `text-white!` to stand out against the dark background.

## Color System
### Backgrounds
- **Base**: `bg-lithium-0` - Main background (Deep dark).
- **Surface**: `bg-lithium-50` - Cards, sidebars, inputs.
- **Highlight**: `bg-lithium-100` - Borders, hover states.

### Typography
- **Headings**: Inter/Sans-serif. Bold. White.
- **Body**: Inter/Sans-serif. `text-lithium-400`.
- **Links**: `text-white!` (Important). Hover: Underline with `decoration-lithium-500`.
- **Code**: JetBrains Mono.

## Layouts
### Documentation Layout
- **Structure**: 3-Column Layout (Flexbox).
  - **Left Sidebar**: Fixed width (`w-72`). Navigation.
  - **Center**: Flexible (`flex-1`). Content is centered within this area (`max-w-3xl`).
  - **Right Sidebar**: Fixed width (`w-72`). Table of Contents.
- **Responsiveness**: Sidebars collapse on smaller screens.

## Components
### Navigation
- **Sidebar Item**:
  - Default: `text-lithium-400 hover:text-white! hover:bg-lithium-50`.
  - Active: `bg-lithium-100 text-white! font-medium`.
  - Links: `text-white!` is enforced.

### Editor (UX)
- **Focus Mode**: When editing, sidebars should be hidden or minimized to reduce distraction.
- **Split View**: Clear separation between Markdown input and Live Preview.
- **Toolbar**: Actions (Save, Cancel) should be easily accessible but not obstructing.

## Spacing
- **Content Width**: `max-w-3xl` for optimal reading line length.
- **Vertical Rhythm**: Ample spacing between sections (`py-12`).
