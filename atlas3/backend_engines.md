# Media Engines (The "Triple Engine" Core)

## Description
Project CCC is defined by its hybrid architecture, simultaneously running three distinct rendering engines to achieve optimal playback and browsing.

## The Engines

### 1. WebView2 (The Unified Web)
*   **Tech**: Edge/Chromium.
*   **Role A**: **YouTube Embedded Player** ("Protected Web").
    *   **Reasoning**: Best compatibility with DRM, Codecs, and anti-scraping scripts.
*   **Role B**: **Full Browser Mode** ("Tabbed Web").
    *   **Reasoning**: Replaced CefSharp to provide full H.264 video support, persistent logins, and native downloads.
    *   **Features**: Auto-downloads to "Downloads" folder, [Extensions Support](extensions.md) (Context Menu tools), and Tab/Popup management.
    *   **Infrastructure**: Runs on a **Shared Singleton Environment** (`App.WebEnv`) to ensure cookies and sessions are shared with the YouTube Player.

### 2. MPV (The Native Power)
*   **Tech**: FFmpeg / `libmpv`.
*   **Role**: **Local Video Playback**.
*   **Reasoning**: Zero-latency, hardware-accelerated playback of local files (`.mp4`, `.mkv`) without browser overhead.

## Wiring
All three engines are layered using WPF's `Grid` system and `Panel.ZIndex`.
*   **WebView2/MPV**: Usually reside in the "Player" column of the grid.
*   **CefSharp**: Generally sits top-level but invisible until "Browser Mode" is triggered.

## Related Documentation
*   [Technical: Startup](technical_startup.md): Details the initialization sequence for these engines.
*   [Technical: Bridge](technical_bridge.md): Specifies how the C# layer communicates with the WebView2 engine.
*   [Developer Setup](dev_setup.md): Notes on the `mpv-2.dll` requirement.
*   [UI: Layout](layout.md): Details the Grid Column placement for these engines.
