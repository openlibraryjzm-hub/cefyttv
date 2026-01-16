# Atlas Documentation Index

This directory contains comprehensive documentation for Project CCC (The "Hybrid Hub"), organized by feature and technical domain.

---

## Overview

**CCC** is a hybrid Windows Desktop application that simultaneously hosts three distinct rendering engines to achieve the best of all worlds:
1.  **WebView2 (Edge/Chromium):** for secure, lightweight web content (e.g. YouTube).
2.  **CefSharp (CEF):** for heavy-duty, fully controllable web browsing (Persistence, Tabs, AdBlock).
3.  **MPV (Native):** for high-performance localized video playback without web overhead.

The app uses **WPF** as the host container to manage the windowing, layout, and inter-process orchestration.

## Tech Stack

### Core
- **.NET 10** - Runtime
- **WPF** - UI Framework
- **C# 13** - Language

### User Interface (WPF + XAML)
*   **Shell Architecture**: 3-Column Layout:
    *   **Sidebar**: MVVM-driven navigation commands.
    *   **Player Pane (Left)**: Hosts `WebView2` (YouTube) and `LocalVideoPlayer` (MPV).
    *   **Content Pane (Right)**: Hosts the Library (Playlists/Videos) and a persistent `BrowserView` (CefSharp).
*   **Navigation**: `MainViewModel` manages view switching efficiently, keeping the Browser strictly separated from the lightweight UI.

### Project Structure (`ccc/`)
*   `Services/`
    *   `Database/`: Entity Framework Core `AppDbContext` and Entities (`Playlist`, `VideoProgress`, etc.).
    *   `PlaylistService.cs`: Business logic for DB operations.
*   `ViewModels/`
    *   `MainViewModel.cs`: The core state machine handling Navigation and Engine Visibility.
*   `Views/`
    *   `PlaylistsView.xaml`, `VideosView.xaml`: Partial implementation of Library pages.
    *   `BrowserView.xaml`: Persistent CefSharp browser instance.
    *   `TripleEngineTestView.xaml`: (Legacy/Reference) The original test bed.
*   `MainWindow.xaml`: The main application shell.

## Status Checklist
*   [x] **Triple Engine Setup** (WebView2 + CefSharp + MPV)
*   [x] **Database Layer** (EF Core Sqlite + Entities)
*   [x] **UI Shell** (Sidebar + Player + Content Split)
*   [x] **Navigation System** (MVVM Commands)
*   [ ] **Player Controller** ("The Orb")
*   [ ] **Library Logic** (Tagging, Importing)
*   [ ] **Data Migration** (Import from Atlas 1)
├── atlas2/                       # Documentation (You are here)
│   ├── architecture.md           # System design & Engine details
│   ├── ui-ux.md                  # Layouts, Controls, & Interaction
│   ├── setup.md                  # Build requirements (DLLs, Runtimes)
│   └── session-updates.md        # Change logs & "Wins"
├── MainWindow.xaml               # The Triple-Engine Layout Grid
├── App.xaml.cs                   # CefSharp Initialization logic
└── ccc.csproj                    # Build Config (Win-x64 targeting)
```

## Documentation Index

| Domain | Document | Description |
|--------|----------|-------------|
| **System Architecture** | `architecture.md` | Deep dive into the "Triple Engine" philosophy, CefSharp configuration, and MPV integration strategy. |
| **Setup & Build** | `setup.md` | Critical requirements (mpv-2.dll placement, RuntimeIdentifiers) and Developer Workflow. |
| **UI & UX** | `ui-ux.md` | Layout modes (Split, Full, Half), Control Bars, and Window management. |
| **Change Log** | `session-updates.md` | Chronological history of major features and fixes. |

---

## Usage Tips for AI Agents

1.  **Context is King:** Before modifying the rendering engines, read `architecture.md` to understand why we aren't using standard NuGet packages for MPV.
2.  **Layout Logic:** `MainWindow.xaml.cs` controls the visibility toggles between engines. Read `ui-ux.md` for the state machine logic.
3.  **DLLs matter:** This project relies on unmanaged binaries (`mpv-2.dll`, `libcef.dll`). Always check `setup.md` if "Module Not Found" errors occur.
