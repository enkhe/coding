// Direct StackExchange.Redis — for use cases HybridCache doesn't cover (pub/sub, atomic ops, scripts).
// Package: StackExchange.Redis
using System.Text.Json;
using StackExchange.Redis;

namespace Caching;

public sealed class RedisOrderCache(IConnectionMultiplexer mux)
{
    private readonly IDatabase _db = mux.GetDatabase();

    public async Task<OrderDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var json = await _db.StringGetAsync(Key(id));
        return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<OrderDto>(json!);
    }

    public Task SetAsync(OrderDto order, TimeSpan ttl, CancellationToken ct = default) =>
        _db.StringSetAsync(Key(order.Id), JsonSerializer.Serialize(order), ttl);

    // SET NX with expiry — atomic "create-if-absent". Useful for rate limiters.
    public Task<bool> TrySetIfAbsentAsync(Guid id, OrderDto order, TimeSpan ttl) =>
        _db.StringSetAsync(Key(id), JsonSerializer.Serialize(order), ttl, When.NotExists);

    // Atomic counter — for simple rate limits.
    public async Task<long> IncrAndGetAsync(string key, TimeSpan window)
    {
        var batch = _db.CreateBatch();
        var incr = batch.StringIncrementAsync(key);
        var exp = batch.KeyExpireAsync(key, window);
        batch.Execute();
        await Task.WhenAll(incr, exp);
        return incr.Result;
    }

    private static string Key(Guid id) => $"order:{id:N}";

    public sealed record OrderDto(Guid Id, decimal Amount);
}
