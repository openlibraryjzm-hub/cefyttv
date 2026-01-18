# Startup Flow

**Last Updated:** 2026-01-17
**Status:** Skeleton Draft
**Parent Document:** [Architecture](architecture.md)

---

## 1. The Sequence

The application startup is non-trivial because it involves initializing heavyweight unmanaged resources (CEF, MPV) in a specific order.

### 1.1 App.xaml.cs (Entry Point)
1.  **CefSharp Initialization**: Must happen *before* the first Window is created.
    *   Set `CefSettings` (CachePath, UserAgent).
    *   Call `Cef.Initialize(settings)`.

### 1.2 MainWindow Constructor
1.  `InitializeComponent()`: Loads XAML.
2.  **WebView2 Initialization**:
    *   `EnsureCoreWebView2Async()` is called (usually lazily or on load).

### 1.3 ViewModel Bootstrapping
1.  `MainViewModel` Constructor.
2.  Database Connection Check (`EnsureCreated`).
3.  Load Initial Data (e.g. Last Played Video).
4.  **Auto-Play Initialization**:
    *   `LoadDataAsync`: Fetches all Playlists.
    *   Selects first Playlist.
    *   Loads first Video and triggers `PlayVideoCommand`.

---

## 2. Race Conditions to Watch
*   **WebView2 vs CefSharp**: Both involve multi-process rendering. Ensure UI thread isn't blocked.
*   **MPV Loading**: Requires `mpv-2.dll` to be present. If missing, app might crash or show black screen.
