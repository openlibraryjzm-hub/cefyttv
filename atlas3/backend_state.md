# MVVM Store Architecture & State Management

## Description
This document outlines the **Centralized State Management** strategy. Instead of scattered state (like React's Zustand), Project CCC uses a **Singleton ViewModel** pattern to create a unified "Store" accessible throughout the WPF application.

## Key Components

### 1. `MainViewModel` (The Store)
Located in `ViewModels/MainViewModel.cs`.
*   Acts as the **Single Source of Truth**.
*   **State Held**:
    *   `CurrentView`: The active screen (Router).
    *   `Playlists / Videos`: ObservableCollections driving the UI.
    *   `SelectedVideo`: The currently playing media item.
    *   `IsBrowserVisible`: Toggles the CefSharp layer.

### 2. Display Models (DTOs)
Lightweight wrappers for "Heavy" Database Entities.
*   `VideoDisplayItem`: Adds UI-only state like `IsPlaying`, `IsSelected`.
*   `PlaylistDisplayItem`: Formatted text properties for Cards.

### 3. Services (The Business Logic)
Stateless classes in `Services/` that perform actual work.
*   `PlaylistService`: DB CRUD operations.
*   `YoutubeApiService`: Web requests.
*   **Pattern**: ViewModel calls Service -> Service returns Data -> ViewModel updates ObservableCollection -> UI Updates via Binding.

## Future Plans
*   Refactor `MainViewModel` into smaller composable units (`LibraryViewModel`, `PlayerViewModel`) as complexity grows.

## Related Documentation
*   [Technical: Startup](technical_startup.md): Describes how `MainViewModel` is bootstrapped in `App.xaml.cs`.
*   [Backend: Schema](backend_schema.md): The persistent data structures managed by this state.
