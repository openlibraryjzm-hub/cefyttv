# UI Design System

**Last Updated:** 2026-01-18
**Status:** Planned / Partial Implementation
**Parent:** [README.md](README.md)

---

## 1. Design Philosophy

The original React/Tauri app used a "Premium/Glassmorphic" aesthetic with heavy usage of Tailwind CSS (`slate-900`, `sky-500`, `backdrop-blur`). 
In WPF, we aim to replicate this look using **ResourceDictionaries** and **ControlTemplates** rather than CSS classes.

### 1.1 Core Principles
*   **Vector-First**: Use SVGs (Path Geometry) over Bitmaps.
*   **Deep Dark Mode**: Primary background is Slate-900/950 equivalent (`#0F172A`).
*   **Vibrant Accents**: Primary action color is Sky-400/500 (`#38BDF8`/`#0EA5E9`).
*   **Roundness**: Heavy use of `CornerRadius="12"` or `16` for cards and containers.

---

## 2. Color System

Defined in `Resources/Colors.xaml`.

### 2.1 Surfaces (Backgrounds)
| Name | Hex | Usage |
| :--- | :--- | :--- |
| `BgPrimaryBrush` | `#0F172A` | **Main Window Background** (Dark Slate). |
| `BgSecondaryBrush` | `#1E293B` | **Cards / Sidebars** (Lighter Slate). |
| `BgTertiaryBrush` | `#334155` | **Inputs / Borders**. |

### 2.2 Typography (Foregrounds)
| Name | Hex | Usage |
| :--- | :--- | :--- |
| `TextPrimaryBrush` | `#E2E8F0` | **Headings / Titles** (High Contrast). |
| `TextSecondaryBrush` | `#94A3B8` | **Metadata / Descriptions**. |
| `TextAccentBrush` | `#38BDF8` | **Links / Active States**. |

### 2.3 Functional Palette (Folder Colors)
Full spectrum of Tailwind colors (Red, Orange, Amber... Pink) is defined as `Folder{Name}Brush`.

---

## 3. Typography

Fonts are currently relying on System Default (`Segoe UI`).
**Plan**: Import `Inter` or `Outfit` via Embedded Resource.

*   **Headings**: Bold/Black weight.
*   **Body**: Regular/Medium.

---

## 4. Components & Styles

Defined in `Resources/Styles.xaml`.

### 4.1 Buttons
*   **`IconButtonStyle`**: 
    *   No visible border.
    *   Foreground: `TextSecondary`.
    *   Hover: Background `BgSecondary`, Foreground `TextAccent`.
    *   Size: typically 32x32 or 40x40.

### 4.2 Cards
*   **`VideoCardStyle`** (Template):
    *   CornerRadius: 12.
    *   Shadow: `DropShadowEffect` (Blur 10, Opacity 0.2).
    *   Hover: Translate Y -2px, Glow Effect.

### 4.3 Inputs
*   **`SearchBoxStyle`**:
    *   Background: `BgSecondary`.
    *   Border: `BgTertiary`.
    *   CornerRadius: 8.
    *   Padding: 8,4.

---

## 5. Layout Strategy

### 5.1 The "Shell" Grid
*   **Row 0 (102px)**: Advanced Player Controller (Fixed).
*   **Row 1 (Star)**: Content Area.
    *   **Col 0 (Star)**: Video Player.
    *   **Col 1 (Star)**: Library/Browser.

### 5.2 Responsive logic
*   Currently functional for `1920x1080`+.
*   Future: Use `GridSplitter` to allow resizing the Player/Library ratio.

### 5.3 Window Chrome
*   **Style**: `WindowStyle="SingleBorderWindow"` with `GlassFrameThickness="0"`.
*   **Behavior**: Custom chrome handles Maximize correctly (respects Taskbar).
*   **Hit-Testing**: Specific interactive zones (Buttons) use `IsHitTestVisibleInChrome="True"` to override drag behavior.

### 5.4 Sticky Feed Layout (Videos/Playlists)
*   **Pattern**: Single `ScrollViewer` containing a `StackPanel`.
*   **Banner**: Scrolls away with content. `CornerRadius="12,12,0,0"`.
*   **Toolbar**: Uses `ScrollChanged` event to adjust `Margin.Top`, creating a "Stick to Banner then Stick to Top" effect.
*   **Dimensions**: 
    *   **Container**: `Width="900"`, centered.
    *   **Banner/Toolbar**: `Width="884"` (flush with card visual edges).

---

## 6. Controls & Widgets

### 6.1 Pagination
*   **Strategy**: Client-side slicing of cached full datasets.
*   **Page Size**: 50 Items.
*   **Components**: 
    *   Footer Bar in `PlaylistsView` / `VideosView`.
    *   Controls: Prev (`<`), Next (`>`), Status Text (`Page X of Y`).

