# Session Updates

## [Refactor Phase 2] Building Blocks - 2026-01-17
We have successfully implemented **Phase 2: Building Blocks** of the refactor roadmap. This phase focused on translating the design system and atomic UI components from React to WPF `UserControls` and Resources.

### 🎨 Visual System Implemented
1.  **Resource Dictionaries**:
    *   `Resources/Colors.xaml`: Ported all theme colors (Sky Blue system) and the **16 Folder Colors** from `App.css`/`themes.js`.
    *   `Resources/Styles.xaml`: Defined global styles for `BaseCard` (Border/Background) and invisible Buttons.
    *   `App.xaml`: Configured to merge these resources on startup.

### 🧩 Components Created
1.  **Atomic Controls** (`Controls/Cards/`):
    *   **`CardThumbnail.xaml`**: A highly reusable component encapsulating the 16:9 image, "Now Playing" animated badge (placeholder), "Watched" checkmark, Progress Bar, and Hover Overlay.
    *   **`VideoCard.xaml`**: The primary item card. Composes `CardThumbnail` with a content area for Title/ID. Includes placeholders for the **Pin**, **Star**, and **3-Dot Menu** hover actions described in `ui.md`.
    *   **`PlaylistCard.xaml`**: Implemented the specific "Header-Above-Thumbnail" layout for playlists. Includes the inner-rectangle title styling and specific hover actions (Play, Shuffle, Preview).

2.  **Input Controls** (`Controls/Inputs/`):
    *   **`FolderColorGrid.xaml`**: A reusable 4x4 grid of colored buttons for folder selection menus.

### 🔧 Wiring & Fixes
*   **App.xaml.cs**: Fixed initialization of static Service properties (Sqlite, Playlist, Navigation, Link, Folder) to be null-safe and properly ordered.
*   **Build Success**: Resolved ambiguity issues between `System.Windows.Forms` and `System.Windows.Controls` in UserControls.

### ✅ Status
*   The "Lego blocks" for the UI are ready.
*   Next Phase (Phase 3) will involve assembling these blocks into the actual Page Views (`PlaylistsView`, `VideosView`) and wiring them to the ViewModels.

---

## [Refactor Phase 1] Core Data Layer - 2026-01-17
We have successfully implemented **Phase 1** of the North Star refactoring roadmap, establishing the backend parity with the original Tauri implementation.

### 🏗️ Infrastructure Implemented
1.  **Entities & Schema**: Created strict EF Core entities in `Models/Entities` matching `database-schema.md` (Playlists, Items, FolderAssignments, Progress, History, Sticky).
2.  **Services Layer**:
    *   **SqliteService**: A robust Data Access Layer wrapping `AppDbContext` to mirror Tauri's database commands for CRUD operations.
    *   **ConfigService**: Handles persistent user preferences (Theme, Profile) with JSON storage.
    *   **NavigationService**: Manages internal page routing state (`CurrentPage`, History).
    *   **FolderService**: Manages folder selection state and folder assignment logic.
    *   **PlaylistService**: Refactored to act as the central **State Store** (Stateful Service) for playlist data and playback navigation state.
    *   **Logic Services**: `PinService`, `StickyService`, `ShuffleService` implemented with respective logic.
3.  **Wiring**: All services are instantiated and initialized in `App.xaml.cs`.

### ✅ Status
*   The Data Layer is now ready to support the UI components (Phase 2).
*   `App.xaml.cs` now manages the lifecycle of these services, replacing previous ad-hoc singletons.
*   Old `Services/Database/Entities` folder was cleaned up in favor of `Models/Entities`.

---

## [Clean Slate] Return to Foundation - 2026-01-17
We have successfully **wiped the slate clean** of the initial refactoring attempts (partial Library views, Services, and interim ViewModels) to return to the core, stable "Triple Engine" architecture.

### Why the Reset?
The initial refactor (porting React concepts like Stores directly to C# Services) introduced compilation complexity and drift from the primary goal: a solid Hybrid Host. We are returning to the bare metal to ensure the "Triple Engine" (WebView2, CefSharp, MPV) works flawlessly before layering complex business logic on top.

### ✅ Current State (The "Blank Slate")
1.  **Shell**: `MainWindow` hosting `TripleEngineTestView`.
2.  **Engines**:
    *   **WebView2**: Loads YouTube (`www.youtube.com`) successfully.
    *   **CefSharp**: Loads Browsing Tabs (`www.google.com`) with popup handling.
    *   **MPV**: Loads Local Files via P/Invoke (`MpvNative.cs`).
3.  **Layout**: Split Pane (Player Left, Browser Right) with Fullscreen toggles.
4.  **Database**: `AppDbContext` remains in `Services/Database` (Schema preserved) but is currently disconnected from the UI.

### 🗑️ Cleanup
*   Deleted: `Views/PlaylistsView`, `Views/VideosView`, `Controls/Cards/*`, `Services/PlaylistService` (logic layer).
*   Preserved: `Core Engines`, `Handlers`, `Database Entities`.
