# MVVM Store Architecture & State Management

**Last Updated:** 2026-01-17
**Status:** Skeleton Draft
**Parent Document:** [Architecture](architecture.md)

---

## 1. The Strategy: Replacing Zustand

In the original React app, state was managed via `Zustand` stores (`playlistStore`, `videoStore`, `uiStore`). 
 In C# WPF, we replace this with the **MVVM Pattern (Model-View-ViewModel)**.

### 1.1 Core Components

1.  **`MainViewModel` ( The Singleton State Container )**
    *   This is the equivalent of the "Global Store".
    *   It holds the Single Source of Truth for the UI.
    *   It implements `INotifyPropertyChanged` to drive the UI.

2.  **`Services` ( The Logic Layer )**
    *   While ViewModels hold *UI State*, Services hold *Data Logic*.
    *   Example: `PlaylistService`, `VideoService`, `DatabaseService`.

---

## 2. Key State Slices

### 2.1 Navigation State
*   **React**: `useRouter` / conditional rendering.
*   **WPF**: `CurrentView` property in `MainViewModel`.
    *   Uses `ContentControl` Binding to swap UserControls dynamically.

### 2.2 Media State (The Player)
*   **Properties**:
    *   `CurrentVideoId` (String): YouTube ID.
    *   `IsPlaying` (Bool).
    *   `Volume` (Double).
*   **Sync**:
    *   Updates flow **Down** to controls via Binding.
    *   Updates flow **Up** from controls via Commands or Event Aggregation.

---

## 3. Implementation Plan

1.  [ ] Define `PlaylistService` to interact with `AppDbContext`.
2.  [ ] Refactor `MainViewModel` to use partial classes or nested viewmodels if it grows too large (e.g., `PlayerViewModel`, `LibraryViewModel`).
3.  [ ] Implement `ICommand` using `RelayCommand` or `CommunityToolkit.Mvvm` (if we add that package later).
