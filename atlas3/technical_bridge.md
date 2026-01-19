# Interop Services & The Bridge

## Description
Because the "Brain" of the app is C# (ViewModel) but the "Media" often lives in JavaScript (WebView2/YouTube), we require a robust **Bidirectional Bridge**.

## The Architecture

### 1. The Virtual Host (Solving "Error 153")
YouTube blocks most `file://` protocols and embedded UserAgents. We solve this by serving the player from a fake local domain.
*   **Host**: `https://app.local/`
*   **Asset**: Serves `assets/player.html` (Local file) via `SetVirtualHostNameToFolderMapping`.
*   **Benefit**: Sets a valid `Origin` and `Referer`, bypassing YouTube's anti-embed blocks.

### 2. C# -> JavaScript (Commanding)
We drive the player by injecting JavaScript.
*   **Method**: `WebView2.CoreWebView2.ExecuteScriptAsync()`.
*   **Commands**:
    *   `loadAppVideo(id)`: Seamlessly swaps the video without reloading the iframe.
    *   `player.playVideo()`, `player.pauseVideo()`: Playback control.

### 3. JavaScript -> C# (Events)
The WebView notifies C# of playback state changes.
*   **Method**: `window.chrome.webview.postMessage(json)`.
*   **Listener**: C# `WebMessageReceived` event.
*   **Events**:
    *   `STATE_CHANGE`: (Playing/Paused/Buffering) -> Updates `MainViewModel.IsPlaying`.
    *   `PROGRESS`: (CurrentTime / Duration) -> Updates `MainViewModel.Progress`.
