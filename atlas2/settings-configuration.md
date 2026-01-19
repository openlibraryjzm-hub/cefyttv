# Settings & Configuration

**Last Updated:** 2026-01-18
**Status:** Planned / Skeleton
**Parent Document:** [Architecture](architecture.md)

---

## 1. Overview

Because this is a local-first application, user preferences must be persisted to disk. 
In the previous React/Tauri app, this was handled by `localStorage` or `tauri-plugin-store` (`settings.json`).

In WPF, we will use a dedicated `ConfigService` that serializes a JSON file.

## 2. Storage Location

*   **File:** `%AppData%\ccc\config.json` (or `%LocalAppData%`)
*   **Format:** JSON (System.Text.Json)

## 3. Configuration Schema (Planned)

We plan to implement a strongly-typed C# class `AppConfig`:

```csharp
public class AppConfig
{
    // Appearance
    public bool DarkMode { get; set; } = true;
    public double ZoomLevel { get; set; } = 1.0;

    // Player
    public float DefaultVolume { get; set; } = 0.5f;
    public bool Autoplay { get; set; } = true;
    
    // System
    public string DownloadsPath { get; set; } = "";
    public bool HardwareAcceleration { get; set; } = true;

    // Orb Customization
    public string? CustomOrbImage { get; set; }
    public double OrbScale { get; set; } = 1.0;
    public double OrbOffsetX { get; set; } = 0;
    public double OrbOffsetY { get; set; } = 0;
    public bool IsSpillEnabled { get; set; } = true;
    public bool SpillTopLeft { get; set; }
    public bool SpillTopRight { get; set; }
    public bool SpillBottomLeft { get; set; }
    public bool SpillBottomRight { get; set; }
}
```

## 4. Implementation Strategy

1.  **Service:** `Services/ConfigService.cs`
    *   `LoadAsync()`: Read JSON or return defaults.
    *   `SaveAsync()`: Write JSON.
2.  **ViewModel Integration:**
    *   `MainViewModel` loads config on startup.
    *   Exposes properties enabling UI binding (e.g., `<Slider Value="{Binding Config.DefaultVolume}" />`).
3.  **Hot-Saving:**
    *   Use `PropertyChanged` events to trigger auto-save (debounced).

---

## 5. Current Status (Migration Note)
*   The `tabs.json` file currently exists in the root directory (legacy from Tauri).
*   We need to consolidate `tabs.json`, `settings.json`, and other loose config files into this central `ConfigService`.
