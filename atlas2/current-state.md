# Current State of the Application

**Last Updated:** 2026-01-18
**Status:** Alpha / Wiring Phase
**System of Record:** This document is the source of truth for the current visual and functional state of the application.

---

## 1. High-Level Summary

The application is in a **Barebones / Functional Skeleton** state. 
While the core architecture (WPF + SQLite + Triple Engine Shell) is established and data migration for major tables (Playlists/Videos) is successful, the UI lacks significant styling, polish, and interactivity compared to the previous Rust/Tauri iteration.

### 1.1 Styling & Aesthetics
*   **Status**: ❌ Missing / Extremely Barebones.
*   **Issue**: Styles are basic WPF defaults or minimal custom styles. The distinct "Premium/Glassmorphic" look of the Rust app has not yet been ported.

---

## 2. Layout & Shell

### 2.1 Main Window Structure
*   **Top Strip (Rows 0, Height 102px)**: Dedicated "Advanced Player Controller" space.
    *   **Center**: The Orb (Visual placeholder present).
    *   **Left**: Playlist Menu (Placeholder titles/metadata).
    *   **Right**: Video Menu (Placeholder titles/metadata).
    *   **Interactivity**: All buttons (30+) are non-wired placeholders.
*   **Split View (Row 1)**:
    *   **Left Half**: Video Player (WebView2/MPV container).
    *   **Right Half**: Navigation/Content Pane.

### 2.2 Navigation (Right Pane)
*   **Mechanism**: Top Navigation Bar allows switching between views.
*   **Active Pages**:
    *   `PlaylistsView` (Home)
    *   `VideosView` (Drill-down from Playlist)
    *   `HistoryView` (Data missing)
    *   `LikesView` (Data missing)
    *   `PinsView` (Data missing)
    *   `SettingsView` (Placeholder)
    *   `SupportView` (Placeholder)
*   **Top Nav Controls**:
    *   Setup: Back, Scroll Top, Scroll Active, Scroll Bottom, Close Menu.
    *   Status: **Present but Broken** (Non-functional).
*   **Missing**: 
    *   **Browser Toggle**: No button exists on the Top Nav to switch to the CefSharp browser layer.

---

## 3. Core Features & Data

### 3.1 Database Integration
*   ✅ **Success**: `Playlists` and `Videos` tables migrated successfully.
*   ✅ **Success**: Real data (Titles, Video Counts, Thumbnails) appears in the UI.
*   ❌ **Failure**: `History`, `Pins`, `Likes` data did not port over (Acceptable loss).

### 3.2 Playlists Page (`PlaylistsView`)
*   **Banner**: Present but super minimal.
*   **Data Issue**: Banner does not correctly reflect the selected playlist's info.
*   **Controls**: Validation missing (Play/Shuffle buttons on cards are non-functional).
*   **Interaction**: Navigation works only by clicking the Thumbnail/Cover Image (Drill-down).

### 3.3 Videos Page (`VideosView`)
*   **Banner**: Present but exhibits data mismatch (shows Playlist info incorrectly or placeholder).
*   **Controls Bar**: 
    *   Contains: Colored Folders, Filters (Sort: Default), Bulk Tag.
    *   Status: **Non-functional** and heavily reduced feature set vs Tauri.
*   **Thumbnails**: Hover buttons appear but are non-functional.

---

## 4. Known Issues & Missing Functionality

1.  **Wiring**: 90% of buttons (Nav bar, Player Controller, card actions) are visual only.
2.  **Logic**: Valid filtering/sorting logic is missing from the Videos page.
3.  **Visuals**: No glow effects, no animations, basic fonts.
4.  **Browser**: The CefSharp browser logic exists in code but is unreachable via UI.

## 5. Summary State
*   **Database**: Loaded & Connected.
*   **UI**: Representative skeleton of the old app.
*   **UX**: "Click-through" prototype only; actual logic/media control is pending.
