// HybridCache — L1 in-memory + L2 distributed + stampede protection.
// Package: Microsoft.Extensions.Caching.Hybrid
//          Microsoft.Extensions.Caching.StackExchangeRedis
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(o =>
    o.Configuration = builder.Configuration.GetConnectionString("Redis"));

builder.Services.AddHybridCache(o =>
{
    o.MaximumPayloadBytes = 1 << 20;            // 1 MiB
    o.MaximumKeyLength = 256;
    o.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2),
    };
});

builder.Services.AddSingleton<IOrderRepository, FakeOrderRepository>();
builder.Services.AddSingleton<OrderReader>();

var app = builder.Build();

app.MapGet("/orders/{id:guid}", async (Guid id, OrderReader reader, CancellationToken ct) =>
    Results.Ok(await reader.GetAsync(id, ct)));

app.MapDelete("/cache/orders/{id:guid}", async (Guid id, OrderReader reader, CancellationToken ct) =>
{
    await reader.InvalidateAsync(id, ct);
    return Results.NoContent();
});

app.Run();

public sealed record OrderDto(Guid Id, decimal Amount);
public interface IOrderRepository { Task<OrderDto?> GetAsync(Guid id, CancellationToken ct); }
internal sealed class FakeOrderRepository : IOrderRepository
{
    public Task<OrderDto?> GetAsync(Guid id, CancellationToken ct) =>
        Task.FromResult<OrderDto?>(new OrderDto(id, 99.99m));
}

public sealed class OrderReader(HybridCache cache, IOrderRepository repo)
{
    public async ValueTask<OrderDto?> GetAsync(Guid id, CancellationToken ct) =>
        await cache.GetOrCreateAsync(
            key: $"order:{id}",
            factory: async ct => await repo.GetAsync(id, ct),
            tags: [$"order:{id}", "orders"],
            cancellationToken: ct);

    public Task InvalidateAsync(Guid id, CancellationToken ct) =>
        cache.RemoveAsync($"order:{id}", ct).AsTask();

    public Task InvalidateAllOrdersAsync(CancellationToken ct) =>
        cache.RemoveByTagAsync("orders", ct).AsTask();
}
