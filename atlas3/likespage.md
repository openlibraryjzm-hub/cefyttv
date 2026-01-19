# Likes Page

## Description
The **Likes Page** (`LikesView.xaml`) is a curated collection of videos that the user has marked as favorites using the "Heart" action.

## Key Features
*   **Mechanism**: Videos are added to this view when the "Like" (Heart) button is toggled in the Advanced Player Controller.
*   **Layout**: Displays videos in a standard 3-column grid (50 videos per page), mirroring the main Videos Page.
*   **Interaction**: Clicking a video triggers playback in the main player context.

## UI/UX & Styling
*   **Identifiable**: The page uses the standard `PageBanner` with the title "LIKES" to distinguish it contextually.
*   **Empty State**: Should display a message ("No Liked Videos yet") if the list is empty to prevent a broken appearance.
*   **Card State**: Videos listed here should implicitly show the Heart icon as "Active" (Filled) if displayed on the card itself.

## Relevant Files
### Front-End
*   `Views/LikesView.xaml`: The dedicated view for liked content.
*   `Controls/Player/AdvancedPlayerController.xaml`: Source of the "Like" action.

### Back-End
*   `ViewModels/MainViewModel.cs`: Likely coordinates the state of "Liked" videos.

## Related Documentation
*   [Advanced Player Controller](advplayercontroller.md): Contains the Heart/Like button that populates this page.
*   [Videos Page](videospage.md): The layout template used for this page.
