# Caching

> Cache aside, write through, hybrid, and stampede-safe. .NET 9+ ships **`HybridCache`** as the default.

## Core Concepts

- **Cache aside (lazy load)** — read; if miss, fetch from source, write to cache, return. Most common.
- **Read-through** — cache library fetches on miss (transparent to caller).
- **Write-through / write-behind** — writes go to cache then source (or async).
- **`HybridCache`** — built into .NET; combines `IMemoryCache` (L1) with `IDistributedCache` (L2 — Redis), single-flight stampede protection.
- **Cache stampede** — many requests miss simultaneously and hammer the source. Solve via single-flight (`HybridCache`) or probabilistic early expiration.
- **Invalidation** — TTL is good; **explicit invalidation by tag** is better when correctness matters.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| In-process cache | `IMemoryCache` (per-instance) |
| Distributed cache | `IDistributedCache` (Redis, SQL) |
| Both with single-flight | **`HybridCache`** (.NET 9+) — preferred |
| Tag-based invalidation | `cache.RemoveByTagAsync("user:123")` |
| Explicit set | `await cache.SetAsync(key, value, options)` |
| TTL options | `HybridCacheEntryOptions { Expiration, LocalCacheExpiration }` |

## Quick Reference (HybridCache)

```csharp
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1 << 20;          // 1 MiB
    options.MaximumKeyLength = 256;
    options.DefaultEntryOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2),
    };
});

builder.Services.AddStackExchangeRedisCache(o =>
    o.Configuration = builder.Configuration.GetConnectionString("Redis"));
```

```csharp
public sealed class OrderReader(HybridCache cache, IOrderRepository repo)
{
    public ValueTask<OrderDto> GetAsync(Guid id, CancellationToken ct) =>
        cache.GetOrCreateAsync(
            key: $"order:{id}",
            factory: async ct => await repo.GetAsync(id, ct),
            tags: [$"order:{id}", "orders"],
            cancellationToken: ct);

    public Task InvalidateAsync(Guid id, CancellationToken ct) =>
        cache.RemoveAsync($"order:{id}", ct);
}
```

## When to NOT use a cache

- Source is fast (sub-ms reads, no scale issue)
- Data must always be fresh (no `staleness budget`)
- Adding a cache hides a bug — fix the source first

## Common Pitfalls

- Caching mutable data without invalidation → stale forever
- Long TTLs hiding bugs and stale state during incidents
- Cache key includes user id → cross-user leakage if forgotten
- Storing serialized large objects → memory blowup on Redis
- No metrics on hit rate → you don't know if the cache is helping
- Cache stampede on a hot key → use single-flight (`HybridCache` does this)

## Examples in this folder

- [`HybridCacheExample.cs`](HybridCacheExample.cs) — modern .NET 9+
- [`RedisCacheExample.cs`](RedisCacheExample.cs) — direct StackExchange.Redis
- [`CacheKeys.cs`](CacheKeys.cs) — type-safe key builder

## See also

- [../Redis](../Redis/) · [../../BackEnd/CSharp/Performance](../../BackEnd/CSharp/Performance/)
