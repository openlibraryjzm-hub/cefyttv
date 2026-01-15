# UI & UX Documentation

## Layout System
The main window uses a **Grid-based layout** defined in `MainWindow.xaml`.

### The "Control Bar"
*   **Location:** Bottom Row (Grid.Row="1").
*   **Role:** Persistent navigation and mode switching.
*   **Buttons:**
    *   **WV Full:** Maximizes the WebView2 (YouTube) Pane.
    *   **Split:** Restores standard side-by-side view.
    *   **Cef Full:** Maximizes the Browser Pane.
    *   **Test MPV:** Toggles MPV visibility (Debug).
    *   **Open File:** Opens system file picker to load local video into MPV.

### The Pane System
The application is divided into two primary vertical columns.

#### Left Pane (Column 0)
*   **Content:** Stacked `WebView2` and `LocalVideoPlayer`.
*   **Logic:** Only one is visible at a time.
    *   **Default:** `YoutubeView` (WebView2) is visible.
    *   **Video Mode:** `MpvView` (LocalVideoPlayer) becomes visible, hiding YouTube.
*   **Sizing:** 
    *   In **Split Mode**, takes 1* width.
    *   In **WV Full**, takes entire width (ColumnSpan 2).

#### Right Pane (Column 1)
*   **Content:** `ChromiumView` (CefSharp).
*   **Logic:** Handles multiple tabs via custom button logic.
*   **Sizing:**
    *   In **Split Mode**, takes 1* width.
    *   In **Cef Full**, takes entire width (ColumnSpan 2) while Left Pane is collapsed.

## Interaction Patterns

### Opening Local Videos
1.  User clicks **"Open File"** (Green Button).
2.  Windows File Dialog appears.
3.  Upon selection:
    *   Left Pane switches to **MPV Mode**.
    *   MPV loads the file.
    *   Status Text below the player confirms "Playing: filename".

### Switching Engines
*   The UI allows instant swapping between **YouTube** (Web) and **MPV** (Local).
*   **Note:** Switching *away* from MPV calls `Stop()` to ensure audio doesn't keep playing in the background.
