# Audio Visualizer

## Description
The **Audio Visualizer** is a dynamic graphic component that encircles the `Orb Menu` in the Player Controller. It sends real-time audio data to a visual ring of bars.

## Key Features
*   **Purpose**: To provide real-time visual feedback based on system audio (WASAPI Loopback).
## Implementation Details
*   **Backend**: 
    *   File: `Services/Audio/AudioVisualizerService.cs`
    *   Library: `NAudio` (WasapiLoopbackCapture).
    *   Logic: Captures system audio, performs FFT, maps to 113 frequency bins.
    *   **Tuning Parameters**:
        *   `_smoothing`: (Default 0.75) Controls how quickly bars fall. Higher = slower/smoother.
        *   `_preAmp`: (Default 4.0) Gain multiplier before sending to UI.
        *   `_minFreq` / `_maxFreq`: Frequency range to visualize.
*   **Frontend**: 
    *   File: `Controls/Visuals/AudioVisualizer.xaml.cs`
    *   Logic: Renders 113 `Line` elements in a radial pattern on a Canvas.
    *   **Scaling**: Input data (approx 0.0-0.3) is multiplied by `5.0` to fill the visual range (0.0-1.0).
*   **Integration**:
    *   `MainViewModel`: Initializes service based on `AppConfig`.
    *   `AdvancedPlayerController`: Hosts the control behind the Orb with `Panel.ZIndex="-1"`.

## Usage
1.  Click the "..." button on the right side of the Player Controller.
2.  Check/Uncheck "Toggle Audio Visualizer".

## Tuning Guide (For Developers)
To adjust the feel of the animation:
1.  **Responsiveness**: Adjust `_smoothing` in `AudioVisualizerService.cs`. Lower values (0.5) make it twitchy/fast; higher values (0.9) make it slow/fluid.
2.  **Sensitivity**: Adjust `val = data[i] * 5.0` in `AudioVisualizer.xaml.cs`. Increase multiplier for more movement at low volume.
3.  **Frame Rate**: UI updates are currently throttled by the Service event rate (buffer size). Smaller `_fftSize` (e.g. 1024) might yield faster updates but less frequency resolution.
