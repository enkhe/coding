# .NET Aspire

> Cloud-native orchestration for .NET. Local-dev composition + service discovery + telemetry, in one place.

## Core Concepts

- **AppHost** — a console project (`Aspire.Hosting`) that *describes* your distributed app: services, databases, queues, dashboards.
- **ServiceDefaults** — a shared library every service references. Wires OpenTelemetry, health checks, service discovery, resilient HttpClient defaults.
- **Resource model** — `builder.AddProject<Projects.Api>("api")`, `builder.AddRedis("cache")`, etc. Resources can `WithReference(...)` each other; Aspire wires connection strings via env vars.
- **Dashboard** — runs at `http://localhost:18888` (default), shows logs, traces, metrics, env vars per resource.
- **Manifest publish** — `dotnet run --publisher manifest` emits a JSON description; Azure Developer CLI / `aspirate` turn it into Bicep / Helm.

## "To Be Dangerous" Cheatsheet

| What | How | When |
|---|---|---|
| Add a project | `var api = builder.AddProject<Projects.Api>("api");` | Each microservice |
| Add Redis | `var cache = builder.AddRedis("cache");` | Caching |
| Add Postgres | `var db = builder.AddPostgres("pg").AddDatabase("appdb");` | Relational store |
| Wire deps | `api.WithReference(cache).WithReference(db);` | Inject conn strings |
| External (existing) | `builder.AddConnectionString("auth")` | Pre-provisioned services |
| Use in service | `builder.AddRedisClient("cache");` | Reads connection string by name |
| Health endpoints | `app.MapDefaultEndpoints();` (in ServiceDefaults) | Adds /health, /alive |
| Run | `dotnet run --project AppHost` | Boots everything |
| Publish manifest | `dotnet run --project AppHost -- --publisher manifest --output-path aspire-manifest.json` | For deploy tooling |

## Quick Reference

```csharp
// AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var pg    = builder.AddPostgres("pg").AddDatabase("orders");

var api = builder.AddProject<Projects.OrdersApi>("orders-api")
    .WithReference(cache)
    .WithReference(pg);

builder.AddProject<Projects.Web>("web")
    .WithReference(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

```csharp
// OrdersApi/Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();          // OTel, health, discovery
builder.AddRedisClient("cache");       // resolves via Aspire
builder.AddNpgsqlDbContext<OrdersDb>("orders");

var app = builder.Build();
app.MapDefaultEndpoints();             // /health, /alive
app.MapGet("/orders", (OrdersDb db) => db.Orders.ToListAsync());
app.Run();
```

## Common Pitfalls

- AppHost is a *console* SDK project (`Microsoft.NET.Sdk`), not Web.
- The string passed to `AddProject<T>("name")` becomes the env-var prefix for service discovery — keep it stable.
- Don't ship the AppHost to prod; it orchestrates dev/F5 only.
- Forgetting `AddServiceDefaults()` in services means no OTel correlation in the dashboard.
- Aspire ships docker images for Redis/Postgres/etc. — Docker must be running locally.

## Examples in this folder

- [`AppHost.Program.cs`](AppHost.Program.cs) — composition example
- [`AppHost.csproj`](AppHost.csproj) — AppHost project file
- [`ServiceDefaults.cs`](ServiceDefaults.cs) — typical ServiceDefaults extension
- [`OrdersApi.Program.cs`](OrdersApi.Program.cs) — service that consumes Aspire resources

## See also

- [`../Resilience/`](../Resilience/README.md) — paired with HttpClient defaults
- [`../AspNetCore/`](../AspNetCore/README.md)
