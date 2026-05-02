# C# 14

> The notable language features in C# 14 (.NET 10) you should reach for daily.

## Core Concepts

- **`field` keyword** — implicit backing field inside property accessors. No more manual `_name` fields.
- **Extension members** — extension *properties* and *static methods*, not just instance methods. Group with `extension(...)`.
- **Partial events / partial constructors** — splittable across files; useful with source generators.
- **Lambda parameter modifiers** — `ref`, `in`, `out`, `scoped`, `ref readonly` allowed on lambda parameters without explicit type.
- **Unbound generic in `nameof`** — `nameof(List<>)` works.
- **First-class span conversions** — implicit `T[]` -> `Span<T>` / `ReadOnlySpan<T>` in more places, including overload resolution.
- **`params` collections** — `params ReadOnlySpan<T>`, `params IEnumerable<T>`, etc.
- **Primary constructors recap** — declare deps once on the class signature; capture them as instance state.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Property with validation | `set => field = value ?? throw new ArgumentNullException(...)` | Replace 90% of `_x` fields |
| Add property to a sealed type | `extension(string s) { public bool IsBlank => ...; }` | Augment BCL types |
| Allocation-free varargs | `void Log(params ReadOnlySpan<string> parts)` | Hot logging / formatting paths |
| Get name of open generic | `nameof(Dictionary<,>)` | Diagnostics, source-gens |
| Inject deps | `class OrderSvc(IRepo repo, ILogger<OrderSvc> log)` | Replace constructor + readonly fields |
| Pass `ref` into lambda | `(ref int x) => x++` | Span/struct manipulators |

## Quick Reference

```csharp
// field-backed property
public string Name
{
    get => field;
    set => field = (value ?? "").Trim();
}

// extension member (property + static)
public static class StringExtensions
{
    extension(string s)
    {
        public bool IsBlank => string.IsNullOrWhiteSpace(s);
        public static string Empty => string.Empty;
    }
}

// usage
var blank = "  ".IsBlank;          // true
var empty = string.Empty;          // via extension static

// params collections (no allocation)
static int Sum(params ReadOnlySpan<int> xs)
{
    var total = 0;
    foreach (var x in xs) total += x;
    return total;
}

// unbound nameof
var n = nameof(Dictionary<,>);     // "Dictionary"
```

## Common Pitfalls

- `field` keyword is reserved inside accessors — do not declare a local named `field`.
- Extension properties cannot have *state* (no backing field per instance).
- `params ReadOnlySpan<T>` callers must compile against C# 14; older callers fall back to array overload if you provide one.
- Primary constructor parameters are captured per instance — they're effectively private fields, not readonly. Don't mutate them.

## Examples in this folder

- [`CSharp14Features.cs`](CSharp14Features.cs) — annotated single-file tour of all the headline features

## See also

- [`../AsyncPatterns/`](../AsyncPatterns/README.md)
- [`../Performance/`](../Performance/README.md) — span / params interplay
- [`../SourceGenerators/`](../SourceGenerators/README.md) — partial events / constructors
