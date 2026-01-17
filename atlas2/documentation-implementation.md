# Documentation & Implementation Map

This document bridges the gap between the legacy functionality (documented in `atlas/`) and the current C# implementation. It provides a detailed breakdown of where features lived, where they live now, and what is still missing.

## 🌟 Feature Mapping

### Playlist System
*   **Legacy**: [`atlas/playlist&tab.md`](../atlas/playlist&tab.md) - Defined CRUD, Folder hierarchies, and Tab grouping.
*   **Current C#**: 
    *   **Logic**: `Services/PlaylistService.cs` (Core CRUD operations).
    *   **UI**: `Views/PlaylistsView.xaml` (Grid display) and `Controls/Cards/PlaylistCard.xaml`.
*   **Missing**: "Tab/Workspace" filtering logic (currently all playlists show). "Stuck" folder logic is partially implemented but needs UI integration.

### Video Grid & Data
*   **Legacy**: [`atlas/ui.md`](../atlas/ui.md) - Documented card layout, hover effects, and skeleton loading.
*   **Current C#**:
    *   **Logic**: `Services/PlaylistService.cs` (Data fetching).
    *   **UI**: `Views/VideosView.xaml` (Grid) and `Controls/Cards/VideoCard.xaml`.
*   **Missing**: Complex skeleton loading states (currently basic).

### Folder Logic
*   **Legacy**: [`atlas/playlist&tab.md`](../atlas/playlist&tab.md) - 16-color system for organizing playlists.
*   **Current C#**:
    *   **Logic**: `Services/FolderService.cs`.
    *   **UI**: `Controls/Buttons/SidebarToggle.xaml` (Partial integration).
*   **Missing**: The "Sticky Carousel" for folders (pinned videos ignoring filters) is not yet ported.

### Player Controls (Top Orbit)
*   **Legacy**: [`atlas/advanced-player-controller.md`](../atlas/advanced-player-controller.md) - The "Orb" menu, Shuffle/Repeat toggles, and Dual Player logic.
*   **Current C#**:
    *   **Logic**: `MainViewModel.cs` (Navigation Commands `NextPlaylist`, `NextVideo`...).
    *   **UI**: `Controls/Player/AdvancedPlayerController.xaml` (Themed & Wired).
*   **Status**: **Mixed**. 
    *   ✅ Navigation Arrows (Prev/Next) fully functional (cycles playlist/video).
    *   ✅ Metadata Binding (Titles/Counts) fully functional.
    *   ❌ Play/Pause/Shuffle buttons visible but disconnected.
*   **Missing**: "Preview Mode" and specific "Dual Player" state management.

### Navigation
*   **Legacy**: [`atlas/navigation-routing.md`](../atlas/navigation-routing.md) - React Router paths and History stack.
*   **Current C#**:
    *   **Logic**: `Services/NavigationService.cs` (View switching manager).
    *   **Main VM**: `MainViewModel.cs` manages view state (`CurrentView`).
    *   **UI**: `Controls/TopNavigation.xaml` and `MainWindow.xaml` (Shell).
*   **Status**: **Functional**.
    *   ✅ Tab Switching (Playlists, Videos, Browser).
    *   ✅ **Browser Toggle** implemented.
*   **Missing**: Browser-like history (Forward/Back buttons) for app navigation is simplified in WPF.

### Settings & Configuration
*   **Legacy**: *(Scattered)* - User preferences, theme toggles, and path configs.
*   **Current C#**:
    *   **Logic**: `Services/ConfigService.cs` (Persists to local file/DB).
    *   **UI**: `Views/SettingsView.xaml`.
*   **Missing**: A comprehensive UI for all adjustable settings (currently skeleton).

### Pins System
*   **Legacy**: [`atlas/playlist&tab.md`](../atlas/playlist&tab.md) - Mechanics for "Pinning" videos to a dedicated view.
*   **Current C#**:
    *   **Logic**: `Services/PinService.cs`.
    *   **UI**: `Views/PinsView.xaml`.
*   **Missing**: "Sticky" logic for ensuring pinned items stay at top of other lists.

### YouTube API Integration
*   **Legacy**: [`atlas/importexport.md`](../atlas/importexport.md) - Logic for fetching metadata and playlist items.
*   **Current C#**:
    *   **Logic**: `Services/YoutubeApiService.cs`.
*   **Missing**: The "Unified Config Modal" UI that allows users to paste mixed links (Playlist vs Video) and have the system auto-detect.

### Watch History System
*   **Legacy**: [`atlas/history.md`](../atlas/history.md) - 5-second interval tracking and "Sticky" watched status (>85%).
*   **Current C#**:
    *   **UI**: `Views/HistoryView.xaml` (Skeleton list).
*   **Missing**: **Critical**. Background timer service to auto-update `watch_history` table during playback. Logic to calculate "85%" completion.

### Local Video Playback
*   **Legacy**: [`atlas/local-videos.md`](../atlas/local-videos.md) - MPV integration and file pickers.
*   **Current C#**:
    *   **Start**: `MpvNative.cs` (P/Invoke bindings).
    *   **UI**: `Controls/LocalVideoPlayer.xaml`.
*   **Missing**: Robust error handling for `mpv-2.dll` loading. Full playback control binding (Seek, Volume) from C# to MPV.

### YouTube Embed Player
*   **Legacy**: [`atlas/videoplayer.md`](../atlas/videoplayer.md) - IFrame API wrappers.
*   **Current C#**:
    *   **UI**: `Controls/Player/WebViewYouTubeControls.xaml`.
*   **Missing**: **Critical**. The "Bridge" logic. Executing JS in WebView2 to trigger Play/Pause/Seek on the YouTube player.

### Browser & AdBlock
*   **Legacy**: *Implied* - The "Browsing" capability.
*   **Current C#**:
    *   **UI**: `Views/BrowserView.xaml` (CefSharp implementation).
    *   **Logic**: `Handlers/CustomRequestHandler.cs` (AdBlock/Interception).
*   **Missing**: Tab management for the browser itself (if supported) and UI for URL bar/Navigation.

### Audio Visualizer
*   **Legacy**: [`atlas/audio-visualizer.md`](../atlas/audio-visualizer.md) - FFT Rendering.
*   **Current C#**: *None*
*   **Missing**: Entire feature. Needs a library like NAudio or Bass.NET to replicate.

### Import/Export
*   **Legacy**: [`atlas/importexport.md`](../atlas/importexport.md) - JSON export of library.
*   **Current C#**: *None*
*   **Missing**: Logic to serialize `Playlist` + `FolderAssignment` entities to JSON.
