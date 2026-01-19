# Database Schema & Data Layer

## Description
This document details the **SQLite** schema acting as the persistence layer for Project CCC. It mirrors the legacy Rust/Diesel schema to ensure backward compatibility with existing `playlists.db` files.

## Files
*   **Database**: `playlists.db` (Application Root)
*   **ORM**: `Services/Database/AppDbContext.cs` (EF Core Context)
*   **Entities**: `Models/Entities/*.cs` (Playlist, PlaylistItem, WatchHistory, etc.)

## Schema Overview

### 1. `playlists`
The core collection unit.
*   `id` (PK, AutoInc)
*   `name` (Text, Required)
*   `last_watched_video_id` (Text, Nullable): Resume point.

### 2. `playlist_items`
Videos linked to playlists (One-to-Many).
*   `video_url` / `video_id` (Identifiers)
*   `position` (Sort Order)
*   `is_local` (Boolean: 0=Web, 1=Local)

### 3. `watch_history`
Chronological playback log.
*   `video_id`, `watched_at` (Timestamp)

### 4. `video_progress`
Resume points for individual videos.
*   `video_id`, `last_progress` (Seconds), `progress_percentage` (0.0-100.0)

### 5. `video_folder_assignments`
Tagging system for videos.
*   `playlist_id`, `item_id`, `folder_color` (e.g., "red", "blue")

## Relationships
*   **Entity Framework Core** manages relationships via standard foreign keys.
*   Access is mediated strictly through **Services** (e.g., `PlaylistService`), not direct context calls from the UI.
