# Navigation & Routing

## Description
Unlike web apps with URL-based routing, Project CCC uses **WPF Content Switching** to handle navigation between views.

## The Router Mechanism
*   **Host**: `MainWindow.xaml` contains a `ContentControl` bound to the `CurrentView` property of the `MainViewModel`.
*   **Trigger**: `MainViewModel.Navigate(string destination)`.
*   **Logic**: A simple switch statement instantiates the requested UserControl (e.g., `new VideosView()`) and assigns it to `CurrentView`.

## Engine Toggling (The Layer Switch)
Navigation also involves switching between the **WPF Layer** (Library) and the **CefSharp Layer** (Browser).
*   **Property**: `MainViewModel.IsBrowserVisible` (bool).
*   **Behavior**:
    *   `True`: The Browser Grid `Visibility` is set to `Visible`, covering the WPF Library.
    *   `False`: The Browser is hidden (`Collapsed`), revealing the WPF Library.
*   **Benefit**: This keeps the Browser session (tabs, scroll position) alive in memory even when the user is "in" the Library.

## Related Documentation
*   [UI: Layout](layout.md): Details the Z-Index layering that makes engine toggling possible.
