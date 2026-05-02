# Health Checks

> Liveness, readiness, and dependency probes wired to Kubernetes and load balancers.

## Core Concepts

- **Liveness** — "is the process alive?" Restart on failure. Don't include downstream deps.
- **Readiness** — "ready to receive traffic?" Pull from LB on failure. Include critical deps (DB, cache).
- **Startup** — "still booting?" Some apps need long migrations / warmup.
- **Tags** — group checks (e.g., `live`, `ready`) and expose separate endpoints per tag.
- **Reasonable timeouts** — a hung dep should not hang your probe.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Register checks | `services.AddHealthChecks()` |
| SQL Server | `.AddSqlServer(connStr, tags: ["ready"])` |
| Postgres | `.AddNpgSql(connStr, tags: ["ready"])` |
| Redis | `.AddRedis(connStr, tags: ["ready"])` |
| Service Bus | `.AddAzureServiceBusQueue(...)` |
| Custom | `.AddCheck<MyCheck>("name", tags: ["ready"])` |
| Liveness endpoint | `app.MapHealthChecks("/health/live", new() { Predicate = c => c.Tags.Contains("live") })` |
| Readiness endpoint | `app.MapHealthChecks("/health/ready", new() { Predicate = c => c.Tags.Contains("ready") })` |
| K8s probes | `livenessProbe.httpGet.path: /health/live`; `readinessProbe.httpGet.path: /health/ready` |

## Quick Reference

```csharp
builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddNpgSql(builder.Configuration.GetConnectionString("Db")!,
        name: "postgres", tags: ["ready"], timeout: TimeSpan.FromSeconds(2))
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!,
        name: "redis", tags: ["ready"], timeout: TimeSpan.FromSeconds(1));

var app = builder.Build();

app.MapHealthChecks("/health/live", new() { Predicate = c => c.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new() { Predicate = c => c.Tags.Contains("ready") });
```

## Common Pitfalls

- Putting DB checks in **liveness** — DB blip restarts pods needlessly.
- No timeout — slow probe blocks K8s control loop.
- Exposing health endpoints publicly with full diagnostics — leaks topology.
- Probes that touch every dep transitively — N+1 cascade failures.

## Examples in this folder

- [`Program.cs`](Program.cs) — endpoint setup
- [`MyCustomHealthCheck.cs`](MyCustomHealthCheck.cs) — custom `IHealthCheck`

## See also

- [../OpenTelemetry](../OpenTelemetry/) · [../../DevOps/Kubernetes](../../DevOps/Kubernetes/) — probe wiring
