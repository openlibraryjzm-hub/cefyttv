# Page Banners

## Description
**Page Banners** (`PageBanner.xaml`) are the scrolling header components found at the top of every content page (Playlists, Videos, etc.).

## Key Features
*   **Scrolling Behavior**: The banner starts at full height but scrolls *out of view* as the user moves down the page, maximizing screen real estate for content.
*   **Transparency**: Crucially, the Page Banner has a **Transparent Background**. It relies on the underlying `App Banner` (Z-Index 0) to provide the visual texture. This creates the illusion that the header is part of the global theme.
*   **Context**: Displays the Page Title (e.g., "PLAYLISTS", "HISTORY") and sometimes breadcrumbs.

## UI/UX & Styling
*   **Typography**: Uses large, bold, uppercase text (`FontSize="32"`, `FontWeight="Bold"`) to clearly announce context.
*   **Corner Radius**: The bottom corners are typically square, but the top corners (if visible against a margin) match the card radius (12px).
*   **Integration**: Visually, it should feel like a "Window" looking through to the moving background behind the app.

## Relevant Files
### Front-End
*   `Controls/Visuals/PageBanner.xaml`: The header component.
*   `Controls/Visuals/UnifiedBannerBackground.xaml`: The visual layer seen *through* this component.

## Related Documentation
*   [App Banner](appbanner.md): The background image provided by this system.
*   [Videos Page](videospage.md): Consumes this component.
*   [Playlists Page](playlistspage.md): Consumes this component.
