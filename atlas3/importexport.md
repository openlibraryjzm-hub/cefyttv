# Import / Export & Configuration

## Description
The **Import/Export** system manages the ingestion of external content into the local database and the configuration of library organization.

## Key Features
*   **YouTube API Integration**:
    *   Allows users to migrate external YouTube Playlists and their Videos into the local `playlists.db` SQLite database.
    *   Handles metadata fetching (Titles, Thumbnails, Durations).
*   **Add Button Modal**:
    *   The primary interface for creating new Playlists or managing imports.
    *   **Context Awareness**:
        *   If opened from **Playlists Page**: Defaults to "Unsorted".
        *   If opened from **Videos Page**: Defaults to the current Playlist.
    *   **New Playlist Creation**: Features a "+" button to instantly create a new named playlist during the import flow.
    *   **UX**: Input fields auto-clear upon successful submission to prevent data residue.

## UI/UX & Styling
*   **Modal Design**: The import dialog should be a centered, floating card (`Panel.ZIndex="200"`) with a "Glass" background blackout to focus attention.
*   **Validation**: Input fields must show Red borders for invalid URLs.
*   **Feedback**: A loading spinner (or progress bar) is required during the YouTube API fetch phase, as this can take seconds.

## Relevant Files
### Front-End
*   `Views/Components/ImportModal.xaml` (or inline Popup): The UI for the Add/Import flow.
*   `Views/PlaylistsView.xaml`: Defines the "+ Add" button trigger.

### Back-End
*   `Services/YoutubeApiService.cs`: Handles API requests to YouTube.
*   `Services/PlaylistService.cs`: Writes new videos/playlists to the database.

## Related Documentation
*   [Playlists Page](playlistspage.md): Primary entry point for creating/importing playlists.
*   [Videos Page](videospage.md): Entry point for adding single videos to a playlist.
*   [Settings Page](settingspage.md): Database configuration ensures content is saved to the right place.
*   [Backend: Schema](backend_schema.md): Defines the database tables that this system populates.
