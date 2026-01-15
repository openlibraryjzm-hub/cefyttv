# Architecture & Setup Summary - Project CCC
**Date:** January 15, 2026
**Framework:** .NET 10 (WPF)
**State:** Functional Dual-Engine Browser Hub (Persistence & Popups Handled)

## 1. Core Workflow (Hot Reload)
The development workflow relies on `.NET Hot Reload` via the terminal.

### The Protocol:
1.  **Terminal:** Run `dotnet watch` in the project root.
2.  **Edit:** Make changes to XAML or C# files.
3.  **Update:** The running window receives updates instantly.
    *   *Note:* If the window does not update (e.g., after a crash or Rude Edit), stop the process (`Ctrl+C`), ensure no zombie processes exist (`taskkill /F /IM ccc.exe`), and run `dotnet watch` again.
4.  **Release/Debug Builds:** The `.exe` in `bin/` is a static snapshot. Do not use it for development. Use the window opened by `dotnet watch`.

---

## 2. Project Architecture
The application is a **Hybrid Media & Browser Hub** utilizing three distinct engines for specialized purposes.

### A. The "App" Engine (WebView2)
*   **Technology:** `Microsoft.Web.WebView2` (Edge/Chromium).
*   **Role:** Lightweight, secure, official Microsoft control.
*   **Usage:** Used for the "Organizer" portion (e.g., YouTube video player, UI components).

### B. The "Browser" Engine (CefSharp)
*   **Technology:** `CefSharp.Wpf` (Raw Chromium Embedded Framework).
*   **Role:** Heavy-duty, fully controllable web browser.
*   **Usage:** The "Internet Explorer" competitor. A full-featured tabbed browser.
*   **Configured Features:**
    *   **Persistence:** CachePath set to `%LocalAppData%/ccc/Cache`. Logins/Cookies survive restarts.
    *   **Single-Tab Navigation:** Popups are intercepted via `CustomLifeSpanHandler`.
    *   **Traffic Control:** `CustomRequestHandler` handles blocking.

### C. The "Video" Engine (MPV Native)
*   **Technology:** `libmpv` (v2) accessed via custom P/Invoke (`MpvNative.cs`).
*   **Role:** High-performance local media playback (MKV, MP4, WebM).
*   **Usage:** Renders local video files directly into a WPF window.
*   **Integration:**
    *   Uses `WindowsFormsHost` to provide a Win32 Handle (`HWND`) for MPV to render into.
    *   **Custom Driver:** To avoid NuGet version hell, we wrote a manual P/Invoke wrapper (`Controls/MpvNative.cs`) that directly calls `mpv_create`, `mpv_initialize`, etc. from `mpv-2.dll`.

---

## 3. Critical Setup Details

### A. CefSharp & .NET 10 Configuration
Requires `win-x64` RuntimeIdentifier in `.csproj` to locate unmanaged binaries.

### B. MPV Configuration
*   **DLL Requirement:** `mpv-2.dll` (from Shinchiro "Dev" builds) must be present in the App Root.
*   **Build Action:** `.csproj` is configured to `CopyToOutputDirectory` for `mpv-2.dll`.

### C. Dependencies
*   `Microsoft.Web.WebView2`
*   `CefSharp.Wpf.NETCore`
*   *(No NuGet for MPV - Manual P/Invoke used)*

---

## 4. Current Feature State
*   **Triple Layout:**
    *   **Left Pane:** Swappable between `WebView2` (YouTube) and `LocalVideoPlayer` (MPV).
    *   **Right Pane:** `CefSharp` Browser (Resizeable/Collapsible).
*   **Layout Modes:**
    *   **Split:** 50/50.
    *   **WV Full:** Left Pane 100%.
    *   **Cef Full:** Right Pane 100%.
    *   **WV Half:** Left Pane 50%, Right Pane Hidden.

---

## 5. Extensibility Points (Handover Notes)

### Handle Popups Strategy
Located in: `Handlers/CustomLifeSpanHandler.cs`
*   Currently set to `chromiumWebBrowser.LoadUrl(targetUrl)` (Stay in Tab).
*   To change to "New Tab" behavior, utilize the callback provided in the constructor.

### AdBlock / Injection Strategy
Located in: `Handlers/CustomRequestHandler.cs`
*   Modify `OnBeforeBrowse` to intercept URLs.
*   Example logic is already commented in the file (blocking "ads.google").

### Persistence
Located in: `App.xaml.cs` -> `OnStartup`
*   `settings.CachePath` controls where user data lives.

---

## 6. Performance Strategy
*   **Threading:** Keep heavy database (SQLite) logic off the UI thread to prevent "jank".
*   **Tab Management:** Future implementation implementation needed for "Sleeping Tabs" (disposing the `WebBrowser` control for inactive tabs while keeping metadata) to manage RAM usage.
*   **Initialization:** Cef is initialized once in `App.xaml.cs` with `disable-gpu-shader-disk-cache` optimized flags.
