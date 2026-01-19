# App Banner

## Description
The **App Banner** (specifically `UnifiedBannerBackground`) is a persistent visual layer that sits at the "Shell" level of the application, behind the visible UI.

## Key Features
*   **Continuity**: Unlike individual page headers, the App Banner remains constant during navigation, preventing visual jarring/flickering.
*   **Parallax/Scroll**: Implements an "Infinite Scroll" or Parallax effect to provide dynamic motion to the background.
*   **Source**: Loads a user-defined image (`assets/banner.PNG`) to theme the entire application.

## UI/UX & Styling
*   **Subtlety**: The banner is rarely seen raw; it is usually viewed through **Frosted Glass** (Blur) overlays in the HUD and Page Headers.
*   **Darkening**: The banner often has a dark overlay (Opacity 0.3-0.5) to ensuring text legibility on top of it, regardless of the user's chosen image brightness.
*   **Motion**: The scroll speed should be slow (e.g., 20-30s loop) to act as an ambient effect, not a distraction.

## Relevant Files
### Front-End
*   `Controls/Visuals/UnifiedBannerBackground.xaml`: The custom control implementing the scroll.
*   `MainWindow.xaml`: Hosts this control at the lowest Z-Index.

### Back-End
*   `Services/ConfigService.cs`: May handle default banner paths or user overrides.

## Related Documentation
*   [Page Banners](pagebanners.md): Transparent overlays that reveal this background.
*   [Advanced Player Controller](advplayercontroller.md): Floats above this background.
*   [UI: Layout](layout.md): Explains why this component is at Z-Index 0.
