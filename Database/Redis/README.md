# Redis

> In-memory data structures: cache, pub/sub, queues, leaderboards, distributed coordination.

## Data types

| Type | Use |
|---|---|
| String | Cached blobs, counters (`INCR`) |
| Hash | Aggregate of fields (user profile) |
| List | Queue (`LPUSH`/`RPOP`); but **Streams** are better |
| Set | Unique membership |
| Sorted set | Leaderboards, time-windowed counters |
| Stream | Append-only log; consumer groups |
| HyperLogLog | Approximate counts |
| Geo | Lat/long with `GEOSEARCH` |

## "To Be Dangerous" Cheatsheet

| Need | Command |
|---|---|
| Set with TTL | `SET key value EX 60` |
| Atomic create | `SET key value EX 60 NX` |
| Counter | `INCR key` (`INCRBY key 5`) |
| Hash | `HSET k field v`, `HGET`, `HGETALL` |
| Sorted set | `ZADD k score member`, `ZRANGEBYSCORE k min max` |
| Pub/sub | `SUBSCRIBE channel`, `PUBLISH channel msg` |
| Stream | `XADD orders * id 1`, `XREADGROUP GROUP g c COUNT 10 STREAMS orders >` |
| Lua script | `EVAL` / `EVALSHA` for atomic multi-op |
| Distributed lock | Redlock (with strong caveats) — prefer per-row DB locking |

## Quick Reference (StackExchange.Redis)

```csharp
var mux = await ConnectionMultiplexer.ConnectAsync("redis:6379");
var db = mux.GetDatabase();

// String + TTL
await db.StringSetAsync("user:42", JsonSerializer.Serialize(user), TimeSpan.FromMinutes(10));

// Atomic increment
var n = await db.StringIncrementAsync("rate:user:42");
await db.KeyExpireAsync("rate:user:42", TimeSpan.FromSeconds(60));

// Sorted set leaderboard
await db.SortedSetAddAsync("leaderboard", "alice", 100);
var top10 = await db.SortedSetRangeByRankWithScoresAsync("leaderboard", 0, 9, Order.Descending);
```

## Common Pitfalls

- Trusting Redlock for strong consistency — Kleppmann's critique still holds; use a real DB lock for money
- Storing huge blobs → memory pressure; offload to Blob/S3 with metadata in Redis
- Cache stampede on hot keys — see [`Caching`](../Caching/) (`HybridCache`)
- Persistence misconfigured (RDB-only on a cache that data-loss isn't OK) → know your durability model

## See also

- [../Caching](../Caching/) · [Stream as bus](../../Architecture/Messaging/) · [HybridCache](../Caching/HybridCacheExample.cs)
