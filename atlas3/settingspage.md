# Settings Page

## Description
The **Settings Page** (`SettingsView.xaml`) is the central control hub for configuring the application's behavior, appearance, and system integrations.

## Key Features
*   **Orb Customization**:
    *   **Image**: Upload custom PNG/JPG images for the central Orb.
    *   **Geometry**: Sliders for Scale (0.5x - 2.0x) and X/Y Offsets.
    *   **Spillover**: Toggle to allow the image to "spill" out of the circular frame.
        *   **Quadrants**: When enabled, 4 checkboxes (Top-Left, Top-Right, Bottom-Left, Bottom-Right) appear to let the user selectively choose which corners spill.
*   **System Paths**: Configuration for the SQLite database location (`playlists.db`).
*   **Appearance**: Toggles for Theme settings (though currently locked to "Sky Glass" / Dark Mode).

## UI/UX & Styling
*   **Immediate Feedback**: Interactions with the Orb Sliders (Scale/Offset) must reflect *instantly* on the Advanced Player Controller above. This "Live Preview" is critical for fine-tuning user images.
*   **Grouping**: Settings are grouped into logical `GroupBox` or `Card` containers (e.g., "Appearance", "System", "Data") to avoid a wall of inputs.
*   **Components**:
    *   **Sliders**: Styled with Sky Blue tracks and round thumbs.
    *   **Toggles**: Use modern "Switch" style toggles instead of native Checkboxes where possible.
    *   **Inputs**: File inputs should have a "Browse" button (`...`) for native OS dialogs.

## Relevant Files
### Front-End
*   `Views/SettingsView.xaml`: The UI for configuration sliders and inputs.

### Back-End
*   `Services/ConfigService.cs`: Manages reading/writing user preferences.
*   `Services/Database/AppDbContext.cs`: Database path configuration.

## Related Documentation
*   [Orb Menu](orbmenu.md): The primary visual element heavily customized by this page.
*   [Import/Export](importexport.md): Configuration here may affect where data is imported to (DB Path).
