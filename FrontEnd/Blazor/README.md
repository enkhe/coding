# Blazor

> .NET 10 component model running on Server (SignalR), WebAssembly, Hybrid (MAUI), or Auto.

## Core Concepts

- **Render modes** — `InteractiveServer`, `InteractiveWebAssembly`, `InteractiveAuto`, `Static SSR`
- **Component lifecycle** — `OnInitialized[Async]`, `OnParametersSet[Async]`, `OnAfterRender[Async]`, `Dispose`
- **Routing** — `@page "/route"` directive; `NavigationManager` for programmatic nav
- **Forms** — `EditForm` + `DataAnnotationsValidator`; new `FluentValidation` integration
- **DI** — `[Inject]` attribute or `@inject` directive; scoped per circuit (Server) or app (WASM)
- **JS interop** — `IJSRuntime.InvokeAsync<T>`; `IJSObjectReference` for modules
- **Prerendering** — server renders static HTML, then hydrates to interactive

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Real-time UI, low latency | `InteractiveServer` | dashboards, internal tools |
| Offline / CDN-hosted | `InteractiveWebAssembly` + AOT | public SPAs |
| Best of both | `InteractiveAuto` | start Server, upgrade to WASM |
| Native + web reuse | `BlazorHybrid` (MAUI) | desktop/mobile with shared UI |
| Fast first paint | `StaticServer` + selective interactivity | content-heavy pages |
| Tabular data | `<QuickGrid>` (Microsoft.AspNetCore.Components.QuickGrid) | sortable tables |
| Component caching | `[StreamRendering]` + `@key` | streaming SSR |

## Quick Reference

```razor
@page "/counter"
@rendermode InteractiveServer
@inject ILogger<Counter> Log

<h1>Count: @count</h1>
<button @onclick="Increment">+1</button>

@code {
    int count;
    void Increment() { count++; Log.LogInformation("Clicked"); }
}
```

## Render Mode Selection

```razor
@* per-component *@
<Counter @rendermode="InteractiveAuto" />

@* in Program.cs *@
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
```

## Common Pitfalls

- Forgetting `@rendermode` — component renders static, no interactivity
- `OnInitializedAsync` runs twice with prerendering (server + client) — guard with `RendererInfo.IsInteractive`
- Capturing scoped services in singletons via root component
- Heavy JS interop in tight loops — batch calls
- Mixing `IJSRuntime` from prerender phase (will throw)

## Subfolders

- [BlazorServer](BlazorServer/README.md) — SignalR-backed, scoped per circuit
- [BlazorWasm](BlazorWasm/README.md) — runs in browser, AOT/trimming
- [BlazorHybrid](BlazorHybrid/README.md) — `BlazorWebView` in MAUI

## See also

- [DotNet/AspNet](../../DotNet/AspNet) — host configuration
- [StateManagement](../StateManagement/README.md) — cascading values vs Fluxor
- [Performance](../Performance/README.md) — prerendering, streaming
