# Design — Rate Limiter

## Requirements

- Limit per user / API key / IP
- Configurable per-route limits
- Survive process restart; work across many instances
- < 1ms overhead

## Algorithms

| Algorithm | Notes |
|---|---|
| **Fixed window** | Reset counter every window; bursts at boundary |
| **Sliding window log** | Store every request timestamp; accurate but memory-heavy |
| **Sliding window counter** | Weighted blend of current + previous fixed window — best balance |
| **Token bucket** | N tokens, refill at R/sec; allows bursts up to bucket size |
| **Leaky bucket** | Constant outflow regardless of input |

## Distributed implementation

```
Client → API → Rate-limit middleware → Redis (atomic INCR + EXPIRE)
```

```lua
-- Atomic per-key increment + first-write expiry
local current = redis.call('INCR', KEYS[1])
if current == 1 then
  redis.call('PEXPIRE', KEYS[1], ARGV[1])    -- window in ms
end
return current
```

```csharp
// .NET 10 sliding-window in Redis
public sealed class SlidingWindow(IConnectionMultiplexer mux)
{
    private readonly IDatabase _db = mux.GetDatabase();

    public async Task<bool> AllowAsync(string key, int limit, TimeSpan window)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var min = now - (long)window.TotalMilliseconds;
        var tx = _db.CreateTransaction();
        _ = tx.SortedSetRemoveRangeByScoreAsync(key, double.NegativeInfinity, min);
        _ = tx.SortedSetAddAsync(key, now, now);
        var count = tx.SortedSetLengthAsync(key);
        _ = tx.KeyExpireAsync(key, window);
        await tx.ExecuteAsync();
        return (await count) <= limit;
    }
}
```

## ASP.NET Core 10 built-in

```csharp
builder.Services.AddRateLimiter(o =>
{
    o.AddSlidingWindowLimiter("standard", c =>
    {
        c.Window = TimeSpan.FromSeconds(60);
        c.SegmentsPerWindow = 6;
        c.PermitLimit = 100;
        c.QueueLimit = 0;
    });
});
app.UseRateLimiter();
app.MapGet("/orders", () => Results.Ok()).RequireRateLimiting("standard");
```

## Tradeoffs

- **Local in-process** — fast, but limit is *per pod*, not per fleet
- **Redis-backed** — fleet-wide, ~1ms hop, single point of failure (mitigate with cluster)
- **Edge (CDN/WAF)** — fast and cheap; but can't see app-level identity

## Common Pitfalls

- Using IP as key behind a proxy without `X-Forwarded-For` parsing
- Resetting counters on deploy → bursts allowed
- Forgetting `Retry-After` header on 429
- Sliding window logs without TTL → memory leak

## See also

- [../../Architecture/ApiGateway](../../Architecture/ApiGateway/) · [../../Database/Redis](../../Database/Redis/) · [../../Observability/Resilience](../../Observability/Resilience/)
