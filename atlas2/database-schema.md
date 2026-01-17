# Database Schema & Data Layer

**Last Updated:** 2026-01-17
**Status:** Skeleton Draft
**Parent Document:** [Architecture](architecture.md)

---

## 1. Overview

We use **SQLite** as the local database engine, mirroring the structure of the original Rust application.

**Connection String:** `Data Source=playlists.db`

## 2. ORM: Entity Framework Core

We use **EF Core** (Microsoft.EntityFrameworkCore) for data access.

### 2.1 Context
*   **Class**: `AppDbContext`.
*   **Location**: Root namespace (currently).

### 2.2 Entities (Planned/Partial)

**`Playlist`**
*   `Id` (TEXT/UUID) - Primary Key.
*   `Name` (TEXT).
*   `Description` (TEXT).

**`PlaylistItem` (Videos)**
*   `Id` (TEXT/UUID).
*   `PlaylistId` (FK).
*   `Url` (TEXT).
*   `Title` (TEXT).
*   `ThumbnailUrl` (TEXT).

---

## 3. Migration Strategy
(To be documented: How we migrate the existing `playlists.db` from the Tauri app to the EF Core structure).
