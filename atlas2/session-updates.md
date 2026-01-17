# Session Updates

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

---

## [Previous] Embedded Player Breakthrough & Data Layer - 2026-01-16
**Timestamp**: 2026-01-16

### 🚀 Architectural Breakthrough: WebView2 Embedded Player
We have successfully implemented a robust **YouTube Embedded Player** that bypasses "Error 153" (Restricted/Bot) issues.

### The Solution: Virtual Host Mapping
Instead of using `NavigateToString` (which has no Origin) or raw `file://` URLs, we implemented a **Virtual Host** strategy:
1.  **Assets**: Created `assets/player.html` containing a properly configured `<iframe>`.
2.  **Mapping**: In `MainWindow.xaml.cs`, we mapped `https://app.local` to the physical `assets` directory.
   ```csharp
   PlayerWebView.CoreWebView2.SetVirtualHostNameToFolderMapping(
       "app.local", 
       assetsPath, 
       CoreWebView2HostResourceAccessKind.Allow);
   ```
3.  **Origin**: This provides a valid `Origin: https://app.local` header to YouTube, satisfying their embed requirements.
4.  **Playback**: The app successfully loads and plays valid video IDs (e.g., Lofi Girl).

### ✅ Features Implemented
1.  **Data Persistence**:
    *   Implemented `PlaylistService` seeding logic.
    *   App now auto-seeds "Music Mix" and "Tech Talks" playlists if DB is empty.
    *   Verified Entity Framework SQLite connection.
2.  **Primitive UI Views**:
    *   Created `PlaylistCard` and `VideoCard` WPF controls.
    *   Created `PlaylistsViewModel` and `VideosViewModel` with ObservableCollections.
    *   Wired `PlaylistsView` and `VideosView` to display real data.

### ⚠️ Current Limitations (To Be Addressed)
1.  **UI Fidelity**: The current cards are "primitive" WPF implementations and do not yet match the rich aesthetics of the original React app (documented in `imported-project`).
2.  **Navigation Logic**: The `VideosView` currently shows *all* videos from the DB, rather than filtering by the selected playlist from `PlaylistsView`.
3.  **Autoplay**: The embedded player loads but may require a user click to start depending on policy (minor issue).
