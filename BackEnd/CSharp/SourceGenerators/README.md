# Source Generators

> Move codegen from runtime to compile time. Required for NativeAOT, big perf win even without it.

## Core Concepts

- **`[LoggerMessage]`** — generates a strongly-typed, allocation-free logger method.
- **`[GeneratedRegex]`** — generates a compiled `Regex` at build (no `Regex.Compile()` at runtime).
- **`JsonSerializerContext`** — generates type-info metadata for `System.Text.Json`; AOT-required.
- **Custom incremental generators** — `IIncrementalGenerator` + `RegisterSourceOutput` for your own codegen.
- **`partial`** is the contract — the consumer writes `partial class` / `partial method`; the generator fills it in.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Logging | `[LoggerMessage(LogLevel.Information, "user {UserId}")] static partial void UserOk(ILogger l, int userId);` | Every log site in hot paths |
| Regex | `[GeneratedRegex(@"\d+", RegexOptions.IgnoreCase)] static partial Regex Digits();` | Replace `new Regex(pattern)` |
| JSON | `[JsonSerializable(typeof(Order))] partial class AppJson : JsonSerializerContext;` | Always for AOT, optional otherwise |
| OpenAPI | Built-in via `Microsoft.AspNetCore.OpenApi` (also a source gen) | New projects |
| Custom gen | `[Generator] public sealed class MyGen : IIncrementalGenerator` | Internal DSL / boilerplate |
| Diagnose | View generated files: `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>` | Debugging |

## Quick Reference

```csharp
// Logger
public static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Order {OrderId} accepted in {Elapsed}ms")]
    public static partial void OrderAccepted(ILogger logger, int orderId, long elapsed);
}

// Regex
public static partial class Patterns
{
    [GeneratedRegex(@"^[a-z0-9._-]+@[a-z0-9.-]+\.[a-z]{2,}$", RegexOptions.IgnoreCase)]
    public static partial Regex Email();
}

// JSON
[JsonSerializable(typeof(Order))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AppJsonContext : JsonSerializerContext;
```

## Common Pitfalls

- Forgetting `partial` on the class — generator can't extend it.
- Generator sees stale code — bump version or rebuild after generator changes.
- `[LoggerMessage]` on instance methods — must be `static` if `ILogger` is a parameter; otherwise instance is fine if you have a field.
- JSON source-gen + polymorphism — list every concrete subtype in `[JsonDerivedType]`.
- Custom generators that aren't *incremental* will tank IDE perf — implement `IIncrementalGenerator`, not `ISourceGenerator`.

## Examples in this folder

- [`LoggerMessages.cs`](LoggerMessages.cs) — `[LoggerMessage]` patterns
- [`RegexExamples.cs`](RegexExamples.cs) — `[GeneratedRegex]`
- [`JsonContext.cs`](JsonContext.cs) — `JsonSerializerContext`
- [`IncrementalGeneratorSkeleton.cs`](IncrementalGeneratorSkeleton.cs) — minimal `IIncrementalGenerator`
- [`SourceGen.csproj`](SourceGen.csproj) — analyzer-style csproj for the generator project

## See also

- [`../NativeAOT/`](../NativeAOT/README.md) — when these become mandatory
- [`../CSharp14/`](../CSharp14/README.md) — partial events and primary constructors
