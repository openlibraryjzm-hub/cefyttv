# Next Priority: Resolving YouTube Error 153

**Status:** Resolved
**Context:** The UI Shell, Data Layer, and Navigation are fully functional. Users can browse their real Playlists and Videos. However, attempting to play a video in the embedded WebView2 player results in **Error 153**.

## The Issue
When a video is loaded into the `WebViewYouTubeControls` (left pane), YouTube refuses playback with error code 153. This is typically a restriction related to:
1.  **Embedded Player Policies**: Configuring the player as "embedded" incorrectly.
2.  **Referer/Origin**: YouTube blocking requests from `file://` or null origins.
3.  **User Agent**: The WebView2 default user agent being flagged.
4.  **Google Account**: Failing to authenticate or restricted content issues (less likely for general failure).

## Planned Resolution Steps

### 1. Local HTML Wrapper (The "Virtual Host" Approach)
Instead of loading `https://www.youtube.com/embed/...` directly into the WebView2 Source:
*   Serve a local `player.html` file via a Virtual Host Mapping (`https://app.local/player.html`).
*   This sets a valid Origin/Referer.
*   Use the YouTube IFrame API (JS) instead of a raw IFrame tag.

### 2. Header and User Agent Modification
*   Intercept navigation request in `CoreWebView2`.
*   Inject a generic browser User Agent.
*   Set `Referer` to `https://www.youtube.com`.

### 3. Iframe Parameter Tuning
*   Ensure `origin` parameter matches the host.
*   Test specific flags: `modestbranding=1`, `rel=0`, `nocookie`.

## Success Criteria
*   Clicking a video in the list loads the player.
*   Video plays immediately (autoplay).
*   No "Video Unavailable" or "Error 153" screen.
*   Basic controls (Pause/Play) work via the IFrame API.

## Optimization: Lightning Fast Switching
To solve the slow loading times of `WebView2` for subsequent videos, we implemented a **single-page architecture** for the player:
1.  **First Load**: Navigates to `https://app.local/player.html?v=...`.
2.  **Subsequent Loads**: `WebViewYouTubeControls.cs` detects the player is already active.
3.  **Action**: Instead of reloading the page, it executes `window.loadAppVideo('NEW_ID')` via `ExecuteScriptAsync`.
4.  **Result**: Instant video swapping without tearing down the browser engine.
