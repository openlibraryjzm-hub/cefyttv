# History Page

## Description
The **History Page** (`HistoryView.xaml`) provides a chronological log of all content watched by the user. It serves as a timeline for revisiting recent media.

## Key Features
*   **Data Source**: Queries the `watch_history` table in the SQLite database.
*   **Shared Layout**: Reuses the standard `VideosView` grid layout and styling (reusability).
*   **Access**: Accessible via the "Clock" icon in the Top Navigation or via Right-Click on the Video Grid button in the Player Controller.

## UI/UX & Styling
*   **Grid Consistency**: To reduce cognitive load, this page mirrors the `VideosView` exactly (same card size, same spacing).
*   **Visual Cue**: Unlike standard video lists, items here are strictly ordered by `LastPlayedDate` (Descending).
*   **Future Enhancement**: Grouping items by "Today", "Yesterday", etc., using `CollectionView` grouping headers.

## Relevant Files
### Front-End
*   `Views/HistoryView.xaml`: The view implementation.
*   `Controls/Cards/VideoCard.xaml`: Reuses the standard video card.

### Back-End
*   `Models/Entities/WatchHistory.cs`: Entity tracking playback events.
*   `Services/Database/AppDbContext.cs`: Database context for accessing the `watch_history` table.

## Related Documentation
*   [Advanced Player Controller](advplayercontroller.md): Right-clicking the Video Grid button navigates here.
*   [Videos Page](videospage.md): Shares the underlying grid layout and card design.
