# Async Patterns

> The async toolkit every senior .NET dev should know cold.

## Core Concepts

- **`Task`** — heap-allocated, cached for completed-without-result. Use as the default.
- **`ValueTask` / `ValueTask<T>`** — struct; avoids alloc when the call frequently completes synchronously. Single-await only by default.
- **`IAsyncEnumerable<T>`** — async streaming with `await foreach`. Cancellation via `[EnumeratorCancellation]`.
- **`CancellationToken`** — pass it everywhere; honor it in tight loops with `ct.ThrowIfCancellationRequested()`.
- **`ConfigureAwait(false)`** — needed in libraries to avoid sync-context capture; not needed in ASP.NET Core (no sync context).
- **`IAsyncDisposable` + `await using`** — async cleanup (e.g., flush + close streams).
- **`Parallel.ForEachAsync`** — bounded concurrency over a sequence.
- **`Channel<T>`** — bounded/unbounded producer/consumer queues.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Sync-completing hot path | `ValueTask<int>` return | Cached / in-memory reads |
| Streaming results | `async IAsyncEnumerable<T>` + `[EnumeratorCancellation]` | Pagination, file reads, SSE |
| Bounded concurrency | `Parallel.ForEachAsync(items, opts, body)` | Fan-out HTTP / DB calls |
| Producer/consumer | `Channel.CreateBounded<T>(opts)` | Backpressure, queues |
| Library code | Always `.ConfigureAwait(false)` | NuGet packages |
| Async dispose | `await using var x = ...;` | Files, DB connections, http handlers |
| Combine timeouts + cancel | `CancellationTokenSource.CreateLinkedTokenSource(ct1, ct2)` | Per-request + per-call timeout |
| Don't block on async | `task.GetAwaiter().GetResult()` is a deadlock trap | Never |

## Quick Reference

```csharp
// ValueTask hot path
public ValueTask<User?> GetAsync(int id, CancellationToken ct) =>
    _cache.TryGet(id, out var u)
        ? ValueTask.FromResult<User?>(u)
        : new ValueTask<User?>(LoadAsync(id, ct));

// Streaming with cancellation
public async IAsyncEnumerable<Page> PagesAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    var cursor = 0;
    while (true)
    {
        var page = await FetchAsync(cursor, ct);
        if (page.Items.Count == 0) yield break;
        yield return page;
        cursor = page.NextCursor;
    }
}

// Bounded fan-out
await Parallel.ForEachAsync(
    urls,
    new ParallelOptions { MaxDegreeOfParallelism = 8, CancellationToken = ct },
    async (url, token) => await client.GetAsync(url, token));

// Channel
var ch = Channel.CreateBounded<Job>(new BoundedChannelOptions(100)
    { FullMode = BoundedChannelFullMode.Wait });
```

## Common Pitfalls

- `async void` — only valid for event handlers; otherwise unobserved exceptions crash the process.
- Awaiting a `ValueTask` twice — illegal unless `.Preserve()`'d.
- `Task.Run(() => asyncMethod())` — pointless; you're just adding a thread hop.
- Forgetting to pass / honor `CancellationToken` — leaks work after client disconnects.
- Mixing `Task.Wait()` / `.Result` with sync contexts — classic deadlock.
- Not using `ConfigureAwait(false)` in library code — surprising behavior in WPF/WinForms callers.

## Examples in this folder

- [`ValueTaskHotPath.cs`](ValueTaskHotPath.cs) — cache-first read returning `ValueTask<T>`
- [`AsyncStreams.cs`](AsyncStreams.cs) — `IAsyncEnumerable` with cancellation
- [`Cancellation.cs`](Cancellation.cs) — linked tokens, timeouts, propagation
- [`ChannelExample.cs`](ChannelExample.cs) — bounded producer/consumer
- [`ParallelForEachAsyncExample.cs`](ParallelForEachAsyncExample.cs) — bounded fan-out
- [`AsyncDisposable.cs`](AsyncDisposable.cs) — `await using` with `IAsyncDisposable`

## See also

- [`../Performance/`](../Performance/README.md)
- [`../Resilience/`](../Resilience/README.md) — retry/timeout pipelines
