# Project CCC Documentation (Atlas 3)

## 🧭 The "Start Here" Guide
**Read these files in order to understand the system:**
1.  **Architecture**: [layout.md](layout.md) (The visual skeleton), [backend_engines.md](backend_engines.md) (The triple-engine core), [backend_state.md](backend_state.md) (The Brain/Store).
2.  **The "Bridge"**: [technical_bridge.md](technical_bridge.md) (How C# talks to JS).
3.  **The Data**: [backend_schema.md](backend_schema.md) (What we save).
4.  **The Code**: [dev_setup.md](dev_setup.md) & [technical_startup.md](technical_startup.md) (How it runs).

---

## ⚡ The Golden Rules
1.  **Triple Engine Law**: The application runs 3 engines (WebView2, CefSharp, MPV) simultaneously. You **MUST** respect the Z-Index layering defined in `layout.md`. Never assume standard WPF layering applies to the Airspace of these engines.
2.  **State is Singleton**: All application state lives in `MainViewModel`. Do not store page-specific state in `UserControl` code-behind if it needs to persist across navigation.
3.  **UI is "Glass"**: We use semi-transparent panels over a global Parallax Banner (`Index 0`). Avoid solid opaque backgrounds for main containers; use `BackdropBlur` and opacity where possible.
4.  **Database is Truth**: We use SQLite via EF Core. The `playlists.db` file is the system of record.
5.  **Icons are Paths**: Do not use Bitmap icons. Use Vector Paths (Geometry) to ensure they scale and inherit `Foreground` updates.

---

## App/Dev Overview

**Project CCC** (Formerly yttv2 - a rust-tauri project before it was refactored to c#) is a desktop application for managing and playing YouTube playlists, Local video files (mp4, webm, etc) that also features a specialized "Full Browser" mode using a CEF Child engine. The app provides a modern, grid-based interface for browsing playlists and videos, with full SQLite database integration for persistent storage. Re-factoring front-end and back-end largely complete, now in tuning/style and final features stage.

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

This entire stack is orchestrated by **WPF (Windows Presentation Foundation)**, which provides the windowing, transparent layering, and MVVM state management to make these three engines feel like a single, cohesive application..

---

## 🏛️ Domain Map

### 1. The Core Architecture
*   [layout.md](layout.md): **(Critical)** The Z-Index, Window Grids, and Shell Zones.
*   [backend_engines.md](backend_engines.md): The "Triple Engine" philosophy (WebView2 vs Cef vs MPV).
*   [backend_state.md](backend_state.md): The MVVM Store pattern (`MainViewModel`).
*   [backend_routing.md](backend_routing.md): How we switch views and toggle engine layers.
*   [backend_schema.md](backend_schema.md): The SQLite connection and Table definitions.

### 2. The Technical Docs
*   [technical_startup.md](technical_startup.md): The complex boot sequence (Cef Initialize -> Window -> VMD).
*   [technical_bridge.md](technical_bridge.md): The bidirectional IPC layer (JS <-> C#) for controlling YouTube.
*   [dev_setup.md](dev_setup.md): Environment needs (MPV-2.dll context).

### 3. The HUD & Shell (Persistent UI)
*   [advplayercontroller.md](advplayercontroller.md): The top-floating HUD (Menu, Orb, Playback).
*   [orbmenu.md](orbmenu.md): The central interaction element.
*   [topnavigationbar.md](topnavigationbar.md): The Tabs (Playlists/Videos) and Utility Icons.
*   [appbanner.md](appbanner.md): The Global Parallax Background (Index 0).
*   [pagebanners.md](pagebanners.md): The scrolling contextual headers.

### 4. The Views (Navigable Content)
*   [playlistspage.md](playlistspage.md): The "Home" library view.
*   [videospage.md](videospage.md): The drill-down video grid.
*   [historypage.md](historypage.md): Chronological watch log.
*   [likespage.md](likespage.md): Favorites collection.
*   [pinspage.md](pinspage.md): Quick-access "Stuck" items.
*   [settingspage.md](settingspage.md): Configuration & Customization.

### 5. Features & Systems
*   [importexport.md](importexport.md): YouTube API integration & "Add" Modal.
*   [audiovisualizer.md](audiovisualizer.md) *: (Placeholder) Future Audio Reactivity.*
*   [supportpage.md](supportpage.md) *: (Placeholder) Help docs.*

---

## 🏗️ Project Structure

This tree highlights the **critical files** for understanding the application's architecture.

```text
ccc/
├── Controls/                   # Reusable UI Components
│   ├── Navigation/             
│   │   └── TopNavigation.xaml      # 🧭 The Tab Bar (Playlists/Videos/Icons)
│   ├── Player/                 
│   │   └── AdvancedPlayerController.xaml  # 🎛️ The Main HUD (Orb, Menus, Playback)
│   ├── Visuals/
│   │   ├── UnifiedBannerBackground.xaml   # 🌌 The Parallax Background Shell
│   │   └── PageBanner.xaml                # 🏷️ The Scrolling Header for Pages
│   └── Cards/                  # Data Display
│       ├── VideoCard.xaml         # Standard Video Grid Item
│       └── PlaylistCard.xaml      # Standard Playlist Grid Item
│
├── Services/                   # Business Logic
│   ├── Database/AppDbContext.cs   # 💾 SQLite Entity Framework Context
│   ├── PlaylistService.cs         # 📚 Core Library Management
│   ├── NavigationService.cs       # 🗺️ Routing Logic
│   └── YoutubeApiService.cs       # ☁️ External Data Fetching
│
├── ViewModels/                 
│   └── MainViewModel.cs        # 🧠 The "Store" - Singleton App State
│
├── Views/                      # 🖼️ Main Page Layouts
│   ├── PlaylistsView.xaml      # Home / Library Grid
│   ├── VideosView.xaml         # Video Grid (Generic)
│   ├── HistoryView.xaml        # Watch History Timeline
│   ├── LikesView.xaml          # Favorites Collection
│   └── BrowserView.xaml        # CefSharp "Full Browser" Mode
│
└── atlas3/                     # 📘 Documentation System (Current)
    ├── advplayercontroller.md  # 🎛️ HUD: The Orb, Menus, and 3-Wing layout
    ├── appbanner.md            # 🌌 Shell: The Unified Parallax Background
    ├── asciibanner.md          # 🎨 Visuals: ASCII Art styling (Placeholder)
    ├── audiovisualizer.md      # 🎼 Audio: Reactive visualizers/rings
    ├── backend_engines.md      # 🚂 Core: The Triple Engine Arch (WebView2/Cef/MPV)
    ├── backend_routing.md      # 🗺️ Core: WPF Navigation & Layer Switching
    ├── backend_schema.md       # 💾 Data: SQLite Schema & Entities
    ├── backend_state.md        # 🧠 Core: MVVM Store & State Management
    ├── dev_setup.md            # 🛠️ Guide: Build requirements & mpv-2.dll
    ├── historypage.md          # 🕒 View: Watch History logic
    ├── importexport.md         # 📥 Logic: YouTube Imports & Add Modal
    ├── layout.md               # 📐 UI: Global Window Layout, Z-Index, and Shell Zones
    ├── likespage.md            # ❤️ View: Liked Videos collection
    ├── orbmenu.md              # 🔮 HUD: The Central Orb & Customization
    ├── pagebanners.md          # 🏷️ Visuals: Scrolling context headers
    ├── piebanner.md            # 🥧 Visuals: Radial data displays (Placeholder)
    ├── pinspage.md             # 📌 View: Pinned/Stuck items
    ├── playlistspage.md        # 📚 View: The Main Library Grid
    ├── readme3.md              # 🧭 Index: This file
    ├── settingspage.md         # ⚙️ View: Config, Orib Customization, Paths
    ├── supportpage.md          # 🆘 View: Help & Support (Placeholder)
    ├── technical_bridge.md     # 🌉 Code: Interop & Virtual Host (JS <-> C#)
    ├── technical_startup.md    # 🚦 Code: Boot Sequence & Engine Init
    ├── topnavigationbar.md     # 🧭 Nav: The Top Tab Bar & Icons
    └── videospage.md           # 🎬 View: Video Grids & Sorting Logic
```