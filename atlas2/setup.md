# Setup & Build Guide

## Prerequisites
1.  **Windows OS** (x64)
2.  **.NET 10 SDK** (Preview or Latest)
3.  **Visual Studio / VS Code**

## Critical Binaries (The "Missing Parts")

This project requires unmanaged DLLs that may not be tracked in git or standard NuGets.

### 1. mpv-2.dll
*   **Source:** [Shinchiro Builds](https://sourceforge.net/projects/mpv-player-windows/files/libmpv/) (or similiar libmpv distributions).
*   **Placement:** Must be in the **Project Root** (`c:\Projects\ccc\mpv-2.dll`).
*   **Build Action:** The `.csproj` is configured to copy this file to the output directory automatically:
    ```xml
    <None Update="mpv-2.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
    ```
*   **Troubleshooting:** If the app says "Missing mpv-2.dll" on video load, verify the file exists in `bin\Debug\net10.0-windows\win-x64\`.

## Developer Workflow (Hot Reload)

We use `dotnet watch` for nearly all UI development.

1.  Open Terminal in `c:\Projects\ccc`.
2.  Run: `dotnet watch`
3.  The application will launch.
4.  Edit `MainWindow.xaml` or `.cs` files. The app will update instantly or restart automatically.

**Note:** If you change `MpvNative.cs` or other low-level P/Invoke signatures, you may need to restart the watcher entirely to flush the unmanaged handle state.

## Common Errors

*   **`BadImageFormatException`**: You are likely trying to run an x64 DLL in an x86 host. Ensure `dotnet watch` is running as x64 (Standard on modern Windows).
*   **`DllNotFoundException: mpv-2.dll`**: The file is missing from the build folder. Check the Project Root placement.
*   **`Application` / `UserControl` Ambiguity**: Since we import `System.Windows.Forms` AND `System.Windows` (WPF), you must use fully qualified names (e.g. `System.Windows.Controls.UserControl`) in code-behind files.
