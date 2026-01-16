# Atlas Documentation Index

This directory contains comprehensive documentation for Project CCC (The "Hybrid Hub"), organized by feature and technical domain.

---

## Overview

**CCC** is a hybrid Windows Desktop application that simultaneously hosts three distinct rendering engines to achieve the best of all worlds:
1.  **WebView2 (Edge/Chromium):** for secure, lightweight web content (e.g. YouTube).
2.  **CefSharp (CEF):** for heavy-duty, fully controllable web browsing (Persistence, Tabs, AdBlock).
3.  **MPV (Native):** for high-performance localized video playback without web overhead.

The app uses **WPF** as the host container to manage the windowing, layout, and inter-process orchestration.

## Tech Stack

### Core
- **.NET 10** - Runtime
- **WPF** - UI Framework
- **C# 13** - Language

### Engines
- **Microsoft.Web.WebView2** - `WebView2` Control
- **CefSharp.Wpf.NETCore** - `ChromiumWebBrowser` Control
- **libmpv (v2)** - Accessed via manual P/Invoke (`MpvNative.cs`)

### Development Tools
- **Dotnet Watch** - Hot Reload workflow
- **Visual Studio Code** - Primary Editor

---

## Project Structure

```
ccc/
├── Controls/
│   ├── LocalVideoPlayer.xaml     # MPV Container (WinFormsHost)
│   ├── MpvNative.cs              # Manual P/Invoke Driver for mpv-2.dll
│   └── ...
├── Handlers/
│   ├── CustomLifeSpanHandler.cs  # CEF Popup Management
│   └── CustomRequestHandler.cs   # CEF Traffic Control/AdBlock
├── Views/
│   ├── TripleEngineTestView.xaml # The Main Layout Grid (Current Shell)
│   └── BrowserView.xaml          # (Optional) Standalone Browser wrapper
├── atlas2/                       # Documentation (You are here)
│   ├── architecture.md           # System design & Engine details
│   ├── ui-ux.md                  # Layouts, Controls, & Interaction
│   ├── setup.md                  # Build requirements (DLLs, Runtimes)
│   └── session-updates.md        # Change logs & "Wins"
├── MainWindow.xaml               # Host Window
├── App.xaml.cs                   # CefSharp Initialization logic
└── ccc.csproj                    # Build Config (Win-x64 targeting)
```

## Status Checklist
*   [x] **Triple Engine Setup** (WebView2 + CefSharp + MPV)
*   [x] **Database Layer** (AppDbContext exists, though UI currently disconnected)
*   [x] **Basic Shell** (TripleEngineTestView)

## Documentation Index

| Domain | Document | Description |
|--------|----------|-------------|
| **System Architecture** | `architecture.md` | Deep dive into the "Triple Engine" philosophy, CefSharp configuration, and MPV integration strategy. |
| **Setup & Build** | `setup.md` | Critical requirements (mpv-2.dll placement, RuntimeIdentifiers) and Developer Workflow. |
| **UI & UX** | `ui-ux.md` | Layout modes (Split, Full, Half), Control Bars, and Window management. |
| **Change Log** | `session-updates.md` | Chronological history of major features and fixes. |

---

## Usage Tips for AI Agents

1.  **Context is King:** Before modifying the rendering engines, read `architecture.md` to understand why we aren't using standard NuGet packages for MPV.
2.  **Layout Logic:** `MainWindow.xaml.cs` or `TripleEngineTestView.xaml.cs` controls the visibility toggles between engines. Read `ui-ux.md`.
3.  **DLLs matter:** This project relies on unmanaged binaries (`mpv-2.dll`, `libcef.dll`). Always check `setup.md` if "Module Not Found" errors occur.
