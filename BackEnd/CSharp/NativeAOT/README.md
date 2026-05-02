# Native AOT

> Ahead-of-time compile your .NET 10 app to a single, dependency-free native binary.

## Core Concepts

- **`<PublishAot>true</PublishAot>`** — flips the project into AOT mode; trimmer + ILC kicks in.
- **No JIT, no reflection-emit** — emits machine code at publish time. Fast startup, smaller image, lower memory.
- **Trimming** — unused IL is removed; everything must be statically reachable or annotated.
- **Source generators replace runtime codegen** — JSON, regex, logging, gRPC, dependency injection (Microsoft.Extensions.DI does support AOT in .NET 8+ scenarios).
- **Trade-offs** — slower build, larger source, no `Assembly.LoadFrom`, no `Activator.CreateInstance(Type)` without metadata, no `dynamic`.

## "To Be Dangerous" Cheatsheet

| What | How | Notes |
|---|---|---|
| Enable AOT | `<PublishAot>true</PublishAot>` | Implies `PublishTrimmed=true` |
| AOT-friendly JSON | `[JsonSerializable(typeof(MyDto))]` on a `JsonSerializerContext` | Pass context to `JsonSerializer` |
| Suppress trim warnings | `[DynamicallyAccessedMembers(...)]` | Document what reflection needs |
| Mark dynamic dep | `[DynamicDependency(...)]` | Keep a method even though unreferenced |
| Confirm AOT-clean | `dotnet publish -c Release` and watch for IL2026/IL3050 warnings | Treat warnings as errors in CI |
| Publish | `dotnet publish -c Release -r linux-x64` | Produces a single native binary |

## Quick Reference

```xml
<!-- App.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <InvariantGlobalization>true</InvariantGlobalization>
    <StripSymbols>true</StripSymbols>
    <IsAotCompatible>true</IsAotCompatible>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

```csharp
// AOT-friendly Minimal API entrypoint
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default));

var app = builder.Build();
app.MapGet("/users/{id:int}", (int id) => new User(id, $"u{id}"));
app.Run();

public sealed record User(int Id, string Name);

[JsonSerializable(typeof(User))]
internal partial class AppJsonContext : JsonSerializerContext;
```

## Avoid

- `Assembly.Load*`, `Type.GetType("Foo, Asm")`
- `Activator.CreateInstance(Type)` without `[DynamicallyAccessedMembers]`
- `Expression.Compile()` (uses reflection.emit) — use source-gen alternatives
- Reflection-based serializers (default `JsonSerializer`, Newtonsoft.Json, AutoMapper)
- `dynamic`
- Plugins via `AssemblyLoadContext`

## Common Pitfalls

- "It builds!" — but warnings IL2xxx / IL3xxx mean the trimmer removed something needed at runtime. Fix or annotate.
- Using `Microsoft.AspNetCore.Mvc.Controllers` — controller routing reflects; prefer Minimal API for AOT.
- `EF Core` requires source-generated compiled models (`dotnet ef dbcontext optimize`).
- `Microsoft.Extensions.Logging.Console` console formatter is fine; custom log providers via reflection are not.

## Examples in this folder

- [`Program.cs`](Program.cs) — AOT-ready Minimal API with source-gen JSON
- [`AotApi.csproj`](AotApi.csproj) — `<PublishAot>true</PublishAot>` project file
- [`TrimmingNotes.cs`](TrimmingNotes.cs) — `[DynamicallyAccessedMembers]` and `[RequiresUnreferencedCode]` examples

## See also

- [`../SourceGenerators/`](../SourceGenerators/README.md)
- [`../Performance/`](../Performance/README.md)
