# Interop Services (The Bridge)

**Last Updated:** 2026-01-17
**Status:** Skeleton Draft
**Parent Document:** [Architecture](architecture.md)

---

## 1. Overview

Because "Logic" lives in C# (ViewModel) but "Media" lives in WebView2/JS (YouTube), we need a robust two-way bridge.

## 2. C# -> JavaScript (Commanding)

We rely on `ExecuteScriptAsync` to drive the frontend from the backend.

**Common Patterns:**
*   `loadAppVideo(id)`: Switch video.
*   `player.playVideo()`: Play.
*   `player.pauseVideo()`: Pause.
*   `player.setVolume(n)`: Volume control.

## 3. JavaScript -> C# (Events)

We need the WebView to tell C# when things happen (e.g., video ended, error occurred).

**Mechanisms:**
1.  **`window.chrome.webview.postMessage`**: The standard WebView2 way.
2.  **C# Handlers**: `CoreWebView2.WebMessageReceived += OnMessageReceived`.

**Event Types:**
*   `STATE_CHANGE`: Playing -> Paused.
*   `PROGRESS`: Time update (current time / duration).
*   `ERROR`: Playback failure.

---

## 4. Planned Bridge implementation
*   Create a strongly typed `BridgeMessage` class (JSON serialization).
*   Standardize the message format: `{ type: "EVENT", payload: { ... } }`.
