# Atlas 2.0 Refactor Plan (Rust/React -> C#/WPF)

## Overview
This document outlines the detailed strategy for porting the `yttv2` application to the `ccc` "Triple Engine" architecture.

**Goal:** Recreate the "Library Manager" functionalities (Playlists, Local/Web playback, History, Tagging) using .NET 10 & WPF, while maintaining pixel-perfect design parity with the original React UI.

> **AI INSTRUCTION PROTOCOL:**
> *   **Manual Installations:** The AI MUST NOT attempt to run `dotnet add` or `npm install` commands automatically. Instead, provide the specific command for the USER to run.
> *   **File Movements:** If existing files need to be moved (e.g., icons, assets), asking the USER to do so is preferred over automated moving.
> *   **Verification:** After a user performs a manual action, ask for confirmation before proceeding.


## Critical Dependencies
The following NuGet packages are required:
*   `Microsoft.EntityFrameworkCore.Sqlite` (Database)
*   `CommunityToolkit.Mvvm` (MVVM Pattern)
*   `Microsoft.EntityFrameworkCore.Design` (Migrations)
*   `Microsoft.Extensions.DependencyInjection` (Service Injection - likely already in .NET Core/WPF template, but verify)
*   `Newtonsoft.Json` (Import/Export logic)

**Installation Command:**
```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Newtonsoft.Json
```

---

## Phase 1: The Backbone (Data & Services) - **COMPLETED**
**Objective:** Replace the Rust Database/Commands and JS Config Stores.

### 1.1 Database (EF Core) - **DONE**
**Files:** `Services/Database/`
*   `AppDbContext.cs`: The generic unit of work.
*   **Entities**:
    *   `Playlist`: Tracks custom ASCII art, thumbnails.
    *   `PlaylistItem`: Tracks `IsLocal`, position, metadata.
    *   `VideoFolderAssignment`: The "Color Tag" system.
    *   `StuckFolder`: Tracks folders that are "pinned" to the grid.
    *   `WatchHistory`: Last 100 watched videos.
    *   `VideoProgress`: Resume playback positions.

### 1.2 State Management (Services) - **DONE**
**Files:** `Services/`
*   **`StoreService`**: A Singleton that holds the *in-memory* application state (replacing Zustand).
    *   Properties: `AllPlaylists`, `ActiveContext`, `PreviewContext`.
    *   Events: `OnPlaylistChanged`, `OnVideoChanged`.
*   **`ConfigService`**: Persist User Profile (Avatar/Name) and Visual Settings (Orb Scale, Spill) to `appsettings.json` or a local `.json` file.
*   **`PlaylistService`**: Handles the complex business logic of "Importing," "Moving," and "Deleting" (The logic from `playlistApi.js`).

---

## Phase 2: The Body (UI Architecture)
**Objective:** Setup the WPF Window for MVVM and Navigation.

### 2.1 Shell Structure
**Files:** `MainWindow.xaml`, `ViewModels/MainViewModel.cs`
*   **Regions:**
    *   `PlayerControllerRegion` (Top)
    *   `MainContentRegion` (Center - Navigable)
    *   `StickyToolbarRegion` (Overlay)
*   **Engine Handling:**
    *   The `MainWindow` code-behind stays as the "Traffic Cop" for Engine Visibility (WebView vs MPV vs CefSharp).
    *   The `ViewModel` exposes a generic `CurrentEngineState` enum that the View binds to.

### 2.2 Navigation System
**Files:** `Services/NavigationService.cs`
*   Instead of React Router, we use a `CurrentView` property in the ViewModel associated with `DataTemplates`.
*   **Views:** `PlaylistsView`, `VideosView`, `HistoryView`.

---

## Phase 3: The Brain (Player Controller)
**Objective:** Port the React `PlayerController.jsx` to WPF.

### 3.1 The "Orb" Control (`Controls/PlayerOrb.xaml`)
*   **Visuals:** Use a `Border` with `CornerRadius="100"` and `ImageBrush`.
*   **Spill Effect:** This is hard in WPF. We will likely use a `Canvas` with the image scaled larger than the container and `ClipToBounds="False"`.
*   **Audio Visualizer:** We will likely need a new `NAudio` or `Bass.Net` implementation to drive the visualizer ring, as the Rust `cpal` implementation cannot be copy-pasted. **(Deferred to Phase 6)**.

### 3.2 The Menus (`Controls/PlayerMenu.xaml`)
*   **Video Menu:** Contains `Pin`, `Like`, `Shuffle` buttons.
*   **Playlist Menu:** Contains Title and Navigation Chevrons.
*   **Logic:** These controls directly bind to `StoreService.ActiveContext`.

---

## Phase 4: Features & Interactions
**Objective:** The specific complex UI interactions.

### 4.1 "Sticky" Toolbar
*   Use a `Grid` overlay in `MainWindow` with a high Z-Index.
*   Bind its `Visibility` and `Opacity` to a `ScrollChanged` event from the generic content scroller.

### 4.2 Drag & Drop / Context Menus
*   WPF has native Drag & Drop. Use this for reordering Playlists/Videos.
*   **Context Menus:** Use `ContextMenu` on the `ListViewItem` styles for "Move to Folder" and "Delete".

### 4.3 Skeleton Loading
*   WPF doesn't do "Suspense" like React.
*   **Strategy:** Use a `IsLoading` boolean in the ViewModel. When true, swap the `DataTemplate` to a `SkeletonTemplate` (using gray rectangles with animation triggers).

---

## Phase 5: The Triple Engine Integration
**Objective:** Making the engines talk to the Brain.

### 5.1 WebView2 (YouTube)
*   Use `CoreWebView2.ExecuteScriptAsync` to inject the Player Control Logic (Play/Pause).
*   Listen to `WebMessageReceived` to get Progress updates back from the YouTube JS.

### 5.2 MPV (Native)
*   The `MpvNative` class already exists.
*   Wire the `PlayerController` "Play" command to `MpvNative.Command("cycle pause")`.

---

## Folder Structure (New)
```
ccc/
├── Services/
│   ├── Database/
│   │   ├── AppDbContext.cs
│   │   └── Entities/
│   ├── StoreService.cs
│   └── ConfigService.cs
├── ViewModels/
│   ├── MainViewModel.cs
│   ├── PlaylistsViewModel.cs
│   └── PlayerControllerViewModel.cs
├── Views/
│   ├── PlaylistsView.xaml
│   └── VideosView.xaml
├── Controls/
│   ├── PlayerOrb.xaml
│   ├── VideoCard.xaml
│   └── PlaylistCard.xaml
└── Implementation/
    └── Utils/ (Ported Logic)
```
