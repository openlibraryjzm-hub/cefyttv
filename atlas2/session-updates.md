# Session Updates

## Session: Jan 16, 2026 - Phase 1 & 2 Complete (The Backbone & The Body)

### Major Achievements
1.  **Database Layer (Phase 1)**
    *   Implemented `AppDbContext` using Entity Framework Core Sqlite.
    *   Mapped all Rust `models.rs` structs to C# `Entities` (`Playlist`, `PlaylistItem`, `VideoFolderAssignment`, etc.).
    *   Created `PlaylistService` to handle `InitializeDatabase()` and schema generation.
    *   Verified creation of `playlists.db` (52kb initialized).

2.  **UI Architecture (Phase 2)**
    *   **Shell Refactor**: Moved from a "Triple Engine Test" grid to a robust **3-Column Split App Shell**:
        *   **Sidebar**: Persistent navigation.
        *   **Player Pane (Left)**: Always-available Engine (WebView2 for YouTube vs MPV for Local).
        *   **Content Pane (Right)**: Swappable content area.
    *   **Persistent Browser**: Implemented a "Layered" approach in the Content Pane. The `BrowserView` (CefSharp) sits permanently in the visual tree (Z-Index) to preserve tabs/session, while the lightweight WPF Library Views (`PlaylistsView`, `VideosView`) are toggled on top.

3.  **MVVM Foundation**
    *   Implemented `MainViewModel` using `CommunityToolkit.Mvvm`.
    *   Established Command-based navigation (`NavigateToPlaylists`, `NavigateToBrowser`).
    *   Created Placeholder Views for all future sections (`History`, `Pins`, `Settings`, `Support`).

### Technical Decisions
*   **"Manual First" Protocol**: Adopted a strict rule where the AI instructs the user to run terminal commands for package installation, preventing token-wasting loops.
*   **Split Layout**: Confirmed the user preference for "Left Half = Player, Right Half = Library/Browser".
*   **Browser Persistence**: Decided against destroying the CefSharp instance on navigation to ensure zero-latency browsing resumption.

### Next Steps (Phase 3)
*   Build the **Player Controller** ("The Orb") overlay.
*   Implement the Logic to switch the Left Pane between WebView2 and MPV based on content type.
*   Begin implementing the actual `PlaylistsView` DataTemplates.

---

## Session: Jan 15, 2026 - The "Triple Engine" Victory

### Key Achievements
1.  **Integrated MPV Player (Native):**
    *   Successfully embedded `libmpv` into the WPF application alongside WebView2 and CefSharp.
    *   **Architecture Win:** Bypassed the need for electron/tauri "hole punching". WPF handles the layering perfectly.
    *   **Soloved "DLL Hell":**
    *   abandoned the broken `Mpv.NET` NuGet package (incompatible with modern `mpv-2.dll`).
    *   Implemented `Controls/MpvNative.cs`: A custom, lightweight P/Invoke driver that talks directly to the DLL.
2.  **UI Integration:**
    *   Added **"Open File"** button to load local videos.
    *   Added **Status Text** overlay (outside the WinFormsHost airspace) to debug playback state.
    *   Implemented automatic swapping: Loading a file hides the WebView2 (YouTube) and reveals the MPV Player.

### Technical Learnings
*   **Airspace Issues:** `WindowsFormsHost` always paints on top of WPF content in the same window region. Overlays must work *around* it, not *over* it.
*   **Path Sanitization:** `libmpv` prefers forward slashes (`/`). Windows file paths (`\`) must be sanitized before passing to `mpv_command`.
*   **Async Void:** `dotnet watch` / Hot Reload can get confused if you change a method signature to `async void` without a full restart.
