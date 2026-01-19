# Startup Flow

## Description
The application initialization sequence is complex due to the need to coordinate three distinct engines (WPF, CEF, WebView2) and asynchronous backend services (SQLite, Config) without blocking the UI thread or launching the UI in a broken state.

## The Sequence (`App.xaml.cs`)

### 1. Service Initialization (Phase 0)
*   **Context**: `OnStartup` entry point.
*   **StartupUri**: Removed from `App.xaml` to prevent premature window launch.
*   **Action**: 
    1.  `ConfigService`: Loads user preferences (JSON).
    2.  `SqliteService`: Connects to DB, runs migrations.
    3.  `PlaylistService`, `FolderService`, `TabService`: Initialized with valid dependencies.
*   **Wait**: The application `await`s these tasks `OnStartup` before proceeding.

### 2. MainWindow Launch (Phase 1)
*   **Action**: `base.OnStartup(e)` allows the Application base to settle.
*   **Explicit Show**: `new MainWindow().Show()` is called manually.
*   **ViewModel**: `MainViewModel` is constructed. Since services are pre-warmed, it can safely access `App.ConfigService` immediately.

### 4. Auto-Play & Resume Logic (Phase 3)
*   Once the View is ready, `MainViewModel.LoadDataAsync` runs on a background thread:
    1.  **Check History**: Queries `watch_history` for the most recently watched video.
    2.  **Context Match**: Finds the playlist for that video.
    3.  **Resume**: Opens that playlist and cues the specific video.
    4.  **Fallback**: If no history exists, defaults to the first available Playlist/Video.

## Race Conditions (Resolved)
*   **Service vs View**: Previously, `StartupUri` launched the View before Services were ready, causing crashes. Moving to manual `Show()` after `await InitializeAsync()` fixed this.
*   **Threading**: UI-bound collections (`TabPresets`, `Playlists`) are strictly updated via `Dispatcher.Invoke` to prevent `CollectionView` threading errors.
