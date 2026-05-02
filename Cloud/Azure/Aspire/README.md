# Aspire on Azure

> Deploying .NET Aspire apps to Azure (Container Apps + supporting services). The Aspire dashboard becomes the dev-time mirror of prod topology.

## Core Concepts

- **App Host** — the orchestrator project; declares projects + dependencies (Postgres, Redis, ServiceBus, etc.)
- **Service Defaults** — shared library: OTel, health checks, resilience defaults applied to every service
- **`azd` (Azure Developer CLI)** — `azd init`, `azd provision`, `azd deploy` — provisions infra (Bicep) + deploys
- **Mapping** — Aspire model → Container Apps environment + apps + secrets + Service Bus + DBs
- **OTel in prod** — point Aspire-generated services to a Collector or Application Insights via `OTEL_EXPORTER_OTLP_ENDPOINT`

## "To Be Dangerous" Cheatsheet

| Need | How |
|---|---|
| Bootstrap | `dotnet new aspire-starter` |
| Provision + deploy | `azd up` |
| Update only code | `azd deploy <service>` |
| Redirect telemetry | `OTEL_EXPORTER_OTLP_ENDPOINT` + `OTEL_SERVICE_NAME` env vars |
| Secrets | Stored in Container Apps secrets (or Key Vault refs) |
| Connection strings | `builder.AddAzurePostgresFlexibleServer("db")` etc. |

## Quick Reference (AppHost.cs)

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var pg = builder.AddAzurePostgresFlexibleServer("orders-pg")
    .WithPasswordAuthentication()
    .AddDatabase("orders");

var redis = builder.AddAzureRedis("orders-cache");

var bus = builder.AddAzureServiceBus("orders-bus")
    .AddQueue("orders.created");

builder.AddProject<Projects.Orders_Api>("api")
    .WithReference(pg)
    .WithReference(redis)
    .WithReference(bus);

builder.AddProject<Projects.Orders_Worker>("worker")
    .WithReference(pg)
    .WithReference(bus);

builder.Build().Run();
```

## Cost & ops gotchas

- Aspire dashboard is **dev-only by default** — don't expose its endpoints to prod.
- Service Bus standard tier is fine for low-medium volume; premium for VNet integration / message size.
- Postgres Flexible Server has minimum SKU + storage → cheapest is ~$25/mo. Use `Burstable` for non-prod.
- ACA scales to zero only on consumption plan; revisions cost during transitions.

## Examples in this folder

- [`AppHost.cs`](AppHost.cs)
- [`azure.yaml`](azure.yaml) — `azd` config
- [`infra/main.bicep`](infra/main.bicep) — generated/customized bicep

## See also

- [../../../BackEnd/CSharp/Aspire](../../../BackEnd/CSharp/Aspire/) · [../ContainerApps](../ContainerApps/) · [../../../DevOps/Bicep](../../../DevOps/Bicep/)
