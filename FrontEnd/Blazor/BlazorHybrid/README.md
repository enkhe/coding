# BlazorHybrid

> Razor components hosted by `BlazorWebView` inside a native MAUI / WPF / WinForms app.

## Core Concepts

- **No HTTP between UI and host** — components run in-process; full access to native APIs
- **`BlazorWebView`** — WebView2 (Win) / WKWebView (mac) / WebView (Android) renders your components
- **Shared RCL** — put components in a Razor Class Library so Server/WASM/Hybrid all reuse them
- **Native bridge** — inject native services directly (`IGeolocation`, `IFileSystem`, etc.)

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Cross-platform desktop/mobile | MAUI + `BlazorWebView` | reuse Razor UI in native shell |
| Share code with web | Razor Class Library | one component, three hosts |
| Call native APIs | inject MAUI service into component | camera, sensors, files |
| Custom CSS isolation | `MyPage.razor.css` | per-component scoping |

## Quick Reference

```xml
<!-- MainPage.xaml in MAUI -->
<ContentPage xmlns:b="http://schemas.microsoft.com/dotnet/2021/maui">
    <b:BlazorWebView HostPage="wwwroot/index.html">
        <b:BlazorWebView.RootComponents>
            <b:RootComponent Selector="#app" ComponentType="{x:Type local:Main}" />
        </b:BlazorWebView.RootComponents>
    </b:BlazorWebView>
</ContentPage>
```

## Common Pitfalls

- Forgetting to register Razor components: `services.AddMauiBlazorWebView()`
- Mixing WASM-only packages — they may break in Hybrid
- Heavy DOM updates in mobile WebViews are slow — use virtualization

## Examples in this folder

- [`MauiProgram.cs`](MauiProgram.cs) — MAUI host registration
- [`MainPage.xaml`](MainPage.xaml) — `BlazorWebView` declaration
- [`Main.razor`](Main.razor) — root component

## See also

- [../README.md](../README.md) — render modes
- [`../../../Mobile/Maui`](../../../Mobile/Maui) — MAUI fundamentals
