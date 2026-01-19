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
| **0 (Left)** | `*` | **Playlist Menu** | 340x102px | Right aligned, Vertically Centered |
| **1 (Center)** | `Auto` | **The Orb** | 154x154px | Fixed, Vertically Centered |
| **2 (Right)** | `*` | **Video Menu** | 340x102px | Left aligned, Vertically Centered |

This 102px menu height against the 200px container creates significant "breathing room" (approx 49px) above and below each menu, enhancing the floating aesthetic.

### 2.3 Banner Background
To provide a rich visual anchor, the controller now incorporates a scrolling background layer:
*   **Component**: `UnifiedBannerBackground`.
*   **Source**: `/assets/banner.PNG` (User Custom Banner).
*   **Behavior**: Sits behind the Glass Menus and Orb, providing visual texture and continuity with the Page Banners.

---

## 3. Control Inventory & Wiring

### 3.1 Playlist Menu (Left Wing)
*   **Metadata Area**: Displays `SelectedPlaylist.Name` and video count.
*   **Priority Pin**: Top-Right corner element (`52x39px`) displaying the active thumbnail with a `#334155` border and `#38BDF8` active ring.
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
*   **Tool Bar** (Right Side):
    *   **Shuffle** (Randomize)
    *   **Star** (Favorite)
    *   **Pin** (Sticky) -> **Right-Click**: Navigate to Pins Page.
    *   **Like** (Heart) -> **Right-Click**: Navigate to Likes Page.
    *   **...** (More Actions)
    *   **Video Grid Button (#)** -> **Right-Click**: Navigate to History Page.

---

## 4. Styling & Resources (Sky Theme)

The controller adopts the "Sky Glass" aesthetic from the legacy Rust-Tauri project, utilizing specific light-mode colors with high blur values.

### 4.1 Color Palette & Materials
*   **Menu Backgrounds**: `#E0F2FE` (Sky 100) at **95% Opacity** with high Blur.
*   **Control Bar Background**: `#F0F9FF` (Sky 50).
*   **Orb Background**: `#F0F9FF` (Sky 50) with `backdrop-blur-3xl`.
*   **Text (Primary)**: `#082F49` (Sky 950) - **Black/Extra Bold**.
*   **Text (Metadata)**: `#0EA5E9` (Sky 500) - **Bold/Uppercase**.
*   **Shadows**: `0 25px 50px -12px rgba(0,0,0,0.25)` (Shadow-2XL).

### 4.2 Button Styles
Buttons primarily use a **White Circle** geometry. All icons use Vector Paths.

*   **`IconButtonStyle`**:
    *   **Shape**: `32px` Circle (`rounded-full`).
    *   **Fill**: `#FFFFFF` (White).
    *   **Border**: `2px` Solid `#334155` (Slate 700).
    *   **Shadow**: `shadow-sm`.
    *   **Hover**: Border matches Folder Color or `#475569`.
    *   **Usage**: Primary tools (Grid, Shuffle, Star, Pin, Play).

*   **`ChevronButtonStyle`**:
    *   **Shape**: `24x32` Rectangular hit target.
    *   **Appearance**: Transparent background, Borderless (Text Only).
    *   **Icon Color**: `#38BDF8` (Sky 400).
    *   **Usage**: Flanking navigation arrows (Prev/Next).

*   **`OrbButtonStyle`**:
    *   **Shape**: `28px` Circle.
    *   **Fill**: `#FFFFFF` (White).
    *   **Border**: `2px` Solid `#F0F9FF` (Sky 50).
    *   **Shadow**: `shadow-xl`.

### 4.3 Typography
*   **Titles**: `Segoe UI` (System), Weight **Black** (900), Size `~18px`.
*   **Metadata**: Size `10-11px`, Weight **Bold**, Tracking **Widest**.
*   **Folder Badges**: `9px`, Black, Uppercase, Tracking `0.15em`.

### 4.4 Theme Integration
*   **Contrast**: The Light Sky menu bodies provide high contrast against the typically dark video backgrounds/banners.
*   **Borders**: Critical for separation. Menus use `4px` Solid borders (Default: `#bae6fd` Sky 200).

---

## 5. Technical Implementation (Window Chrome)

### 5.1 Hit Testing Strategy
To prevent the window drag behavior from interfering with these larger buttons:
*   `WindowChrome.IsHitTestVisibleInChrome="True"` is applied to the **Bottom Bar** of each menu and the **Orb Container**.
*   This ensures clicks on buttons register immediately, while the empty glass space around them still allows for dragging the window.

---

## 6. Functional Behaviors

### 6.1 Contextual Navigation Sync
*   **Problem**: Playing a video from a non-playlist source (History, Likes, Pins) traditionally breaks the "Next/Prev" flow because the player doesn't know the context.
*   **Solution**: The controller now performs an automatic **Playlist Reverse-Lookup**. 
    *   When a video loads, if it belongs to a playlist, the **Left Menu** and **Right Menu** automatically snap to that playlist's context.
    *   This ensures `Next` and `Prev` buttons correctly cycle through the video's original playlist, even if you started playback from the History page.

### 6.2 View Navigation Shortcuts
*   Several buttons offer secondary navigation actions via **Right-Click**:
    *   **Video Grid Button**: Go to History.
    *   **Pin Button**: Go to Pins.
    *   **Like Button**: Go to Likes.
*   **Fullscreen Handling**: If triggered while in Fullscreen, the app automatically exits to Split View to display the requested page.

---

## 7. Next Steps (Implementation Roadmap)

1.  **Player Bridge Integration**: Wiring the Play/Pause logic to `WebView2`.
2.  **Audio Visualizer**: Implement the canvas/rendering logic for the ring around the orb.
3.  **Real Data Binding**: Ensure all "Author" and "Count" metadata fields are tied to live ViewModel data.

## 8. Customization Features

### 8.1 Orb Image Customization
To match the legacy app's flexibility, the Orb now supports deep customization via the **Settings Page**:

*   **Custom Image**: Users can upload any image (`.png`, `.jpg`, etc.) via the Top Radial Button on the Orb.
*   **Scale & Position**:
    *   **Scale**: Slider (0.5x to 2.0x) to resize the central image.
    *   **Offset**: X/Y sliders (-100px to +100px) to fine-tune alignment.
*   **Spillover Effects**:
    *   Users can "break" the circular boundary of the Orb by enabling **Spillover Quadrants**.
    *   **Mechanism**: A custom `OrbSpillMaskConverter` uses a **Normalized Coordinate System (0-1)**. 
        *   **Master Toggle**: Explicit `IsSpillEnabled` property controls whether spill logic is active.
        *   **Spill Mode**: When enabled, the mask includes quadrants that extend infinitely beyond the 0-1 bounds (e.g. coordinates `-10` to `11`), allowing the image to spill out of the `154px` container (which has `ClipToBounds=False`).
        *   **Object Fit**: When Spill is **OFF**, the Image uses `Stretch="UniformToFill"` (Cover) to perfectly fill the circle. When Spill is **ON**, it switches to `Stretch="Uniform"` (Contain), preventing zoom issues and allowing manual scaling.
    *   **Z-Index**: The Orb grid utilizes natural Z-indexing (Image first, Buttons second) to ensure buttons overlay the spilled image, while the entire controller relies on WindowChrome settings to handle hit-testing.

