# Advanced Player Controller

## Description
The **Advanced Player Controller** (`AdvancedPlayerController.xaml`) is the application's primary "Head-Up Display" (HUD). Occupying the top 200px of the window, it provides immediate access to media controls, navigation context, and visualization.

## Key Features
*   **Structure**: 3-Column "Floating Wings" design.
    *   **Left Wing**: Playlist Menu (Context, Navigation).
    *   **Center**: The Orb (See `orbmenu.md`).
    *   **Right Wing**: Video Menu (Title, Playback Controls, Tools).
*   **Visuals**: Uses "Sky Glass" aesthetics—semi-transparent white/blue panels floating above the `App Banner`.
*   **Z-Index**: Placed at `Panel.ZIndex="100"` to float above all other content.
*   **Context Sync**: Automatically detects the playlist context of the currently playing video to ensure "Next/Prev" buttons work logically.

## UI/UX & Styling
*   **"Glass" Aesthetics**: The menus use semi-transparent white/blue backgrounds (opacity ~0.95) with blur effects to float over the `App Banner`.
*   **Typography**: Uses bold, uppercase metadata labels (`#0EA5E9`, Sky-500) against dark text for high readability.
*   **Animations**: Buttons should scale slightly on Hover. Navigation arrows fade in/out based on availability.

## Relevant Files
### Front-End
*   `Controls/Player/AdvancedPlayerController.xaml`: Main XAML implementation.
*   `Controls/Buttons/OrbButtonStyle.xaml` (Hypothetical): Styles for the specific circular buttons.

### Back-End
*   `ViewModels/MainViewModel.cs`: Holds `SelectedVideo` and `SelectedPlaylist` state.
*   `Services/ShuffleService.cs`: Logic for the Shuffle button.
*   `Services/PinService.cs`: Logic for the Pin button.

## Related Documentation
*   [Orb Menu](orbmenu.md): The central visual component of this controller.
*   [Likes Page](likespage.md): Destination for the Heart button's Right-Click action.
*   [History Page](historypage.md): Destination for the Video Grid button's Right-Click action.
*   [App Banner](appbanner.md): The background layer this controller "floats" over.
*   [Backend: State](backend_state.md): The Controller binds directly to the Global Store (`MainViewModel`).
*   [Backend: Media Engines](backend_engines.md): The Controller sends Play/Pause commands to these engines.
*   [Technical: Bridge](technical_bridge.md): Logic for sending commands to the WebView2 player.
*   [UI: Layout](layout.md): Explains the "Floating HUD" concept in Row 0.
