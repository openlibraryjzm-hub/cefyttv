# MVVM Store Architecture & State Management

**Last Updated:** 2026-01-18
**Status:** Implemented (Phase A)
**Parent Document:** [Architecture](architecture.md)

---

## 1. The Strategy: Replacing Zustand

In the original React app, state was dispersed across `Zustand` stores (`playlistStore`, `videoStore`, `uiStore`). 
In the C# WPF rewrite, we consolidate this into a centralized **MVVM Architecture** using the **CommunityToolkit.Mvvm** library.

### 1.1 The "Global Store" Concept
To simplify state sharing (a core requirement of the original app), the `MainViewModel` acts as a Singleton "Store" for the entire application. While strictly "Pure MVVM" suggests distinct ViewModels per View, our requirement for global player state (visible everywhere) necessitates this centralized approach.

---

## 2. Core Components

### 2.1 `MainViewModel` (The Single Source of Truth)
Located in `ViewModels/MainViewModel.cs`, this class holds:
1.  **Navigation State**: `_currentView` (The active UserControl).
2.  **Player State**: `_currentVideoId`, `_isPlaying` logic.
3.  **Collections**: `Playlists`, `Videos`, `HistoryItems` (ObservableCollections that drive Lists).
4.  **UI Flags**: `_isBrowserVisible`, `_pageTitle`.

It inherits from `ObservableObject` and uses `[ObservableProperty]` to auto-generate `INotifyPropertyChanged` boilerplate.

### 2.2 Display Models (DTOs)
Because Entity Framework entities (`Playlist`, `PlaylistItem`) are data-heavy, the ViewModel projects them into lightweight display classes:
*   `PlaylistDisplayItem`: Formatted for the "Card" view (e.g. `VideoCountText`).
*   `VideoDisplayItem`: Includes UI-specific state like `IsPlaying`, `ProgressPercentage`.
*   `HistoryDisplayItem`: Includes relative time strings.

### 2.3 Services (The "Thunks")
Business logic is offloaded to stateless services (in `Services/`):
*   `PlaylistService`: Fetches Entities from DB.
*   `YoutubeApiService`: Fetches metadata from the web.

---

## 3. Data Flow

### Scenario: Opening a Playlist
1.  **User Action**: Clicks a Playlist Card in `PlaylistsView`.
2.  **Command**: Fires `OpenPlaylistCommand(id)` in `MainViewModel`.
3.  **Service Call**: `App.PlaylistService.LoadPlaylistAsync(id)` fetches data from `AppDbContext`.
4.  **State Update**: 
    *   Service returns list of items.
    *   ViewModel clears `Videos` collection.
    *   ViewModel repopulates `Videos` with new `VideoDisplayItem`s.
5.  **Navigation**: `CurrentView` is set to `new VideosView()`.
6.  **UI Refresh**: WPF DataBinding detects the collection change and updates the Grid.

---

## 4. State Slices Map

| Original React Store | WPF Equivalent | Property |
| :--- | :--- | :--- |
| `router` | `MainViewModel` | `CurrentView`, `PageTitle` |
| `playlistStore` | `MainViewModel` | `Playlists` (Collection), `PlaylistService` |
| `videoStore` | `MainViewModel` | `Videos` (Collection), `CurrentVideoId` |
| `uiStore.sidebar` | `MainViewModel` | `IsLibraryVisible` (Derived) |

---

## 5. Future Refactoring (ViewModels)
As the app grows, `MainViewModel` will become too large. We plan to split it:
*   `LibraryViewModel`: Manages Playlists/Grids.
*   `PlayerViewModel`: Manages MPV/WebView handling.
*   `BrowserViewModel`: Manages CefSharp state.

These child VMs will be properties on the MainViewModel to ensure binding context flows correctly.
