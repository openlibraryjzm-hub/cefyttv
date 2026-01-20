# Chrome Extensions & WebView2

## Overview
Recent development (Jan 2026) enabled support for standard **Chrome Extensions** within the WebView2 engine. This allows the application to support custom browser functionality like content blockers, download managers, and productivity tools, bridging the gap between a simple embedded browser and a full-featured web client.

## Architecture

### 1. The Shared Environment ("Singleton Profile")
To avoid process conflicts, initialization race conditions, and to ensure cookies/sessions are shared between the **Main Browser** and the **YouTube Embedded Player**, the application now uses a single, centralized WebView2 Environment:

*   **Definition**: `App.WebEnv` (Static property in `App.xaml.cs`).
*   **Initialization**: `App.EnsureWebViewEnvironmentAsync()`.
*   **Profile Path**: Fixed to `%LOCALAPPDATA%\CCC_WebView_Profile`.
*   **Configuration**: `AreBrowserExtensionsEnabled = true`.

**Crucial Logic**:
Any WebView2 control (**BrowserView** or **WebViewYouTubeControls**) **MUST** check for and blindly use `App.WebEnv` during its initialization. Creating a new `CoreWebView2Environment` locally will cause the app to crash if the profile directory is already locked by another instance.

### 2. Extension Loading Strategy
Extensions are loaded dynamically at runtime.

*   **Location**: `[ApplicationRoot]/Extensions/` (Copied to bin output on build).
*   **Structure**: Each extension must be its own folder containing a valid `manifest.json`.
*   **Loader**: `BrowserView.xaml.cs` iterates this directory on startup calling `AddBrowserExtensionAsync(dir)`.
    *   *Optimization*: A static flag `_extensionsLoaded` prevents the app from trying to install extensions more than once per session, which would be redundant when opening multiple tabs.

## The "Quick Download" Extension (Reference Implementation)
The first custom extension built for this system is the **Quick Download** tool.

### Functionality
*   **Trigger**: Right-click context menu on any Link, Image, Video, or Audio.
*   **Action**: Immediately downloads the target file to the user's `Downloads` folder.
*   **Behavior**: Bypasses the "Save As" dialog prompt.

### Implementation Details
1.  **Manifest (V3)**:
    ```json
    "permissions": ["contextMenus", "downloads"]
    ```
2.  **Background Script (`background.js`)**:
    *   Creates the Context Menu Item ("⚡ Quick Download (Extended)").
    *   Listens for `chrome.contextMenus.onClicked`.
    *   Calls `chrome.downloads.download({ url: ..., saveAs: false })`.
3.  **C# Host Handling (`BrowserView.xaml.cs`)**:
    *   Listens to `CoreWebView2.DownloadStarting`.
    *   **Intercepts**: Sets `e.Handled = true` to suppress the native dialog.
    *   **Routing**: Automatically creates and uses subfolders based on the current domain (e.g., `Downloads/reddit.com/file.jpg` or `Downloads/x.com/image.png`) using `WebView.Source.Host`.
    *   **Persistence**: Folders are reused if they exist.
    *   **Conflict Resolution**: Sequentially renames duplicate files (e.g., `image (1).jpg`).

## How to Add New Extensions
1.  Create a folder in `.../ccc/Extensions/[MyExtensionName]`.
2.  Place your unpacked extension files (`manifest.json`, `background.js`, icons, etc.) inside.
3.  **Build** the solution. (The `.csproj` is configured to copy the `Extensions` folder to the output directory).
4.  Restart the application.

## Troubleshooting
*   **"WebView2 was already initialized"**: This occurs if a control attempts to create a new Environment with different settings (e.g., extensions disabled) after the Global Environment has already been created. **Fix**: Ensure the control uses `App.WebEnv`.
*   **Extensions not loading**: Verify the `Extensions` folder exists in `bin/Debug/net10.0-windows/win-x64/`.
