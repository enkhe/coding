# C# / .NET

> .NET 10 + C# 14 cheatsheet for senior/architect track. Each subfolder is a focused topic with runnable example code.

## Subfolders

### Language & runtime
- [`CSharp14/`](CSharp14/README.md) — field-backed properties, extension members, partial events, lambda modifiers, unbound `nameof`
- [`AsyncPatterns/`](AsyncPatterns/README.md) — `Task` vs `ValueTask`, `IAsyncEnumerable`, channels, cancellation
- [`Performance/`](Performance/README.md) — `Span<T>`, `ArrayPool`, `Pipelines`, BenchmarkDotNet
- [`NativeAOT/`](NativeAOT/README.md) — AOT publishing, trimming, what to avoid
- [`SourceGenerators/`](SourceGenerators/README.md) — `[LoggerMessage]`, regex, JSON, custom incremental gen

### Hosts & frameworks
- [`AspNetCore/`](AspNetCore/README.md) — middleware, DI, options, OpenAPI 3.1, rate limiting, output cache
- [`MinimalApi/`](MinimalApi/README.md) — route groups, typed results, endpoint filters
- [`WebApi/`](WebApi/README.md) — controllers, model binding, ProblemDetails
- [`Workers/`](Workers/README.md) — `BackgroundService`, scoped services, graceful shutdown
- [`ConsoleApps/`](ConsoleApps/README.md) — `System.CommandLine`, `Spectre.Console`, host builder
- [`ClassLibraries/`](ClassLibraries/README.md) — multi-target, `InternalsVisibleTo`, packaging

### Cloud-native & cross-cutting
- [`Aspire/`](Aspire/README.md) — AppHost, service references, dashboard, Redis/Postgres
- [`Resilience/`](Resilience/README.md) — Polly v8 pipelines, HttpClient resilience
- [`Mediator/`](Mediator/README.md) — handler pattern, pipeline behaviors, notifications

### Legacy
- [`WCF/`](WCF/README.md) — maintenance only; see `Modernization/WcfToAspNetCore`

## .NET 10 / C# 14 "To Be Dangerous" Cheatsheet

| Feature | Snippet | Notes |
|---|---|---|
| Field-backed property | `public string Name { get; set => field = value?.Trim(); }` | `field` keyword replaces backing field boilerplate |
| Extension members | `extension(string s) { public bool IsBlank => string.IsNullOrWhiteSpace(s); }` | Properties + static members on types you don't own |
| Primary constructor | `public class Svc(ILogger<Svc> log)` | Captures parameters; treat as readonly |
| Required member | `public required string Email { get; init; }` | Compiler-enforced init |
| Collection expression | `int[] xs = [1, 2, 3, ..other];` | Spread + target-typed |
| `params` collections | `void Log(params ReadOnlySpan<string> args)` | Allocation-free |
| Pattern matching | `obj is Order { Total: > 100 } o` | Property + type pattern |
| Records | `public record Order(int Id, decimal Total);` | Value equality, `with` expressions |
| `ValueTask` hot path | `public ValueTask<int> ReadAsync(...)` | Avoid alloc when sync-completing |
| `IAsyncEnumerable` | `await foreach (var x in stream.WithCancellation(ct))` | Stream large results |
| Result types | `Results.Ok(x)` / `TypedResults.Ok(x)` | `TypedResults` is OpenAPI-aware |
| Source-gen logger | `[LoggerMessage(LogLevel.Information, "user {id}")]` | Zero-alloc, structured |
| Source-gen JSON | `[JsonSerializable(typeof(Foo))]` | Required for AOT |
| Source-gen regex | `[GeneratedRegex(@"\d+")]` | Compiled at build |
| Polly pipeline | `new ResiliencePipelineBuilder().AddRetry(...).Build()` | v8 API |
| Channel | `Channel.CreateBounded<T>(opts)` | Producer/consumer |
| `[SkipLocalsInit]` | on hot method | Skip stack-zeroing |

## Project file essentials (.NET 10)

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <InvariantGlobalization>true</InvariantGlobalization>
    <PublishAot>true</PublishAot>
  </PropertyGroup>
</Project>
```

## See also

- [.NET 2026 Roadmap](../../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [`../README.md`](../README.md) — backend index
