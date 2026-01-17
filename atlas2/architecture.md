# System Architecture

## Core Philosophy: The "Triple Engine"

Project CCC solves the specific problem of "One browser cannot do it all" by embedding three specialized engines into a single WPF Window.

### 1. The "App" Engine (WebView2)
*   **Technology:** `Microsoft.Web.WebView2`
*   **Role:** The "Safe & Standard" viewer.
*   **Primary Use Case:** YouTube Video Player / Google Services.
*   **Why:** Google actively fights specialized browsers (like CEF). WebView2 is effectively Edge, so it passes DRM and Integrity checks easily. It is lightweight and shares the OS runtime.

### 2. The "Browser" Engine (CefSharp)
*   **Technology:** `CefSharp.Wpf.NETCore`
*   **Role:** The "Power User" browser.
*   **Primary Use Case:** General Web Browsing, Tab Management, Ad-Blocking.
*   **Why:** We need full control over the network stack (request interception), cookies, and window management (popups) which WebView2 limits.
*   **Configuration:**
    *   **Cache:** `%LocalAppData%/ccc/Cache` (Persistent).
    *   **Popups:** Forced into current tab via `CustomLifeSpanHandler`.

### 3. The "Video" Engine (MPV Native)
*   **Technology:** `libmpv` (v2)
*   **Role:** The "Raw Power" media player.
*   **Primary Use Case:** Local MKV/MP4 playback.
*   **Why:** Web browsers have poor codec support (HEVC/MKV issues) and overhead. Native MPV renders directly to the GPU bypasses the DOM entirely.

---

## Technical Implementation Details

### MPV Integration (The "Native Driver" Pattern)
Unlike standard NuGet implementations, we use a **Manual P/Invoke Driver** (`Controls/MpvNative.cs`).

*   **Problem:** The `Mpv.NET` NuGet package is outdated (2021) and looks for `mpv-1.dll`. Modern builds use `mpv-2.dll`.
*   **Solution:** We define our own `DllImport` signatures matching `client.h` from the libmpv project.
*   **Rendering:**
    1.  We define a `WindowsFormsHost` in WPF (`LocalVideoPlayer.xaml`).
    2.  We get the `.Handle` (HWND) of the hosted WinForms Panel.
    3.  We call `mpv_set_option_string(ctx, "wid", handle)` to force MPV to draw on that surface.

### CefSharp Platform Targeting
CefSharp is an unmanaged wrapper. It **requires** the application to run as `x64` specifically, not `Any CPU`.
*   **csproj Setup:** `<RuntimeIdentifier>win-x64</RuntimeIdentifier>`
*   **App.xaml.cs:** Initializes `CefSettings` with `disable-gpu-shader-disk-cache` to prevent startup lag artifacts.

### Layout Management
The layout is strictly defined in `MainWindow.xaml`:

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/> <!-- Col 0: Player (Left) -->
        <ColumnDefinition Width="*"/> <!-- Col 1: Library (Right) -->
    </Grid.ColumnDefinitions>
    
    <!-- Left Pane: Player Stack -->
    <Grid Grid.Column="0">
        <WebView2 .../>       <!-- YouTube Engine -->
        <WindowsFormsHost.../> <!-- MPV Engine -->
    </Grid>
    
    <!-- Right Pane: App Shell -->
    <Grid Grid.Column="1">
       <TopNavBar .../>       <!-- Navigation -->
       <ContentControl .../>  <!-- MVVM Pages -->
    </Grid>
</Grid>
```

This split ensures that media playback (Left) is **always** visually distinct from library management (Right), preventing the "context switching" fatigue common in single-pane apps.
