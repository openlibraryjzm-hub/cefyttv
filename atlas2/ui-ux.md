# UI & UX Documentation

## Core Philosophy
The interface is designed around a **Simultaneous Consumption** model:
1.  **The Stage (Left 50%)**: Dedicated always-on media playback.
2.  **The Library (Right 50%)**: A rich, navigable application for managing content without interrupting playback.

## Global Layout architecture
The `MainWindow.xaml` defines a strict **Two-Column Grid**:

### 1. Left Pane: "The Stage" (Column 0)
*   **Purpose**: Media Playback.
*   **Engine Handling**: Acts as a container for the Triple-Engine system.
    *   **Default**: `WebViewYouTubeControls` (WebView2) for streaming content.
    *   **Local**: `LocalVideoPlayer` (MPV) for file playback (swapped dynamically).
*   **Behavior**:
    *   Persists across all navigation in the Right Pane.
    *   Responds to "Play" commands triggered from the Library.

### 2. Right Pane: "The Library" (Column 1)
*   **Purpose**: Content Discovery & Management.
*   **Structure**: Hosted within a specific `Grid` shell.
    *   **Header (Row 0)**: `TopNavBar`.
    *   **Body (Row 1)**: `ContentControl` managed by `MainViewModel`.

---

## Component Systems

### Navigation (TopNavBar)
Ideally located at the top of the Right Pane, this replaces the traditional Sidebar to maximize horizontal space for content cards.
*   **Tabs**: Text-based tabs (Playlists, Videos) for primary views, Icon-based tabs (History, Likes, Settings) for utilities.
*   **Context**: Changes active state based on the `CurrentView` bound in the ViewModel.

### Content Views (MVVM)
The application uses a standard MVVM pattern to swap views inside the Right Pane's Body.
*   **Playlists View**: A grid of `PlaylistCard` elements.
    *   *Interaction*: Clicking a card navigates to the **Videos View** and loads that playlist's context.
*   **Videos View**: A list/grid of `VideoCard` elements.
    *   *Interaction*: Clicking a card triggers the global `PlayVideoCommand`, updating the **Left Pane**.

### Visual Language
*   **Theme**: Deep Dark Mode (`#121212` background).
*   **Typography**: Clean, sans-serif (Segoe UI / Inter).
*   **Banners**: `PageBanner` component provides context (Title, Video Count) with visual flair (ASCII art, gradients).

---

## Interaction Flow
1.  **Browse**: User scrolls through Playlists in the Right Pane.
2.  **Select**: User clicks a Playlist -> Right Pane transitions to Videos list.
3.  **Consume**: User clicks a Video -> Right Pane *stays* on list, Left Pane *immediately* loads and plays video.
4.  **Multitask**: User can navigate to "Settings" or "History" in the Right Pane while the Left Pane continues playback uninterrupted.
