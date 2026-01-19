# Developer Setup & Build Guide

## Description
This guide outlines the environment requirements and potential pitfalls when building Project CCC.

## Prerequisites
1.  **OS**: Windows 10/11 (x64).
2.  **SDK**: .NET 8.0, 9.0, or 10.0 (Preview) - Check `global.json` or `.csproj`.
3.  **IDE**: Visual Studio 2022 or VS Code.

## Critical Binary Dependencies
The project relies on **unmanaged libraries** that are often not included in git.

### `mpv-2.dll`
*   **Role**: The core engine for the Native Video Player.
*   **Placement**: MUST be in the **Project Root** (e.g., `E:\git2\ccc\mpv-2.dll`).
*   **Build Action**: The `.csproj` contains a task to copy this file to the Output Directory (`bin\Debug\...`).
*   **Missing Dilemma**: If the app launches but crashes/blacks-out on video play, this file is likely missing or blocked.

## Workflow: Hot Reload
We optimize for rapid UI iteration using `dotnet watch`.
1.  Open Terminal in Project Root.
2.  Run `dotnet watch`.
3.  Edit XAML/CS files -> App auto-updates.

**Note**: Changes to P/Invoke signature (`MpvNative.cs`) or `App.xaml.cs` (Startup) usually require a full restart.
