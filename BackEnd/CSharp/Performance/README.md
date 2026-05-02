# Performance

> Memory and throughput primitives in modern .NET.

## Core Concepts

- **`Span<T>` / `ReadOnlySpan<T>`** — stack-only view over contiguous memory (array, stackalloc, native, string). Zero-alloc slicing.
- **`Memory<T>` / `ReadOnlyMemory<T>`** — heap-storable analog of span; safe to capture in async methods.
- **`ArrayPool<T>`** — rent buffers; `Return` after use. Cuts GC pressure in hot paths.
- **`IBufferWriter<T>`** — sink interface used by JSON writer, pipelines, etc.
- **`System.IO.Pipelines`** — high-throughput byte processing without `Stream` allocation/copy chatter.
- **`[SkipLocalsInit]`** — skip JIT-emitted zero-init of stack locals; only safe when your code initializes them itself.
- **struct vs class** — use `struct` for short-lived, small (≤ ~24 bytes), value-semantic data. Pass `in` to avoid copies.
- **BenchmarkDotNet** — measure first; never optimize on hunches.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Slice a string | `ReadOnlySpan<char> s = src.AsSpan(2, 4);` | Parsing without `Substring` |
| Stackalloc | `Span<byte> buf = stackalloc byte[256];` | Tiny, short-lived buffer |
| Pool larger | `var rent = ArrayPool<byte>.Shared.Rent(8192); try {...} finally { ArrayPool<byte>.Shared.Return(rent); }` | Per-request buffers |
| Read pipe | `var result = await reader.ReadAsync(ct); var buf = result.Buffer;` | Byte stream parsing |
| Write to JSON | `Utf8JsonWriter` over `IBufferWriter<byte>` | Custom serializers |
| Skip zero-init | `[SkipLocalsInit] static void Hot() { ... }` | Hottest-of-hot |
| Avoid struct copy | `void Take(in BigStruct s)` | Large struct param |
| Measure | `[Benchmark] public int Run() { ... }` | Always before/after |

## Quick Reference

```csharp
// Pooled buffer
var pool = ArrayPool<byte>.Shared;
var buf = pool.Rent(4096);
try
{
    var read = await stream.ReadAsync(buf.AsMemory(0, 4096), ct);
    Process(buf.AsSpan(0, read));
}
finally { pool.Return(buf); }

// Span-based parse
static int ParseInt(ReadOnlySpan<char> text) => int.Parse(text);

// Pipelines reader
var reader = PipeReader.Create(stream);
while (true)
{
    var result = await reader.ReadAsync(ct);
    var buffer = result.Buffer;
    // consume...
    reader.AdvanceTo(buffer.End);
    if (result.IsCompleted) break;
}
```

## Common Pitfalls

- Capturing `Span<T>` in async / lambda — won't compile (it's `ref struct`). Use `Memory<T>` instead.
- Forgetting `Return` on rented arrays — leaks pooled memory until GC.
- Stackalloc inside loops — use a single buffer outside the loop.
- Premature struct conversion — large structs copy by value; profile before/after.
- Optimizing serialization where the bottleneck is DB.
- `[SkipLocalsInit]` on methods that read uninitialized locals — undefined behavior.

## Examples in this folder

- [`SpanBasics.cs`](SpanBasics.cs) — stackalloc, slicing, span-based parsing
- [`ArrayPoolExample.cs`](ArrayPoolExample.cs) — rent/return pattern
- [`PipelinesExample.cs`](PipelinesExample.cs) — `PipeReader`/`PipeWriter`
- [`BufferWriterExample.cs`](BufferWriterExample.cs) — write JSON to `IBufferWriter<byte>`
- [`StructVsClass.cs`](StructVsClass.cs) — `in` parameter and `readonly struct`
- [`Benchmarks.cs`](Benchmarks.cs) — BenchmarkDotNet skeleton

## See also

- [`../AsyncPatterns/`](../AsyncPatterns/README.md)
- [`../NativeAOT/`](../NativeAOT/README.md)
