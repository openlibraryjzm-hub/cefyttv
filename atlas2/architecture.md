# System Architecture

**Last Updated:** 2026-01-18
**Status:** Active
**Parent:** [README.md](README.md)

---

## 1. Core Philosophy: The "Triple Engine"

Project CCC solves the specific problem of "One browser cannot do it all" by embedding three specialized rendering engines into a single WPF Window.

### 1.1 The "App" Engine (WebView2)
*   **Technology:** `Microsoft.Web.WebView2` (Chromium Edge)
*   **Role:** The "Safe & Standard" viewer.
*   **Primary Use Case:** YouTube Video Player / Google Services.
*   **Why:** Google actively fights specialized browsers (like CEF). WebView2 is effectively Edge, so it passes DRM and Integrity checks easily. It is lightweight and shares the OS runtime.

### 1.2 The "Browser" Engine (CefSharp)
*   **Technology:** `CefSharp.Wpf.NETCore`
*   **Role:** The "Power User" browser.
*   **Primary Use Case:** General Web Browsing, Persistent Tabs, Ad-Blocking.
*   **Why:** We need full control over the network stack (request interception), cookies, and window management (popups) which WebView2 limits.
*   **Configuration:**
    *   **Cache:** `%LocalAppData%/ccc/Cache` (Persistent).
    *   **Popups:** Forced into current tab via `Handlers/CustomLifeSpanHandler.cs`.

### 1.3 The "Video" Engine (MPV Native)
*   **Technology:** `libmpv` (v2)
*   **Role:** The "Raw Power" media player.
*   **Primary Use Case:** Local MKV/MP4 playback.
*   **Why:** Web browsers have poor codec support (HEVC/MKV issues) and overhead. Native MPV renders directly to the GPU, bypassing the DOM entirely.

---

## 2. Technical Implementation Details

### 2.1 MVVM State Management
Unlike the original React app (Zustand), we use **CommunityToolkit.Mvvm**:
*   **`MainViewModel`**: Plays the role of the Singleton Store.
*   **Binding**: Views bind directly to `MainViewModel` (e.g. `CurrentView`, `Playlists`).
*   **Services**: Data fetching logic is isolated in `Services/` (e.g. `PlaylistService`, `YoutubeApiService`).

### 2.2 MPV Integration (The "Native Driver" Pattern)
Unlike standard NuGet implementations, we use a **Manual P/Invoke Driver** (`Controls/MpvNative.cs`).
1.  **Problem**: The `Mpv.NET` NuGet package is outdated/unmaintained.
2.  **Solution**: We define our own `DllImport` signatures matching `client.h`.
3.  **Rendering**:
        *   We define a `WindowsFormsHost` in WPF (`LocalVideoPlayer.xaml`).
        *   We pass the `host.Handle` (HWND) to MPV via `mpv_set_option_string(..., "wid", handle)`.

### 2.3 Layout Management
The layout is defined in `MainWindow.xaml` and relies on Z-Index Layering:

| Layer | Component | Grid Row |
| :--- | :--- | :--- |
| **Top (100)** | `AdvancedPlayerController` | 0 |
| **Content** | `TripleEngineTestView` (Split Grid) | 1 |

The Content Grid splits 50/50:
*   **Left**: Player Container (WebView2 or MPV).
*   **Right**: Library Container (WPF Views or CefSharp Browser).

---

## 3. Build & Runtime
*   **Target**: `net10.0-windows`
*   **Platform**: `win-x64` (Required by CefSharp/MPV unmanaged DLLs).
*   **Hot Reload**: Enabled via `dotnet watch`.
