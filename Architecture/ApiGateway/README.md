# API Gateway

> The seam at the edge of your system — routing, auth, rate-limiting, transformation. Default in .NET land: **YARP**.

## Core Concepts

- **Single edge** — one place to enforce auth, headers, CORS, rate limits.
- **Routing** — path/host/header-based; transforms before forward.
- **BFF (Backend-for-Frontend)** — a gateway *per UI*, aggregating internal services for a specific client.
- **Strangler Fig** — gateway routes new paths to new services, rest to the legacy.
- **Don't put business logic in the gateway** — it becomes a distributed monolith.

## YARP cheatsheet

| Need | Where |
|---|---|
| Add | `services.AddReverseProxy().LoadFromConfig(cfg.GetSection("ReverseProxy"))` |
| Use | `app.MapReverseProxy()` |
| Auth | Place `app.UseAuthentication(); app.UseAuthorization();` before `MapReverseProxy()` |
| Rate limit | `app.UseRateLimiter()` before `MapReverseProxy()` |
| Transforms | `Transforms` array in route config or `AddTransforms<T>` for typed |

## Quick Reference (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(o => o.AddFixedWindowLimiter("default", c =>
{
    c.Window = TimeSpan.FromSeconds(10);
    c.PermitLimit = 100;
}));

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapReverseProxy();
app.Run();
```

## Quick Reference (appsettings.json)

```jsonc
{
  "ReverseProxy": {
    "Routes": {
      "orders": {
        "ClusterId": "orders",
        "Match": { "Path": "/api/orders/{**catch-all}" },
        "AuthorizationPolicy": "default",
        "RateLimiterPolicy": "default",
        "Transforms": [
          { "PathRemovePrefix": "/api" },
          { "RequestHeader": "X-Forwarded-For", "Append": "{RemoteIpAddress}" }
        ]
      }
    },
    "Clusters": {
      "orders": {
        "Destinations": {
          "d1": { "Address": "https://orders.internal/" }
        },
        "HealthCheck": {
          "Active": { "Enabled": true, "Interval": "00:00:10", "Path": "/health/ready" }
        }
      }
    }
  }
}
```

## BFF pattern (sample aggregating endpoint)

```csharp
app.MapGet("/me/dashboard", async (
    [FromServices] IOrdersClient orders,
    [FromServices] INotificationsClient notifs,
    ClaimsPrincipal user, CancellationToken ct) =>
{
    var (myOrders, myNotifs) = (await orders.RecentForAsync(user.Id(), ct),
                                await notifs.UnreadForAsync(user.Id(), ct));
    return Results.Ok(new { orders = myOrders, notifs = myNotifs });
});
```

## Common Pitfalls

- Putting domain rules in the gateway → distributed monolith.
- Forgetting health checks on clusters → traffic to dead pods.
- Single gateway sized for total throughput → bottleneck. Scale horizontally.
- TLS termination only at the gateway → unencrypted east-west traffic. Use mTLS internally.

## Examples in this folder

- [`Program.cs`](Program.cs) — YARP gateway setup
- [`appsettings.json`](appsettings.json) — config-driven routes
- [`BffEndpoint.cs`](BffEndpoint.cs) — aggregation example

## See also

- [../StranglerFig](../StranglerFig/) · [../Microservices](../Microservices/)
