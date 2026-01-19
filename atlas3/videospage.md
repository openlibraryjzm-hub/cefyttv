# Videos Page

## Description
The **Videos Page** (`VideosView.xaml`) is the granular browsing interface for viewing individual video items. It typically displays the contents of a selected playlist or a filtered collection (like "All Videos").

## Key Features
*   **Grid Layout**: Displays videos in a responsive grid using `VideoCard` components.
*   **Pagination**: content is sliced into pages of 50 items to ensure performance.
*   **Sticky Header**: A banner that scrolls away, leaving a persistent toolbar for actions.
*   **Sorting & Filtering**:
    *   **Sort**: Dropdown for Random, Chronological, and Progress-based sorting.
    *   **Folder Filter**: Color-coded tagging system for filtering content.
*   **Bulk Actions**: Support for selecting multiple videos for bulk tagging or organization.

## UI/UX & Styling
*   **Card Design**: `VideoCard` uses `CornerRadius="12"` and subtle drop shadows (`Effect={StaticResource ShadowDepth2}`).
*   **States**:
    *   **Hover**: Cards translate Y -4px to indicate interactivity.
    *   **Active**: A colored border (Sky-500) indicates the currently playing video.
*   **Reflow**: The UniformGrid (or WrapPanel) automatically adjusts items per row based on window width (min-width logic).

## Relevant Files
### Front-End
*   `Views/VideosView.xaml`: Main layout and grid logic.
*   `Controls/Cards/VideoCard.xaml`: Individual video item display template.
*   `Controls/Visuals/PageBanner.xaml`: Top banner component.

### Back-End
*   `ViewModels/MainViewModel.cs`: Handles sorting sort state and filter logic.
*   `Services/PlaylistService.cs`: Fetches video lists for specific playlists.
*   `Models/Entities/PlaylistItem.cs`: The data model for a video.

## Related Documentation
*   [Top Navigation Bar](topnavigationbar.md): Main way to navigate here from other tabs.
*   [Page Banners](pagebanners.md): Details on the scrolling header implementation.
*   [Import/Export](importexport.md): Details on the "Add" button and import modal functionality.
*   [Advanced Player Controller](advplayercontroller.md): Controls playback for videos selected here.
*   [Backend: Schema](backend_schema.md): Details the data structures (`playlist_items`) displayed here.
*   [Backend: State](backend_state.md): Defines how `MainViewModel` manages the video collection.
*   [Technical: Bridge](technical_bridge.md): Used when a video is clicked to "Quick Swap" playback without reloading the player.
