// Health checks wired for K8s + load balancers.
// Packages:
//   AspNetCore.HealthChecks.NpgSql
//   AspNetCore.HealthChecks.Redis
//   AspNetCore.HealthChecks.AzureServiceBus
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHealthChecks()
    // Liveness — process-level only.
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    // Readiness — critical dependencies.
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Db")!,
        name: "postgres",
        tags: ["ready"],
        timeout: TimeSpan.FromSeconds(2))
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        tags: ["ready"],
        timeout: TimeSpan.FromSeconds(1))
    .AddCheck<DiskSpaceHealthCheck>("disk", tags: ["ready"]);

var app = builder.Build();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("live"),
    AllowCachingResponses = false,
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("ready"),
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
    },
});

app.Run();
