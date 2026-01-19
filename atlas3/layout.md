# Application Layout Strategy

## Overview
Project CCC uses a **"Shell" Architecture** where the main window acts as a persistent container for various layers (Media, UI, Navigation). This ensures that heavy components (like 3 different browser engines) remain stable while the user navigates lightweight XAML views.

## 1. The Shell Grid (`MainWindow.xaml`)
The root layout is a `Grid` with clearly defined Zones.

### Row Definitions
*   **Row 0 (Fixed 200px)**: **The HUD Zone**. Dedicated to the `AdvancedPlayerController`.
*   **Row 1 (Star `*`)**: **The Content Zone**. Contains the primary viewport for Players, Browsers, and Libraries.

### Z-Index Layering
To manage the "Triple Engine" composition, we use strict Z-Index rules:
1.  **Bottom (Index 0)**: `UnifiedBannerBackground` (Parallax Image).
2.  **Low (Index 10)**: `WebViewYouTubeControls` / `WinFormsHost(MPV)`.
3.  **Medium (Index 50)**: `Grid` (The "Library" View: Playlists/Videos).
4.  **High (Index 90)**: `BrowserView` (CefSharp Overlay).
5.  **Top (Index 100)**: `AdvancedPlayerController` (Floating HUD).

## 2. Split Screen vs. Full Screen logic
The application supports dynamic resizing of the "Player" vs "Library" areas.

### Library Mode (Default)
*   **Left Column**: Media Player (WebView2/MPV).
*   **Right Column**: Navigation/Library content.
*   **Split**: Defined by `Grid.ColumnDefinitions` in the Content Row.

### Full Screen Mode
*   **Logic**: The "Library" UI (Right Column) is hidden (`Visibility=Collapsed`).
*   **Behavior**: The Player Column `ColumnSpan` is set to cover the entire width.
*   **HUD**: The `AdvancedPlayerController` remains floating on top (Row 0), allowing control even in full screen.

## 3. Component Deployment

### The Top Navigation Bar
*   **Placement**: Located at the **Top of the Library View** (Right Column), *below* the HUD space.
*   **Dimensions**: Height `~50px`.
*   **Context**: It is part of the "Page", not the global Shell, meaning it hides when the Library is hidden.

### The App Banner
*   **Area**: Covers the entire Window (`RowSpan="2"`, `ColumnSpan="2"`).
*   **Visibility**: Visible through transparent "Glass" panels in the HUD and Page Headers.

## 4. Responsive Logic
*   **Containers**: Main Content containers typically restrict max-width (e.g., `900px`) to prevent "Ultra-Wide" stretching on large monitors, ensuring cards remain scannable.

## Relevant Files
### Front-End
*   `MainWindow.xaml`: The definitive source of the Grid, RowDefinitions, and Z-Index stacking.

## Related Documentation
*   [App Banner](appbanner.md): The Index 0 background layer.
*   [Advanced Player Controller](advplayercontroller.md): The Index 100 top layer.
*   [Backend: Engines](backend_engines.md): The heavy components (WebView2/MPV) being managed in the layout.
*   [Backend: Routing](backend_routing.md): How view switching occurs within the "Type 3" (Index 50) layer.
