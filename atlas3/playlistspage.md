# Playlists Page

## Description
The **Playlists Page** (`PlaylistsView.xaml`) is the high-level library view. It acts as the primary "Home" screen for the application, allowing users to browse their collection of YouTube playlists and local folders.

## Key Features
*   **Grid Layout**: Displays playlists using `PlaylistCard` components.
*   **Sticky Header**: Features the standard scrolling banner and a persistent "+ Add" button toolbar.
*   **Navigation**: Clicking a card navigates to the `VideosView` for that specific playlist.
*   **Pagination**: Handles large libraries by splitting content into 50-item pages.

## UI/UX & Styling
*   **Card Aesthetics**: `PlaylistCard` mirrors the `VideoCard` but with distinct visual markers (e.g., a "Stack" effect or Folder Icon) to denote it is a collection.
*   **Header**: The "Add" button in the Sticky Toolbar should use the `IconButtonStyle` with a Green or Primary Blue accent to invite action.
*   **Empty State**: If the library is empty, a centered "Get Started" prompt guides the user to the Import Modal.

## Relevant Files
### Front-End
*   `Views/PlaylistsView.xaml`: Main layout container.
*   `Controls/Cards/PlaylistCard.xaml`: Individual playlist item display.
*   `Controls/Visuals/PageBanner.xaml`: Top banner.

### Back-End
*   `Services/PlaylistService.cs`: Manages CRUD operations for playlists.
*   `Models/Entities/Playlist.cs`: Database entity for playlist metadata.

## Related Documentation
*   [Videos Page](videospage.md): The destination when clicking a playlist card.
*   [Top Navigation Bar](topnavigationbar.md): Primary access point ("PLAYLISTS" tab).
*   [Page Banners](pagebanners.md): Uses the standard banner architecture.
*   [Import/Export](importexport.md): The "+ Add" button triggers the Import functionality.
*   [Backend: Schema](backend_schema.md): Explains the `playlists` table structure.
*   [Backend: State](backend_state.md): Describes how the `Playlists` store collection is updated.
