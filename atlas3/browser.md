---
description: Documentation for the WebView2 Browser Implementation
---
# WebView2 Browser

## Overview
The **WebView2 Browser** provides a robust, multi-tabbed web browsing experience within the application. It runs on the Microsoft Edge WebView2 engine, sharing a consistent environment/profile with the embedded YouTube player to ensure session persistence (cookies, logins) across the entire application.

## Architecture

### 1. Tab Management
Tabs are managed using a **Visibility-Based Switching** mechanism rather than a standard `TabControl`.
*   **Standard `TabControl` Issue**: In WPF, switching tabs within a standard `TabControl` unloads the visual tree of the hidden tab. For a `WebView2` control, this destroys the underlying browser process or detaches it, causing crashes and state loss.
*   **Solution**: The application uses a custom container (Grid) where *all* active WebView instances remain loaded in the visual tree. Only the *selected* tab's WebView is set to `Visibility.Visible`; others are `Visibility.Collapsed`. This preserves the state of background tabs perfectly.

### 2. ViewModel-Driven State
The browser state is managed by `BrowserViewModel`:
*   **Tabs**: An `ObservableCollection` of `BrowserTabViewModel`.
*   **ActiveTab**: The currently selected tab.
*   **Commands**: `NewTab`, `CloseTab`, `Navigate`, `GoBack`, `GoForward`, `Refresh`.

### 3. Styling
The browser interface is designed to match the application's "Premium" aesthetic:
*   **Tabs**: Styled like modern browser tabs (Chrome/Edge style) with close buttons and clear active states.
*   **Address Bar**: A sleek, rounded input field with an integrated search/go button.
*   **Controls**: Minimalist Back/Forward/Refresh icons.

## Extensions
As documented in [extensions.md](extensions.md), the browser supports loading unpacked Chrome extensions from the `Extensions/` directory.
*   **Shared Profile**: All tabs share the global `App.WebEnv` environment.
*   **Loading**: Extensions are registered ensuring they are loaded only once per session profile.

## Fullscreen & Layout Modes
The browser supports two distinct viewing modes, controlled by the toolbar buttons:
1.  **Split Screen (Default)**:
    *   The browser occupies the **Right Half** of the application window.
    *   The **Top Navigation Bar** (Playlists, Videos, Settings) remains visible above the browser.
    *   The **Left Half** continues to show the Media Player.
2.  **Fullscreen Browser**:
    *   **Toggle**: Click the `⛶` button on the far left of the tab strip.
    *   **Behavior**: The browser expands to cover the entire application window (Row 0 & 1, Col 0 & 1).
    *   **Interaction**: The Top Navigation Bar is hidden to provide maximum browsing space.
    *   **Z-Index**: Set to `300` to overlay the Player and App Banner.

## Usage
*   **New Tab**: Click the `+` button in the scrollable tab strip.
*   **Close Tab**: Click the `x` on a tab.
*   **Navigation**: Enter a URL or search query in the address bar.
*   **Close Browser**: Click the **Red Cross (❌)** on the far right. This closes the browser entirely and maximizes the YouTube player.
*   **Shortcuts**:
    *   `Ctrl+T`: New Tab (Planned)
    *   `Ctrl+W`: Close Tab (Planned)

## Troubleshooting
*   **Tabs not appearing**: Ensure `BrowserViewModel` is properly bound.
*   **White Screen**: If a tab shows a white screen, the WebView2 process may have crashed. Try closing and reopening the tab.
*   **Unclickable Top Bar**: If the top bar (Tabs/URL) becomes unclickable in Fullscreen, ensure `WindowChrome.IsHitTestVisibleInChrome="True"` is set on the containing Border.
