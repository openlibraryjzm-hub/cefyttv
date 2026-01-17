# Atlas Documentation Index

This directory contains comprehensive documentation for Project CCC (The "Hybrid Hub"), serving as the **System of Record** for the application's architecture, state, and implementation details.

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

## 📚 System Documentation Map

We structure our documentation to mirror the successfully deployed Rust/Tauri architecture, adapted for the C# WPF paradigm.

### 1. Architecture & Core
| Document | Description | Status |
| :--- | :--- | :--- |
| `NORTH_STAR2.md` | The Grand Roadmap and Master Plan. | ✅ Active |
| `architecture.md` | System design, Triple Engine philosophy, and Engine integration. | ⚠️ Update Needed |
| `mvvm-store-architecture.md` | **State Management**. How `MainViewModel` replaces Zustand/Redux. | 🚧 In Progress |
| `interop-services.md` | **API Bridge**. How JS (WebView2) talks to C# and vice versa. | 🚧 In Progress |
| `startup-flow.md` | Application initialization sequencing (CefSharp, WebView2, Database). | 🚧 In Progress |

### 2. UI & Interaction
| Document | Description | Status |
| :--- | :--- | :--- |
| `ui-system.md` | Design System, Theming (Colors/Fonts), and Layout Strategy. | 🚧 Planned |
| `navigation-routing.md` | **Navigation**. View switching logic, Back stack, and History. | 🚧 In Progress |
| `advanced-player-controller.md`| **Deep Dive** into the top controller strip (Orb, Menus, Buttons). | ⚠️ Skeleton |

### 3. Data & Persistence
| Document | Description | Status |
| :--- | :--- | :--- |
| `database-schema.md` | **SQLite Schema**. Mirroring the Rust/Diesel definition. | 🚧 In Progress |
| `settings-configuration.md` | User preferences, Config Store, and LocalStorage equivalents. | ❌ Missing |

### 4. Media Engines
| Document | Description | Status |
| :--- | :--- | :--- |
| `media-engines.md` | **Video Player** logic. MPV P/Invoke, YouTube Wrappers, Audio Visualizers. | 🚧 In Progress |

### 5. Developer Guide
| Document | Description | Status |
| :--- | :--- | :--- |
| `setup.md` | Build requirements (`mpv-2.dll`, runtimes) and "Hot Reload" workflow. | ✅ Stable |
| `session-updates.md` | Chronological change log and "Wins". | ✅ Active |
| `youtube-error-153.md` | Specific troubleshooting guide for the "Error 153" saga. | ✅ Case Study |

---

## 🏗️ Project Structure

This tree represents the actual, current state of the C# codebase as of `2026-01-17`.

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
```

## Usage Tips for AI Agents

1.  **Read First**: Before asking "how does X work?", check the corresponding domain document above.
2.  **State vs UI**: We strictly separate Logic (`ViewModels/Services`) from Presentation (`Views/Controls`).
3.  **Engine Specifics**: This is a *Hybrid* app.
    *   **WebView2**: Lightweight, Public Web (YouTube).
    *   **CefSharp**: Heavy, Persistent Web (Browsing).
    *   **MPV**: Native Local Video.
4.  **Documentation is Live**: If you implement a feature, **you must update the corresponding `atlas2` file**.
