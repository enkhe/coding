# Worker Services

> Long-running background processing — message consumers, schedulers, polling jobs.

## Core Concepts

- **`BackgroundService`** — base class with `ExecuteAsync`; honors host shutdown
- **`IHostedService`** — lower-level; manage your own loop
- **Scoped services in workers** — create scope per iteration: `using var scope = sp.CreateScope()`
- **Graceful shutdown** — respect `stoppingToken`; flush state; release locks
- **Distributed work** — partition by key; use leases for single-instance jobs

## Quick Reference

```csharp
public sealed class OrderEnricher(
    IServiceScopeFactory scopeFactory,
    ILogger<OrderEnricher> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDb>();
                var enricher = scope.ServiceProvider.GetRequiredService<IEnricher>();

                var pending = await db.Orders
                    .Where(o => !o.Enriched)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var o in pending)
                {
                    await enricher.EnrichAsync(o, stoppingToken);
                    o.Enriched = true;
                }
                await db.SaveChangesAsync(stoppingToken);

                if (pending.Count == 0)
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                log.LogError(ex, "Enricher loop error");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
```

## Hosted in Worker SDK or web app

```csharp
// Worker SDK
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OrderEnricher>();
builder.Build().Run();

// Or co-host with a web app
builder.Services.AddHostedService<OrderEnricher>();
```

## Common Pitfalls

- Resolving scoped services from the root provider → captive dependency. Always create scope.
- Ignoring `stoppingToken` → 30-second forced shutdown on deploy.
- Single instance running on N pods → duplicate work. Use lease (DB row, blob, K8s lease) or partition.
- Exceptions kill the loop → wrap in try/catch + delay before retry.

## See also

- [../Resilience](../Resilience/) · [../../../Architecture/Messaging](../../../Architecture/Messaging/) · [../../../Observability/HealthChecks](../../../Observability/HealthChecks/)
