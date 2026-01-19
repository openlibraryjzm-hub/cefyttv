# Top Navigation Bar

## Description
The **Top Navigation Bar** (`TopNavigation.xaml`) is the persistent navigation strip located (typically) below the Player Controller or integrated into the shell. It allows for high-level switching between the application's major views.

## Key Features
*   **Tab System**: Explicit text tabs for "PLAYLISTS" and "VIDEOS".
*   **Icon Actions**:
    *   **History**: Clock Icon.
    *   **Likes**: Heart Icon.
    *   **Pins**: Pin Icon.
    *   **Settings**: Gear Icon.
    *   **Support**: Cat/Help Icon.
    *   **Browser**: Globe Icon.
*   **Design**: Uses a clean "Sky Blue" tint (`#E0F2FE`) with high-contrast, scalable vector icons (Path geometry). Use `Path` elements strictly to ensure proper `Foreground` color inheritance.

## UI/UX & Styling
*   **Interactivity**: Button states are critical.
    *   **Default**: Dark Gray/Slate Foreground.
    *   **Hover**: Light Blue background wash.
    *   **Selected**: Bold Sky Blue text/icon with an under-line or accent marker.
*   **Geometry**: Icons are sized at **32x32** for easy touch/click targets.
*   **Layout**: Uses a `Grid` to balance the Tabs (Center) against the Utility Icons (Right) and Back Navigation (Left).

## Relevant Files
### Front-End
*   `Controls/Navigation/TopNavigation.xaml`: The user control markup.
*   `MainWindow.xaml`: The parent shell where this control is hosted.

### Back-End
*   `Services/NavigationService.cs`: Handles the logic for switching views when buttons are clicked.
*   `ViewModels/MainViewModel.cs`: Updates `CurrentView` based on navigation events.

## Related Documentation
*   [Playlists Page](playlistspage.md): Corresponds to the "PLAYLISTS" tab.
*   [Videos Page](videospage.md): Corresponds to the "VIDEOS" tab.
*   [Settings Page](settingspage.md): Linked via the Gear icon.
*   [History Page](historypage.md): Linked via the Clock icon.
*   [Pins Page](pinspage.md): Linked via the Pin icon.
*   [Likes Page](likespage.md): Linked via the Heart icon.
*   [Backend: Routing](backend_routing.md): Explains how the View Switching logic actually works.
