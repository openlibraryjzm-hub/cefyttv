# Orb Menu

## Description
The **Orb Menu** is the central, circular interaction hub located in the middle of the Advanced Player Controller. It serves as the visual anchor of the application.

## Key Features
*   **Radial Layout**: Features 4 satellite buttons positioned around the central image:
    *   **Top**: Upload Image (Customization).
    *   **Right**: Menu Toggle.
    *   **Left**: Search / Help.
    *   **Bottom**: Clipping / Spillover Toggle.
*   **Customization**: Supports user-uploaded images, scaling, and specific X/Y offsetting.
*   **Spillover Logic**: 
    *   **Snug Mode (Clip to Circle)**: The image is strictly clipped to the 154px circular boundary when Spillover is OFF.
    *   **Quadrants Mode (Custom Shape)**: When Spillover is ON, the clipping is dynamically calculated by `OrbSpillGeometryConverter`.
        *   **Base**: Always includes the central circle.
        *   **Quadrants**: Each selected quadrant (TL, TR, BL, BR) adds a large rectangular area to the clipping geometry, allowing the image to "spill" infinitely in that direction.
        *   **Infinite Spill**: Unlike a simple square Clip, the spill rectangles extend beyond the Orb's container bounds (`-500` to `+500`), ensuring that even highly scaled images are not cut off at the container edges in the spill direction.

## UI/UX & Styling
*   **Material**: The Orb container uses `BackdropBlur` heavily to distinguish itself from the banner behind it.
*   **Shadows**: Deep `ShadowDepth3` or `ShadowDepth4` to lift the Orb significantly above the UI plane (`ZIndex=100`).
*   **Button Style**: Satellite buttons are circular (`OrbButtonStyle`) with subtle border interactions (`Sky-200` to `Sky-500` on hover).

## Relevant Files
### Front-End
*   `Controls/Player/AdvancedPlayerController.xaml`: Hosting the Grid and Buttons.
*   `Converters/OrbSpillGeometryConverter.cs`: The core logic that Unions the Circle with 4 optional Quadrant Rectangles.

### Back-End
*   `Services/ConfigService.cs`: Persists the path to the custom Orb image (User Preference).

## Related Documentation
*   [Advanced Player Controller](advplayercontroller.md): The parent component.
*   [Settings Page](settingspage.md): The UI for configuring Orb properties (Scale, Offset, Spillover).
