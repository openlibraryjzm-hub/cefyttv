# Mission Brief: Rapid Frontend Fidelity (React -> WPF)

## 🎯 The Objective
The "Backbone" (Database) and "Body" (Shell/Player) are complete. Your mission is to give this application its "Face".
You must translate the original React/CSS design from the `imported-project` directly into WPF XAML, making it look and feel like the original high-end application.

## ⚠️ CRITICAL WARNING: The "Golden" Embedded Player
**DO NOT MODIFY** the `InitializeWebView` logic in `MainWindow.xaml.cs`.
*   We use a sophisticated **Virtual Host Mapping** (`https://app.local` -> local `assets/` folder) to bypass YouTube's strict embed blocking (Error 153).
*   Any changes to `SetVirtualHostNameToFolderMapping` or the `assets/player.html` file could break the core video playback feature.

## 🛠 Active Tasks

### 1. Visual Fidelity (The "Look")
The current `Views/PlaylistsView.xaml` and `Views/VideosView.xaml` are functional but visually primitive (basic debug styling).
*   **Source of Truth:** `e:\git2\ccc\imported-project\components\`
    *   Match `PlaylistCard.jsx` styling in `Controls/Cards/PlaylistCard.xaml`.
    *   Match `VideoCard.jsx` styling in `Controls/Cards/VideoCard.xaml`.
    *   Match the Grid layouts (spacing, background colors, scrollbars).
*   **Design Language:** Dark Mode, rounded corners, specific hover effects (glowing borders), subtle animations. Use the `atlas` documentation for visual reference if available.

### 2. Navigation Logic (The "Flow")
Currently, clicking a Playlist Card navigates to the Videos Page, but it shows **ALL** videos in the database.
*   **Fix:** Implement logic to pass the `PlaylistId` to the `VideosViewModel`.
*   **Requirement:** When a user clicks "Music Mix" playlist, `VideosView` must **only** show videos belonging to "Music Mix".

### 3. Missing UI Elements
The following elements from the original app are currently missing in the WPF port:
*   **"Three-Dot" Context Menus** on cards.
*   **Banner/Header** on the Videos page (showing Playlist Title, "Play All", "Shuffle" buttons).
*   **Shifting/Sorting**: Drag-and-drop support (Phase 5, but keep it in mind).

## 📂 Key Files
*   `e:\git2\ccc\Views\PlaylistsView.xaml` (The Hub)
*   `e:\git2\ccc\Views\VideosView.xaml` (The List)
*   `e:\git2\ccc\Controls\Cards\PlaylistCard.xaml` (The Unit)
*   `e:\git2\ccc\MainWindow.xaml.cs` (The Engine - **READ ONLY**)

## 💡 Tips for React -> XAML Translation
*   `div` with `display: flex` -> `prolly Grid` or `StackPanel`.
*   `border-radius` -> `CornerRadius` (Border).
*   `box-shadow` -> `DropShadowEffect`.
*   CSS Variables -> define as `SolidColorBrush` resources in `App.xaml` for global consistency.

**Good luck, Agent. Make it beautiful.**
