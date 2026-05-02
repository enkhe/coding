# .NET MAUI

> .NET 10 MAUI (Multi-platform App UI) — one C# codebase for iOS, Android, macOS, Windows.

## Core Concepts

- **Single project** that targets `net10.0-android`, `net10.0-ios`, `net10.0-maccatalyst`, `net10.0-windows10.0.19041.0`.
- **XAML** for declarative UI; code-behind in C#. Hot Reload + .NET MAUI Hot Reload for XAML.
- **MVVM** with `CommunityToolkit.Mvvm` source generators (`[ObservableProperty]`, `[RelayCommand]`).
- **Shell** for app structure: tabs, flyout, URI navigation (`Shell.Current.GoToAsync("//details?id=42")`).
- **Platform-specific code** via `#if ANDROID/IOS/WINDOWS/MACCATALYST` or partial classes in `Platforms/`.
- **DI** via `MauiAppBuilder.Services` (same `IServiceCollection` you already know).
- **Blazor Hybrid** with `BlazorWebView` — Razor components rendering inside a WebView, sharing state with native MAUI.

## "To Be Dangerous" Cheatsheet

```bash
# install workload
dotnet workload install maui
# new app
dotnet new maui -n MyApp
# new MAUI Blazor Hybrid
dotnet new maui-blazor -n MyHybridApp
# run
dotnet build -t:Run -f net10.0-android
dotnet build -t:Run -f net10.0-ios
```

Project layout:

```
MyApp/
  MauiProgram.cs           // app + DI bootstrap
  App.xaml(.cs)            // app lifecycle
  AppShell.xaml(.cs)       // routes
  MainPage.xaml(.cs)       // pages
  Resources/
    Styles/                // colors, fonts
    Images/                // svg/png -> per-platform
  Platforms/
    Android/  iOS/  Windows/  MacCatalyst/
```

MVVM with CommunityToolkit.Mvvm:

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] private string greeting = "Hello";
    [RelayCommand] private async Task RefreshAsync() => Greeting = await api.PingAsync();
}
```

## Quick Reference

| Task                         | API                                                |
| ---------------------------- | -------------------------------------------------- |
| Navigation                   | `Shell.Current.GoToAsync("route")`                 |
| Inject service               | ctor injection from `MauiAppBuilder.Services`      |
| Storage                      | `Preferences`, `SecureStorage`, SQLite-net         |
| HTTP                         | `IHttpClientFactory`                               |
| Connectivity                 | `Connectivity.Current.NetworkAccess`               |
| Permissions                  | `Permissions.RequestAsync<Permissions.Camera>()`   |
| Dispatch to UI               | `Dispatcher.Dispatch(() => ...)`                   |
| Lifecycle                    | `Window.Created/Activated/Stopped/Destroying`     |

## Common Pitfalls

- Forgetting to install MAUI workloads (`dotnet workload install maui`) on CI.
- Mixing `Page.OnAppearing` with constructor-time async — use `IAsyncInitialize` patterns.
- Missing platform permissions in `Platforms/Android/AndroidManifest.xml` and `Platforms/iOS/Info.plist`.
- Blazor Hybrid: keep DOM small, use virtualization, avoid recreating `BlazorWebView`.
- Memory leaks from event handlers — prefer `WeakReferenceMessenger`.

## Examples in this folder

- [MainPage.xaml](./MainPage.xaml) - Shell page with a button bound to a command.
- [MainViewModel.cs](./MainViewModel.cs) - `ObservableObject` view model with `[RelayCommand]`.
- [App.xaml.cs](./App.xaml.cs) - `MauiProgram` + `App` setup with DI.

## See also

- [Mobile/](../README.md)
- [BackEnd/](../../BackEnd/README.md) - shared API consumption.
- Microsoft Docs: <https://learn.microsoft.com/dotnet/maui/>
