# Advanced Player Controller (C# Implementation)

**Last Updated:** 2026-01-18
**Status:** Wired (Full Visuals & Navigation) / Interactive
**Parent Document:** [Architecture](architecture.md)

---

## 1. Component Overview
The **Advanced Player Controller** is the application's primary "Head-Up Display". It constitutes the top 200px of the interface, providing a spacious and modern canvas for media controls, visualization, and navigation.

**Key Definition:**
*   **File:** `Controls/Player/AdvancedPlayerController.xaml`
*   **Height:** 200px (Allocated Banner Space)
*   **Design Style:** "Floating Wings" & "Orbital Navigation"
*   **Layout Strategy:** 3-Column Grid with vertically centered elements.

## 2. Visual Architecture

### 2.1 The "Floating Canvas" Layout
The controller occupies a significantly taller space (200px) than traditional title bars. The interface elements (Orb, Menus) are vertically centered within this space, creating a sense of floating above the banner image.

**Implementation Details:**
```xml
<!-- MainWindow.xaml -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="200"/> <!-- Expanded Top Row -->
        <RowDefinition Height="*"/>   <!-- Content Row -->
    </Grid.RowDefinitions>

    <!-- ZIndex=100 ensures Orb draws ABOVE the content below -->
    <player:AdvancedPlayerController Grid.Row="0" Panel.ZIndex="100"/>
</Grid>
```

```xml
<!-- AdvancedPlayerController.xaml -->
<!-- 200px Height provides the canvas for centering -->
<Grid Height="200" VerticalAlignment="Top" ClipToBounds="False">
```

### 2.2 The 3-Part Grid Structure
The control uses a 3-column grid to organize its primary sections:

| Column | Width | Element | Dimensions | Alignment |
| :--- | :--- | :--- | :--- | :--- |
| **0 (Left)** | `*` | **Playlist Menu** | 340x98px | Right aligned, Vertically Centered |
| **1 (Center)** | `Auto` | **The Orb** | 154x154px | Fixed, Vertically Centered |
| **2 (Right)** | `*` | **Video Menu** | 340x98px | Left aligned, Vertically Centered |

This 98px menu height against the 200px container creates significant "breathing room" (approx 50px) above and below each menu, enhancing the floating aesthetic.

---

## 3. Control Inventory & Wiring

### 3.1 Playlist Menu (Left Wing)
*   **Metadata Area**: Displays `SelectedPlaylist.Name` and video count.
*   **Orbital Navigation**:
    *   **Layout**: A central circular "Grid" button flanked by two floating chevron arrows.
    *   `<` (Chevron): **Prev Playlist**. Cycle to previous playlist.
    *   `#` (Circle): **Playlist Grid** (Navigates to `PlaylistsView`).
    *   `>` (Chevron): **Next Playlist**. Cycle to next playlist.

### 3.2 The Orb (Center)
*   **Central Image**: currently a dark circle placeholder (150px).
*   **Radial Buttons**: 4 buttons positioned absolute/margin based:
    *   **Top**: Upload Image
    *   **Right**: Menu Toggle
    *   **Left**: Search/Help
    *   **Bottom**: Clipping/Spill Toggle

### 3.3 Video Menu (Right Wing)
*   **Metadata Area**: Displays `SelectedVideo.Title`.
*   **Orbital Navigation** (Left Side):
    *   `<` (Chevron): **Prev Video**.
    *   `#` (Circle): **Video Grid** (Navigates to `VideosView`).
    *   `>` (Chevron): **Next Video**.
*   **Playback Control** (Independent):
    *   `▶` (Play): Situated to the right of the nav cluster.
*   **Tool Bar** (Right Side):
    *   **Shuffle** (Randomize)
    *   **Star** (Favorite)
    *   **Pin** (Sticky)
    *   **Like** (Heart)
    *   **...** (More Actions)

---

## 4. Styling & Resources

### 4.1 Button Styles
All buttons now use **Vector Paths** (SVG Data) instead of font glyphs for perfectly sharp rendering.

*   **`IconButtonStyle`**: 
    *   **Shape**: 32x32 Circle (`CornerRadius="16"`).
    *   **Appearance**: Glassy Background (`#1AFFFFFF`) with a Solid Slate Border (`#FF475569`, 1.5px).
    *   **Hover**: Border and Icon turn Sky Blue (`#38BDF8`). Background brightens.
    *   **Usage**: Primary tools (Grid, Shuffle, Star, Pin, Play).

*   **`ChevronButtonStyle`**: 
    *   **Shape**: 24x32 Rectangular hit target, minimal.
    *   **Appearance**: Transparent background, Borderless.
    *   **Content**: Small 10px Chevron Path.
    *   **Hover**: Path turns Sky Blue (`#38BDF8`).
    *   **Usage**: Flanking navigation arrows (Prev/Next).

*   **`OrbButtonStyle`**: 
    *   **Shape**: 28x28 Circle.
    *   **Appearance**: White background, Dark text/icon.
    *   **Hover**: Scale transform (Zoom).

### 4.2 Theme Integration
*   The controller adheres to the cohesive **Dark Glass** look, using `GlassCardBrush` for the menu panels.

---

## 5. Technical Implementation (Window Chrome)

### 5.1 Hit Testing Strategy
To prevent the window drag behavior from interfering with these larger buttons:
*   `WindowChrome.IsHitTestVisibleInChrome="True"` is applied to the **Bottom Bar** of each menu and the **Orb Container**.
*   This ensures clicks on buttons register immediately, while the empty glass space around them still allows for dragging the window.

---

## 6. Next Steps (Implementation Roadmap)

1.  **Player Bridge Integration**: Wiring the Play/Pause logic to `WebView2`.
2.  **Audio Visualizer**: Implement the canvas/rendering logic for the ring around the orb.
3.  **Real Data Binding**: Ensure all "Author" and "Count" metadata fields are tied to live ViewModel data.
