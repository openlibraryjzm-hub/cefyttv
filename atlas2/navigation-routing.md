# Navigation & Routing

**Last Updated:** 2026-01-18
**Status:** Implemented (Basic)
**Parent Document:** [Architecture](architecture.md)

---

## 1. Overview

In the Triple Engine Architecture, "Navigation" refers to two distinct concepts:
1.  **View Navigation**: Switching between XAML UserControls (Playlists -> Videos -> Settings).
2.  **Engine Toggling**: Switching between "Library Mode" (WPF) and "Browser Mode" (CefSharp).

Reference Implementation: `ViewModels/MainViewModel.cs`.

## 2. View Navigation (The "Router")

Unlike Web routers (URL-based), WPF uses **Content Switching**.

### 2.1 The Mechanism
*   **Host**: `MainWindow.xaml` contains a `ContentControl` bound to `CurrentView`.
*   **Trigger**: `MainViewModel.Navigate(string destination)`.
*   **Resolution**: A switch statement instantiates the requested UserControl.

```csharp
// ViewModels/MainViewModel.cs
public void Navigate(string destination)
{
    switch (destination.ToLower())
    {
        case "playlists": CurrentView = new PlaylistsView(); break;
        case "videos":    CurrentView = new VideosView();    break;
        // ...
    }
}
```

### 2.2 Data Templates
WPF knows how to render these Views because they are standard UserControls. In the future, if we use strict "ViewModel-First" navigation, we will define DataTemplates in `App.xaml`:

```xml
<DataTemplate DataType="{x:Type vm:PlaylistsViewModel}">
    <views:PlaylistsView />
</DataTemplate>
```

Currently, we are using "View-First" instantiation (`new PlaylistsView()`) inside the ViewModel for simplicity in Phase 1.

## 3. Engine Toggling (The "Layer")

The **CefSharp Browser** is distinct because it is **heavy**. We cannot destroy/recreate it like we do with lightweight XAML Views.

### 3.1 The Visibility Toggle
The Browser is always alive in the Visual Tree (inside `TripleEngineTestView` or `MainWindow`). We simply toggle its Visibility/Z-Index.

*   **Property**: `MainViewModel.IsBrowserVisible` (bool).
*   **Binding**:
    *   If `true`: Browser Grid `Visibility="Visible"`.
    *   If `false`: Browser Grid `Visibility="Collapsed"`.

This ensures that when a user "navigates away" from the web, their tabs/session remain intact in memory.

## 4. The Back Stack strategy (Todo)

Currently, navigation is linear (Menu -> Page). To support a "Back" button:
1.  **Stack**: `Stack<object> _navigationHistory`.
2.  **Push**: On `Navigate()`, push current `CurrentView` type/state.
3.  **Pop**: On `GoBack()`, pop and restore.

*Status: Pending Implementation.*
