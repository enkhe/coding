# BlazorWasm

> .NET runs in the browser via WebAssembly; static-host friendly, no server round-trip per event.

## Core Concepts

- **AOT compilation** — `<RunAOTCompilation>true</RunAOTCompilation>` reduces startup CPU at cost of size
- **Trimming** — IL linker drops unused code; `<PublishTrimmed>true</PublishTrimmed>`
- **Lazy assembly loading** — load DLLs on demand via `LazyAssemblyLoader`
- **HttpClient** — typed clients with `DelegatingHandler` for auth, retries
- **Globalization** — invariant by default; opt-in with `<InvariantGlobalization>false</InvariantGlobalization>`

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Smaller bundles | trimming + AOT + Brotli | always for production |
| Auth header on every call | `DelegatingHandler` injected into `HttpClient` | secured APIs |
| Defer rare features | `LazyAssemblyLoader.LoadAssembliesAsync` | admin pages, charting |
| State across reloads | `localStorage` via JS interop or `Blazored.LocalStorage` | offline-friendly UX |
| Hot reload | `dotnet watch` + interactive WASM | dev loop |

## Quick Reference

```csharp
// Program.cs (WASM)
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddTransient<AuthHandler>();
builder.Services.AddHttpClient("Api", c =>
        c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AuthHandler>();

await builder.Build().RunAsync();
```

## Common Pitfalls

- Shipping non-trimmed binaries — multi-MB downloads
- Reflection-heavy code breaks under trimming — annotate with `[DynamicallyAccessedMembers]`
- Synchronous JS interop in render path — always async
- `HttpClient.BaseAddress` not set — relative URIs fail

## Examples in this folder

- [`Program.cs`](Program.cs) — DI + HttpClient with auth handler
- [`AuthHandler.cs`](AuthHandler.cs) — DelegatingHandler attaching bearer token
- [`LazyLoadDemo.razor`](LazyLoadDemo.razor) — load assembly on demand

## See also

- [../README.md](../README.md) — render modes
- [../../Performance](../../Performance/README.md) — bundle size, caching
