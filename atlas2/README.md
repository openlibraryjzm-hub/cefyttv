# Atlas Documentation Index

This directory contains comprehensive documentation for Project CCC (The "Hybrid Hub"), serving as the **System of Record** for the application's architecture, state, and implementation details.

## App Overview

**Project CCC** is a desktop application for managing and playing YouTube playlists, Local video files (mp4, webm, etc) that also features a specialized "Full Browser" mode using a CEF Child engine. The app provides a modern, grid-based interface for browsing playlists and videos, with full SQLite database integration for persistent storage.

## 🌟 The Hybrid Hub Concept

**Project CCC** is a Windows Desktop application that solves the "Browser Performance vs Native Control" dilemma by adopting a **Triple Engine Architecture**:

1.  **WebView2 (Edge/Chromium)**:
    *   **Role**: *The Protected Web*.
    *   **Use Case**: YouTube Embedded Player.
    *   **Reasoning**: Handles rigorous DRM, codec complexities, and anti-scraping measures of modern video platforms effortlessly.
2.  **CefSharp (CEF/Chromium)**:
    *   **Role**: *The Controlled Web*.
    *   **Use Case**: Persistent browsing, Tab management, AdBlocking, and Request Interception.
    *   **Reasoning**: Provides the deep hooks (ResourceHandler, CookieManager) that WebView2 hides, allowing for a "Power User" browser experience.
3.  **MPV (Native/FFmpeg)**:
    *   **Role**: *The Native Power*.
    *   **Use Case**: Local video playback and high-bitrate content.
    *   **Reasoning**: Zero-latency, hardware-accelerated playback without the memory overhead of a browser engine.

This entire stack is orchestrated by **WPF (Windows Presentation Foundation)**, which provides the windowing, transparent layering, and MVVM state management to make these three engines feel like a single, cohesive application.

---

## 🧭 Service Manual & System Map

**For AI Agents:** This documentation is structured to answer specific questions. Use the map below to find the right context.

### 🟡 Status Legend
| Icon | Meaning | Action for Agent |
| :--- | :--- | :--- |
| ✅ | **Active / Implemented** | Read this for the Truth. Code exists and works. |
| ⚠️ | **Skeleton / Partial** | Read for High-Level Strategy. Detailed implementation is missing or TBD. |
| 🚧 | **Planned / In-Progress**| Read for Intent. Code is currently being written or does not exist. |
| ❌ | **Missing** | No documentation exists. Rely on generic WPF knowledge. |

### 1. Architecture & Core
*   **"What was the refactor plan?"**
    *   📄 [NORTH_STAR2.md](NORTH_STAR2.md) (✅ Active)
    *   *The now completed refactor plan - reference only now.*
*   **"How do the 3 engines work together?"**
    *   📄 [architecture.md](architecture.md) (✅ Active)
    *   *System design, Triple Engine philosophy, and Engine integration.*
*   **"What is the State Store?"**
    *   📄 [mvvm-store-architecture.md](mvvm-store-architecture.md) (✅ Implemented)
    *   *State Management. How `MainViewModel` replaces Zustand/Redux.*
*   **"What is the current status of the App?"**
    *   📄 [current-state.md](current-state.md) (✅ Active)
    *   *Snapshot. Detailed description of the app's status as of Jan 2026.*
*   **"How does the app start up?"**
    *   📄 [startup-flow.md](startup-flow.md) (⚠️ Skeleton)
    *   *Application initialization sequencing (CefSharp, WebView2, Database).*

### 2. UI & Interaction
*   **"How does navigation work?"**
    *   📄 [navigation-routing.md](navigation-routing.md) (✅ Implemented)
    *   *View switching logic, Back stack, and History.*
*   **"How is the Top Player Controller built?"**
    *   📄 [advanced-player-controller.md](advanced-player-controller.md) (✅ Implemented)
    *   *Deep Dive into the top controller strip (Orb, Menus, Buttons).*
*   **"What are the Colors/Styles?"**
    *   📄 [ui-system.md](ui-system.md) (⚠️ Skeleton)
    *   *Design System, Theming (Colors/Fonts), and Layout Strategy.*

### 3. Data & Persistence
*   **"What is the Database Schema?"**
    *   📄 [database-schema.md](database-schema.md) (✅ Implemented)
    *   *SQLite Schema. Mirroring the Rust/Diesel definition.*
*   **"Where are user settings?"**
    *   📄 [settings-configuration.md](settings-configuration.md) (⚠️ Skeleton)
    *   *User preferences, Config Store, and LocalStorage equivalents.*

### 4. Technical Implementation & Development
*   **"How do I build the app?"**
    *   📄 [setup.md](setup.md) (✅ Stable)
    *   *Build requirements (`mpv-2.dll`, runtimes) and "Hot Reload" workflow.*
*   **"How does the YouTube Interop work?"**
    *   📄 [interop-services.md](interop-services.md) (⚠️ Skeleton)
    *   *API Bridge. How JS (WebView2) talks to C# and vice versa.*
*   **"How does MPV load?"**
    *   📄 [media-engines.md](media-engines.md) (⚠️ Skeleton)
    *   *Video Player logic. MPV P/Invoke, YouTube Wrappers, Audio Visualizers.*
*   **"What did we just change?"**
    *   📄 [session-updates.md](session-updates.md) (✅ Active)
    *   *Chronological change log and "Wins".*
*   **"How do I fix Error 153?"**
    *   📄 [youtube-error-153.md](youtube-error-153.md) (✅ Case Study)
    *   *Specific troubleshooting guide for the "Error 153" saga.*

---

## 🏗️ Project Structure

This tree represents the actual, current state of the C# codebase as of `Jan 18, 2026`.

```text
ccc/
├── Controls/                   # Reusable UI Components
│   ├── Buttons/                # Styled Buttons (IconButton, etc.)
│   ├── Cards/                  # Data Display (VideoCard, PlaylistCard)
│   ├── Inputs/                 # Form Elements (SearchBox)
│   ├── Lists/                  # Collection Views (VerticalList, Sidebar)
│   ├── Menus/                  # Context Menus & Dropdowns
│   ├── Navigation/             # Nav Bars & Sidebars
│   │   ├── TopNavBar.xaml      # (Legacy) Top Bar
│   │   └── ...
│   ├── Player/                 # Media Controls
│   │   ├── AdvancedPlayerController.xaml  # The 3-Part Top Controller (Orb/Menus)
│   │   └── WebViewYouTubeControls.xaml    # YouTube Embed Wrapper (WebView2)
│   ├── Visuals/                # Decorators (Banners, Loading Skeletons)
│   ├── LocalVideoPlayer.xaml   # MPV Container (WinFormsHost)
│   ├── MpvNative.cs            # P/Invoke Driver for mpv-2.dll (Manual Binding)
│   └── TopNavigation.xaml      # Primary Navigation Tabs
│
├── Handlers/                   # CefSharp Core Handlers
│   ├── CustomLifeSpanHandler.cs # Popup & Window Management
│   └── CustomRequestHandler.cs  # Network Interception & AdBlock
│
├── Models/                     # Data Structures
│   ├── Entities/               # Database Models (EF Core)
│   │   ├── Playlist.cs         # Playlist Metadata
│   │   ├── PlaylistItem.cs     # Video Items
│   │   ├── WatchHistory.cs     # Playback Events
│   │   └── ...                 # (+ FolderMetadata, StuckFolder, etc.)
│   └── ...
│
├── Services/                   # Business Logic Layer
│   ├── Database/               # EF Core Context & Migrations
│   ├── ConfigService.cs        # Settings & Preferences
│   ├── FolderService.cs        # Folder & Tab Logic
│   ├── NavigationService.cs    # View Switching Logic
│   ├── PinService.cs           # Pinned Item Logic
│   ├── PlaylistService.cs      # Core Playlist CRUD
│   ├── ShuffleService.cs       # Randomization Logic
│   ├── StickyService.cs        # Sticky Headers Logic
│   └── YoutubeApiService.cs    # Metadata Fetching (External API)
│
├── ViewModels/                 # MVVM State Containers
│   ├── MainViewModel.cs        # Singleton App State (The "Store")
│   └── ...
│
├── Views/                      # Full Page Layouts (UserControls)
│   ├── BrowserView.xaml        # CefSharp Browser Container
│   ├── HistoryView.xaml        # Watch History Page
│   ├── LikesView.xaml          # Liked Videos Page
│   ├── PinsView.xaml           # Pinned Videos Page
│   ├── PlaylistsView.xaml      # Library Grid Page
│   ├── SettingsView.xaml       # App Configuration Page
│   ├── SupportView.xaml        # Support/Donate Page
│   ├── VideosView.xaml         # All Videos Grid Page
│   └── TripleEngineTestView.xaml # (Debug) Engine Integration Tester
│
├── App.xaml.cs                 # Entry Point (CefSharp Init)
├── MainWindow.xaml             # Root Window (Grid Layout & Z-Index Layering)
└── atlas2/                     # Documentation System (This Directory)
    ├── NORTH_STAR2.md          # 🚀 Rust-Tauri -> C# Refactor plan (completed)
    ├── architecture.md         # 🏛️ System Design & Triple Engine Theory
    ├── current-state.md        # 📸 Current Status Snapshot
    ├── README.md               # 🧭 The Index (You are here)
    ├── documentation-conversion.md # 🗺️ Mapping Legacy (Rust/Tauri) to New (C#) Docs
    ├── setup.md                # 🛠️ Build & Environment Setup
    ├── session-updates.md      # 📝 Chronological Change Log
    ├── ui-system.md            # 🎨 Design System & Theming
    ├── database-schema.md      # 💾 SQLite Schema & Entities
    ├── advanced-player-controller.md # 🎛️ Top Controller Deep-Dive
    ├── mvvm-store-architecture.md    # 🧠 State Management (MVVM)
    ├── navigation-routing.md   # 🗺️ Navigation Logic
    ├── interop-services.md     # 🌉 JS <-> C# Bridge Specs
    ├── media-engines.md        # 🎬 MPV & Youtube Player Details
    ├── startup-flow.md         # 🚦 Boot Sequence Details
    ├── settings-configuration.md # ⚙️ Config & Preferences
    └── youtube-error-153.md    # 🐛 "Error 153" Case Study
```

## Usage Tips for AI Agents

1.  **Read First**: Before asking "how does X work?", check the corresponding domain document above.
2.  **State vs UI**: We strictly separate Logic (`ViewModels/Services`) from Presentation (`Views/Controls`).
3.  **Engine Specifics**: This is a *Hybrid* app.
    *   **WebView2**: Lightweight, Public Web (YouTube).
    *   **CefSharp**: Heavy, Persistent Web (Browsing).
    *   **MPV**: Native Local Video.
4.  **Documentation is Live**: If you implement a feature, **you must update the corresponding `atlas2` file**.
