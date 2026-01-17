# Media Engines (The "Triple Engine" Core)

**Last Updated:** 2026-01-17
**Status:** Skeleton Draft
**Parent Document:** [Architecture](architecture.md)

---

## 1. The Trinity

Project CCC is defined by its ability to compose three different rendering technologies into one window.

### 1.1 WebView2 (Lightweight Web)
*   **Purpose**: Secure, standard web rendering.
*   **Primary Use Case**: YouTube Embedded Player (`WebViewYouTubeControls.xaml`).
*   **Why**: It handles DRM and modern JS efficiently without the overhead of a full browser profile.
*   **Implementation**: `Microsoft.Web.WebView2.Wpf`.

### 1.2 CefSharp (Heavy Web)
*   **Purpose**: "Power User" browsing.
*   **Primary Use Case**: The persistent browser tab (Tabs, AdBlock, Extensions).
*   **Why**: Gives us deep control over network requests (`ResourceHandler`), cookies, and rendering.
*   **Implementation**: `CefSharp.Wpf.NETCore`.

### 1.3 MPV (Native Logic)
*   **Purpose**: Local Video Playback.
*   **Why**: Best-in-class codec support (ffmpeg backend), near-zero latency, low resource usage compared to web video.
*   **Implementation**: Manual P/Invoke wrapper (`MpvNative.cs`) loading `mpv-2.dll`.

---

## 2. YouTube Integration Strategy

### 2.1 The "Error 153" Solution
To bypass YouTube's embed restrictions, we use a **Local Virtual Host** strategy:
1.  **Local Asset**: We serve `assets/player.html` locally.
2.  **Origin Header**: We intercept the request (via WebView2) or serve it from `https://app.local`.
3.  **JS Interop**: We communicate with the IFrame API via `ExecuteScriptAsync`.

### 2.2 Lightning Fast Switching
We do **not** reload the WebView for every video.
1.  Load `player.html` **once**.
2.  Call `window.loadAppVideo(id)` via JS for all subsequent tracks.

---

## 3. MPV Integration Strategy
(To be documented: WinFormsHost placement, Handle passing)
