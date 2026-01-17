# Advanced Player Controller (C# Implementation)

**Last Updated:** 2026-01-18
**Status:** UI Wired (Navigation Logic) / Visuals Updated
**Parent Document:** [Architecture](architecture.md)

---

## 1. Component Overview
The **Advanced Player Controller** is the application's primary "Head-Up Display". It sits fixed at the top of the interface, providing global access to media controls, visualization, and navigation.

**Key Definition:**
*   **File:** `Controls/Player/AdvancedPlayerController.xaml`
*   **Height:** 102px (Reserved Space) - *Note: The Orb spills over this height.*
*   **Layout Strategy:** 3-Column Grid with visual overlap.

## 2. Visual Architecture

### 2.1 The "Spill" Layout
Unlike standard WPF layouts that clip content, this controller relies on a specific Z-Indexing strategy to allow the central "Orb" to protrude into the main content area (Row 1).

**Implementation Details:**
```xml
<!-- MainWindow.xaml -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="102"/> <!-- Dedicated Top Row -->
        <RowDefinition Height="*"/>   <!-- Content Row -->
    </Grid.RowDefinitions>

    <!-- ZIndex=100 ensures Orb draws ABOVE the content below -->
    <player:AdvancedPlayerController Grid.Row="0" Panel.ZIndex="100"/>
</Grid>
```

```xml
<!-- AdvancedPlayerController.xaml -->
<!-- ClipToBounds="False" is CRITICAL to allow the 154px Orb to fit in a 102px Row -->
<Grid Height="102" VerticalAlignment="Top" ClipToBounds="False">
```

### 2.2 The 3-Part Grid Structure
The control itself uses a simple 3-column grid:

| Column | Width | Purpose |
| :--- | :--- | :--- |
| **0 (Left)** | `*` | **Playlist Menu**. Aligned `Right`. |
| **1 (Center)** | `Auto` | **The Orb**. 154px Fixed Width. |
| **2 (Right)** | `*` | **Video Menu**. Aligned `Left`. |

---

## 3. Control Inventory & Wiring

### 3.1 Playlist Menu (Left)
*   **Metadata Area**: Displays `SelectedPlaylist.Name` and `SelectedPlaylist.VideoCountText`.
*   **Navigation Bar**:
    *   `<` (Previous Playlist): **Wired**. Cycles to previous playlist and auto-plays video.
    *   `#` (Playlist Grid): **Wired**. Navigates to `PlaylistsView`.
    *   `>` (Next Playlist): **Wired**. Cycles to next playlist and auto-plays video.

### 3.2 The Orb (Center)
*   **Central Image**: currently a dark circle placeholder.
*   **Radial Buttons**: 4 buttons positioned absolute/margin based:
    *   `^` (Top): Upload Image
    *   `M` (Right): Menu Toggle
    *   `?` (Left): Search/Help
    *   `O` (Bottom): Clipping/Spill Toggle

### 3.3 Video Menu (Right)
*   **Metadata Area**: Displays `SelectedVideo.Title`.
*   **Navigation Bar** (Left Cluster):
    *   `<` (Prev Video): **Wired**. Cycles to previous video in list.
    *   `#` (Video Grid): **Wired**. Navigates to `VideosView`.
    *   `>` (Next Video): **Wired**. Cycles to next video in list.
    *   `▶` (Play/Folder Cycle): *Pending Wiring*.
*   **Tool Bar** (Right Cluster):
    *   `S` (Shuffle), `*` (Star), `P` (Pin), `L` (Like), `...` (More).

---

## 4. Styling & Resources

### 4.1 Theme Integration (Updated 2026-01-18)
The controller now uses the GLOBAL resource dictionary (`Colors.xaml`, `Styles.xaml`) for a cohesive **Dark Glass** look.

*   **Backgrounds**: Uses `GlassCardBrush` (#CC1e293b) for semi-transparent panels.
*   **Text**: Uses `TextPrimaryBrush` (Slate-50) and `TextMutedBrush` (Slate-500).
*   **Accents**: Uses `AccentPrimaryBrush` (Red-500) for highlights.

### 4.2 Local Resources
*   **`IconButtonStyle`**: 
    *   Transparent background by default.
    *   Hover: `BgTertiaryBrush` (Slate-700) background, `AccentPrimaryBrush` text.
*   **`OrbButtonStyle`**: 
    *   White background, Dark text.
    *   Hover: Scale 1.1x zoom effect.

---

## 5. Next Steps (Implementation Roadmap)

1.  **Player Bridge Integration**: Wiring the Play/Pause logic to `WebView2`.
2.  **Icon Integration**: Replace text placeholders (`<`, `S`) with `Path` vectors or an Icon Font (FontAwesome/Material).
3.  **Audio Visualizer**: Implement the canvas/rendering logic for the ring around the orb.
