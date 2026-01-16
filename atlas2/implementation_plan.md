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

## Phase 2: The Body (UI Architecture & Shell) - **COMPLETED**
**Objective:** Create the visual container and navigation structure.

### 2.1 MVVM Setup - **DONE**
*   **Tooling:** `CommunityToolkit.Mvvm` installed.
*   **Architecture:** `MainViewModel` controls state. `Views` folder populated.

### 2.2 The "Layout Shell" - **DONE**
*   **Structure:** 3-Column Grid (`Sidebar`, `PlayerPane`, `ContentPane`).
*   **Components:**
    *   **Sidebar:** MVVM Command Bindings to switching views.
    *   **Player Pane:** Hosts `WebView2` (YouTube) and `LocalVideoPlayer` (MPV).
    *   **Content Pane:** Hosts `BrowserView` (Persistent Layer) and `ContentControl` (Library Layer).
*   **Persistence:** `BrowserView` uses Visibility toggling to maintain session state.

---

## Phase 3: The Brain (Player Controller & Logic)
**Objective:** Port the complex "Orb" and interactions.

### 3.1 The "Orb" (PlayerController)
*   **Source:** `PlayerController.jsx`
*   **Destination:** `Controls/PlayerOrb.xaml` (or `PlayerController.xaml`)
*   **Location:** Moves to the `PlayerPane` (Left Col) or stays as an overlay? *Decision: Likely an overlay on the PlayerPane or bottom of ContentPane.*
*   **Key Features:**
    *   **State:** Bind to `StoreService` (Active Context).
    *   **Interactions:** Long Press (Pin), Right Click (Shuffle).

### 3.2 Engine Integration Logic
*   **Task:** Connect `MainViewModel` commands to the `PlayerPane` controls.
*   **Logic:**
    *   `PlayVideo(url)` -> Updates `ActiveContext` -> `PlayerWebView` navigates.
    *   `PlayLocal(file)` -> Updates `ActiveContext` -> `PlayerMpv` visible / Play.

---

## Phase 4: The Features (Pages & Cards)
**Objective:** Build specific views (Playlists, Videos).

### 4.1 Playlists Grid (`Views/PlaylistsView.xaml`)
*   **Source:** `PlaylistsPage.jsx` (React)
*   **Destination:** `PlaylistsView.xaml`
*   **Components:** `PlaylistCard.xaml` (replaces `PlaylistCard.jsx`)
*   **Interactions:**
    *   Click: Navigates to `VideosView` for that playlist.
    *   Drag & Drop: Reorder playlists.
    *   Context Menu: Rename, Delete, Export.

### 4.2 Videos Grid (`Views/VideosView.xaml`)
*   **Source:** `VideosPage.jsx` (React)
*   **Destination:** `VideosView.xaml`
*   **Components:** `VideoCard.xaml` (replaces `VideoCard.jsx`)
*   **Interactions:**
    *   Click: Plays video in `PlayerPane`.
    *   Drag & Drop: Reorder videos within a playlist.
    *   Context Menu: Move to another playlist, Delete, Tag.

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
