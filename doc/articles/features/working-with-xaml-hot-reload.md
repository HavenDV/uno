---
uid: Uno.Features.HotReload
---

# Hot Reload

The Uno Platform Hot Reload feature provides a way to modify the XAML and C# of your running application, in order to iterate faster on UI or code changes. This makes the inner developer loop faster.

## Features

- Supported in **Visual Studio 2022** (Windows) and **VS Code** (Linux, macOS, Windows, CodeSpaces, and GitPod)
- XAML and [C# Markup](xref:Uno.Extensions.Markup.Overview) Hot Reload for **iOS, Catalyst, Android, WebAssembly, and Skia (X11, Windows, macOS and FrameBuffer)**
- All **[C# of Hot Reload](https://learn.microsoft.com/visualstudio/debugger/hot-reload)** in both Visual Studio and VS Code. See [supported code changes](https://learn.microsoft.com/visualstudio/debugger/supported-code-changes-csharp).
- **Simulator and physical devices** support
- What can be Hot Reloaded:
  - **XAML files** in the **main project** and **referenced projects libraries**
  - **C# Markup controls**
  - **Bindings**
  - Full **x:Bind expressions**
  - **App.xaml** and **referenced resource dictionaries**
  - **DataTemplates**
  - **Styles**
  - Extensible [**State restoration**](xref:Uno.Contributing.Internals.HotReload)
  - Support for partial **tree hot reload**, where modifying a `UserControl` instantiated in multiple locations will reload it without reloading its parents

Hot Reload features vary between platforms and IDE, you can check below the list of currently supported features.

## How to use Hot Reload

### [**Visual Studio 2022**](#tab/vswin)

- Setup your environment by following our [getting started guides](xref:Uno.GetStarted.vs2022).
- Start your application (with or without the debugger, depending on the supported features below)
- Make changes to your XAML or C# code, save your file then press the red flame icon in the toolbar or use `Alt+F10`

### [**Visual Studio Code**](#tab/vscode)

- Setup your environment by following our [getting started guide](xref:Uno.GetStarted.vscode)
- Start the application (with or without the debugger, depending on the supported features below)
- Wait a few seconds for the hot reload engine to become available (see our troubleshooting tips below)
- Make changes to your XAML or C# code, then save your file

***

> [!IMPORTANT]
> Using [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) or later (`net8.0` in the `TargetFrameworks` property) is required for Hot Reload to be available when your solution contains iOS, Android, Mac Catalyst, or WebAssembly project heads. On Windows, [Visual Studio 17.8](https://visualstudio.microsoft.com/vs) or later is required.

## Supported features

### [**Desktop**](#tab/skia-desktop)

Skia-based targets provide support for full XAML Hot Reload and C# Hot Reload. There are some restrictions that are listed below:

- The Visual Studio 2022 for Windows support is fully available, with and without running under the debugger
- As of VS 2022 17.9 XAML Hot Reload under WSL is not supported
- VS Code
  - With the debugger: The C# Dev Kit is handling hot reload [when enabled](https://code.visualstudio.com/docs/csharp/debugging#_hot-reload). As of December 20th, 2023, C# Dev Kit hot reload does not handle class libraries. To experience the best hot reload, do not use the debugger.
  - Without the debugger: The VS Code Uno Platform extension is handling Hot Reload (C# or XAML)
  - Adding new C# or XAML files to a project is not yet supported

### [**WebAssembly**](#tab/wasm)

WebAssembly is currently providing both full and partial Hot Reload support, depending on the IDE.

- In Visual Studio Code:
  - Both C# and XAML Hot Reload are fully supported
  - Adding new C# or XAML files to the project is not yet supported
  - Hot Reload is not supported when using the debugger
- In Visual Studio for Windows:
  - Hot Reload is sensitive to Web Workers caching, which can cause errors like [this Visual Studio issue](https://developercommunity.visualstudio.com/t/BrowserLink-WebSocket-is-disconnecting-a/10500228), with a `BrowserConnectionException` error. In order to fix this:
    - Update to Uno.Wasm.Bootstrap 8.0.3 or later
    - Unregister any Web Worker associated with your app (Chrome or Edge) by **Developer tools (F12)** -> **Application** -> **Service worker** and **unregister**.
  - [`MetadataUpdateHandlers`](https://learn.microsoft.com/dotnet/api/system.reflection.metadata.metadataupdatehandlerattribute) are invoked without the list of changed types, which means that some hot reload features may not be available.
  - Hot Reload is not supported when using the debugger

### [**iOS, Android, and Catalyst**](#tab/mobile)

Mobile targets are currently using a limited version of XAML Hot Reload and do not support C# Hot Reload until [this dotnet runtime](https://github.com/dotnet/runtime/issues/93860) issue is fixed.

- In Visual Studio, the "Hot Reload on File Save" feature must be disabled to avoid crashing the app. You can find this feature by clicking on the down arrow next to the red flame in the Visual Studio toolbar.
- In both VS and VS Code, C# Hot Reload is not yet supported
- XAML `x:Bind` hot reload is limited to simple expressions and events

### [**WinAppSDK**](#tab/winappsdk)

Hot Reload is supported by Visual Studio for WinAppSDK and provides support in unpackaged deployment mode.

***

## Troubleshooting

### Common issues

- Observe the application logs, you should see diagnostics messages in the app when a XAML file is reloaded.
- The file named `RemoteControlGenerator\RemoteControl.g.cs` in the analyzers node for your project contains the connection information, verify that the information host addresses and the port number.
- WinAppSDK on Windows-specific issues
  - Grid Succinct syntax [is not supported](https://github.com/microsoft/microsoft-ui-xaml/issues/7043#issuecomment-1120061686)
- If you're getting `ENC0003: Updating 'attribute' requires restarting the application`, add the following in the `Directory.Build.props` (or in each csproj project heads):

  ```xml
  <PropertyGroup>
    <!-- Required for Hot Reload (See https://github.com/unoplatform/uno.templates/issues/376) -->
    <GenerateAssemblyInfo Condition="'$(Configuration)'=='Debug'">false</GenerateAssemblyInfo>
  </PropertyGroup>
  ```

- If you're getting the `Unable to access Dispatcher/DispatcherQueue` error, you'll need to update your app startup to Uno 5 or later:
  - Add the following lines to the shared library project `csproj` file :

    ```xml
    <ItemGroup>
        <PackageReference Include="Uno.WinUI.DevServer" Version="$UnoWinUIVersion$" Condition="'$(Configuration)'=='Debug'" />
    </ItemGroup>
    ```

    > [!NOTE]
    > If your application is using the UWP API set (Uno.UI packages) you'll need to use the `Uno.UI.DevServer` package instead.
  - Then, in your `App.cs` file, add the following:
  
    ```csharp
    using Uno.UI;

    //... in the OnLaunched method

    #if DEBUG
            MainWindow.EnableHotReload();
    #endif
    ```

### Visual Studio 2022

- Make sure that **C# Hot Reload** is not disabled in Visual Studio
  - Open Tools / Options
  - Search for **.NET / C++ Hot Reload**
  - Ensure that all three checkboxes are checked (_**Enable hot reload when debugging**_, _**Enable Hot Reload without debugging**_ and _**Apply Hot Reload on File Save**_)
- Hot Reload for WebAssembly is not supported when using the debugger. Start your app using `Ctrl+F5`.
- The output window in VS has an output named `Uno Platform` in its drop-down. Diagnostics messages from the VS integration appear there. Changing the MSBuild build output verbosity in `Tools/Options/Projects and Solutions/Build And Run` controls the log level.
- When a file is reloaded, XAML parsing errors will appear in the application's logs, on device or in browser.
- If there are multiple versions of the Uno.WinUI Package present in the solution, the newest will be used, regardless of the started application
- For `net8.0-windows10.xx`:
  - [A VS issue for WinUI may be hit](https://developercommunity.visualstudio.com/t/net80-windows10-needs-to-be-first-for-W/10643724). If XAML hot reload does not work, ensure that the `Uno Platform` output window exists, and that it mentions that the extension has successfully loaded. To do so, try closing and reopening the solution, and make sure that the [Visual Studio extension is installed](xref:Uno.GetStarted.vs2022).
  - Ensure that the `net8.0-windows10.xxx` target framework is selected in the top-left dropdown list of the XAML editor. Any other selected platform will break hot reload.

### VS Code

- The output window in Code has an output named "Uno Platform - Hot Reload" in its drop-down. Diagnostics messages from the extension appear there.
- Hot Reload is not supported for WebAssembly and Skia Desktop when using the debugger. Start your app using `Ctrl+F5`.
- Depending on your machine's performance, the hot reload engine may take a few moments to initialize and take your project modifications into account.
- Make sure that the selected project in the status bar is not the solution file, but rather the project platform you are debugging.
- If Hot Reload does not function properly, you can try using the `Developer: Reload Window` command in the palette (using `Ctrl+Shift+P`)
- When working on Skia Desktop apps, make sure to start the app without the debugger, and make sure that in the debugger tab, the `Uno Platform Desktop (Debug)` target is selected.

## Next Steps

Learn more about:

- [Uno Platform features and architecture](xref:Uno.GetStarted.Explore)
- [Uno Platform App solution structure](xref:Uno.Development.AppStructure)
- [Troubleshooting](xref:Uno.UI.CommonIssues)
- [List of views implemented in Uno](../implemented-views.md) for the set of available controls and their properties.
- You can head to [How-tos and tutorials](xref:Uno.Tutorials.Intro) on how to work on your Uno Platform app.
