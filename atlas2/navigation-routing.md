# Navigation & Routing

**Last Updated:** 2026-01-17
**Status:** Skeleton Draft
**Parent Document:** [Architecture](architecture.md)

---

## 1. Overview

In WPF, "Navigation" is effectively "View Switching". We do not use a URL-based router like react-router; instead, we switch the `Content` property of a `ContentControl`.

## 2. The Navigation Service

We implement a `NavigationService` (or handle it in `MainViewModel`) that exposes:
*   `NavigateTo(ViewModel vm)`
*   `GoBack()`

### 2.1 View Mapping
We rely on `DataTemplates` in `App.xaml` or `MainWindow.xaml` to tell WPF how to render each ViewModel.

**Example Pattern:**
```xml
<DataTemplate DataType="{x:Type vm:PlaylistsViewModel}">
    <views:PlaylistsView />
</DataTemplate>
```

---

## 3. The Back Stack
To support the "Back" button:
1.  Maintain a `Stack<ViewModel> _history`.
2.  Push to stack on Navigate.
3.  Pop from stack on GoBack.
