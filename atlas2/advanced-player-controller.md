# Advanced Player Controller (C# Implementation)

**Last Updated:** 2026-01-17
**Status:** UI Skeleton Implemented / Logic Pending
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

## 3. Control Inventory (Buttons)

We have implemented visual placeholders for all controls defined in the React/Tauri spec.

### 3.1 Playlist Menu (Left)
*   **Metadata Area**: Displays `Author | Views` (Placeholder).
*   **Navigation Bar**:
    *   `<` (Previous Playlist)
    *   `#` (Playlist Grid)
    *   `>` (Next Playlist)

### 3.2 The Orb (Center)
*   **Central Image**: currently a dark circle placeholder.
*   **Radial Buttons**: 4 buttons positioned absolute/margin based:
    *   `^` (Top): Upload Image
    *   `M` (Right): Menu Toggle
    *   `?` (Left): Search/Help
    *   `O` (Bottom): Clipping/Spill Toggle

### 3.3 Video Menu (Right)
*   **Metadata Area**: Title Text.
*   **Navigation Bar** (Left Cluster):
    *   `<` (Prev Video)
    *   `#` (Video Grid)
    *   `>` (Next Video)
    *   `▶` (Play/Folder Cycle)
*   **Tool Bar** (Right Cluster):
    *   `S` (Shuffle)
    *   `*` (Star/Folder Assign)
    *   `P` (Pin Priority)
    *   `L` (Like)
    *   `...` (More Options)

---

## 4. Styling & Resources

### 4.1 Local Resources
To avoid "Forward Reference" crashes in XAML, styles are defined in `<UserControl.Resources>` at the very top of the file.

*   **`IconButtonStyle`**: 
    *   Transparent background by default.
    *   Hover: Deep Blue (`#1E293B`) background, Sky Blue (`#38BDF8`) text.
    *   Corner Radius: 12.
*   **`OrbButtonStyle`**: 
    *   White background, Dark text.
    *   Hover: Scale 1.1x zoom effect.
    *   Corner Radius: 14 (Circular).

---

## 5. Next Steps (Implementation Roadmap)

1.  **Icon Integration**: Replace text placeholders (`<`, `S`, assuming basic chars) with `Path` vectors or an Icon Font (FontAwesome/Material).
2.  **Data Binding**:
    *   Bind `VideoTitle`, `Author`, `ViewCount` to `MainViewModel`.
    *   Bind `ImageSource` for the Orb.
3.  **Command Wiring**:
    *   Connect `Command="{Binding NextVideoCommand}"`, etc.
4.  **Audio Visualizer**: Implement the canvas/rendering logic for the ring around the orb.
