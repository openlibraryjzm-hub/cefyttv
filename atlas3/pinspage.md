# Pins Page

## Description
The **Pins Page** (`PinsView.xaml`) serves as a "Quick Access" board for high-priority content. It is designed for videos or playlists that the user wants to keep immediately available, distinct from the general "Likes" pool.

## Key Features
*   **Mechanism**: Populated by the "Pin" action in the Advanced Player Controller.
*   **User Intent**: Intended for "Stuck" items or current focus content.
*   **Access**: Accessible via the "Pin" icon in the Top Navigation or via Right-Click on the Pin button in the Player Controller.

## UI/UX & Styling
*   **Purpose-Driven**: This view is often smaller than the main library.
*   **Visual Difference**: While it uses the standard `VideoCard`, this page represents a "Manual Selection" rather than an "Algorithmic Log" (History) or "Bucket" (Likes).
*   **Layout**: Maintains the `UniformGrid` consistency for seamless transition from other pages.

## Relevant Files
### Front-End
*   `Views/PinsView.xaml`: The view for pinned items.

### Back-End
*   `Services/PinService.cs`: Service handling the logic for pinning/unpinning items.

## Related Documentation
*   [Advanced Player Controller](advplayercontroller.md): Contains the Pin button that populates this page.
*   [Top Navigation Bar](topnavigationbar.md): Contains the direct link (Pin icon) to this page.
