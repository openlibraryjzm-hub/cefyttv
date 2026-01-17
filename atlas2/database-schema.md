# Database Schema & Data Layer

**Last Updated:** 2026-01-18
**Status:** Implemented
**Parent Document:** [Architecture](architecture.md)

---

## 1. Overview

The application uses **SQLite** as its persistent storage engine. The schema mirrors the original Diesel (Rust) migrations to ensure compatibility with existing `playlists.db` files.

*   **Database File:** `playlists.db` (in App Root)
*   **ORM:** Microsoft Entity Framework Core (EF Core)
*   **Context:** `Services.Database.AppDbContext`

---

## 2. Table Definitions

### 2.1 `playlists`
Stores the high-level collections.

| Column | Type | Attributes | Description |
| :--- | :--- | :--- | :--- |
| `id` | `INTEGER` | `PK`, `AutoInc` | Unique ID. |
| `name` | `TEXT` | `Required` | Display name of the playlist. |
| `description` | `TEXT` | `Nullable` | Optional user description. |
| `created_at` | `TEXT` | `Required` | ISO 8601 Timestamp. |
| `updated_at` | `TEXT` | `Required` | ISO 8601 Timestamp. |
| `custom_ascii` | `TEXT` | `Nullable` | ASCII Art decoration. |
| `custom_thumbnail_url` | `TEXT` | `Nullable` | Override image. |

### 2.2 `playlist_items`
Stores the videos associated with a playlist. This is a one-to-many relationship (Playlist -> Items).

| Column | Type | Attributes | Description |
| :--- | :--- | :--- | :--- |
| `id` | `INTEGER` | `PK`, `AutoInc` | Unique ID. |
| `playlist_id` | `INTEGER` | `Required`, `FK` | Links to `playlists.id`. |
| `video_url` | `TEXT` | `Required` | Platform URL or File Path. |
| `video_id` | `TEXT` | `Required` | YouTube ID or Hash. |
| `title` | `TEXT` | `Nullable` | Cached Video Title. |
| `thumbnail_url`| `TEXT` | `Nullable` | Cached Image URL. |
| `author` | `TEXT` | `Nullable` | Channel Name / Creator. |
| `view_count` | `TEXT` | `Nullable` | String representation (e.g. "1.2M"). |
| `position` | `INTEGER` | `Required` | Sort order definition (0-indexed). |
| `added_at` | `TEXT` | `Required` | ISO 8601 Timestamp. |
| `is_local` | `INTEGER` | `Required` | Boolean flag (0=Web, 1=LocalFile). |
| `published_at` | `TEXT` | `Nullable` | Original upload date. |

### 2.3 `watch_history`
Tracks playback events for "Resume" and "History" features.

| Column | Type | Attributes | Description |
| :--- | :--- | :--- | :--- |
| `id` | `INTEGER` | `PK`, `AutoInc` | Unique ID. |
| `video_id` | `TEXT` | `Required` | The Video Identifier. |
| `video_url` | `TEXT` | `Required` | Source location. |
| `title` | `TEXT` | `Nullable` | Cached Title. |
| `thumbnail_url`| `TEXT` | `Nullable` | Cached Thumbnail. |
| `watched_at` | `TEXT` | `Required` | Timestamp of access. |

### 2.4 `video_progress`
Stores "Resume Playback" positions.

| Column | Type | Attributes | Description |
| :--- | :--- | :--- | :--- |
| `id` | `INTEGER` | `PK` | Unique ID. |
| `video_id` | `TEXT` | `Required` | Target Video. |
| `video_url` | `TEXT` | `Required` | Target URL. |
| `duration` | `REAL` | `Nullable` | Total length in seconds. |
| `last_progress` | `REAL` | `Required` | Last known timestamp (seconds). |
| `progress_percentage` | `REAL` | `Required` | 0.0 to 100.0. |
| `last_updated` | `TEXT` | `Required` | Timestamp. |
| `has_fully_watched` | `INTEGER` | `Required` | 0 or 1. |

### 2.5 `video_folder_assignments`
Maps specific videos (within specific playlists) to a "Folder Color".

| Column | Type | Attributes | Description |
| :--- | :--- | :--- | :--- |
| `id` | `INTEGER` | `PK` | Unique ID. |
| `playlist_id` | `INTEGER` | `FK` | Parent Playlist. |
| `item_id` | `INTEGER` | `FK` | Specific PlaylistItem. |
| `folder_color` | `TEXT` | `Required` | e.g., "red", "blue", "green". |
| `created_at` | `TEXT` | `Required` | Timestamp. |

---

## 3. Data Access Layer (Services)

Access to these tables is mediated by `Service` classes, never accessed directly by ViewModels.

*   `PlaylistService`: Manages `playlists` and `playlist_items`. handles "Add to Playlist", "Reorder", "Remove".
*   `VideoService` (Planned): Will manage `video_progress` and `watch_history`.
*   `FolderService`: Manages `video_folder_assignments`.

## 4. Migration Notes
*   **Legacy Compatibility**: The schema uses `INTEGER` for Booleans (0/1) and `TEXT` for Timestamps (ISO Strings) to match the SQLite limitations and the previous Rust Diesel implementation.
*   **Foreign Keys**: EF Core is configured (via Data Annotations) to recognize `playlist_id` relationships.
