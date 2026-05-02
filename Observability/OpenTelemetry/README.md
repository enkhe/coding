# OpenTelemetry

> Vendor-neutral instrumentation for traces, metrics, and logs. Default observability layer for .NET 10.

## Core Concepts

- **Resource** — identifies the producer (service name, version, instance, deployment.environment)
- **Signals** — Traces (`Activity`/`Span`), Metrics (`Meter`/`Instrument`), Logs (`ILogger` bridge)
- **Instrumentation** — auto (ASP.NET Core, HttpClient, EF Core, gRPC) vs manual (`ActivitySource`, `Meter`)
- **Processor** — batches/filters signals before export (BatchExportProcessor, SimpleExportProcessor)
- **Exporter** — sends to a backend; OTLP is the default protocol (gRPC `4317` / HTTP `4318`)
- **Sampler** — decides which traces are kept (`AlwaysOn`, `TraceIdRatioBased`, `ParentBased`)
- **Collector** — sidecar/agent that receives, processes, and forwards to N backends

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Wire up | `services.AddOpenTelemetry().ConfigureResource(...).WithTracing(...).WithMetrics(...).WithLogging(...)` |
| Trace export | `.AddOtlpExporter()` (defaults to `OTEL_EXPORTER_OTLP_ENDPOINT`) |
| ASP.NET Core spans | `.AddAspNetCoreInstrumentation()` |
| Outbound HTTP spans | `.AddHttpClientInstrumentation()` |
| EF Core spans | `.AddEntityFrameworkCoreInstrumentation()` |
| Runtime metrics | `.AddRuntimeInstrumentation()` |
| Process metrics | `.AddProcessInstrumentation()` |
| Custom spans | `using var act = MyActivitySource.StartActivity("op")` |
| Custom metrics | `_counter.Add(1, KeyValuePair.Create("dim","v"))` |

## Quick Reference

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService(serviceName: "orders-api", serviceVersion: "1.0.0")
        .AddAttributes([new("deployment.environment", builder.Environment.EnvironmentName)]))
    .WithTracing(t => t
        .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1)))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource("Orders.Api")
        .AddOtlpExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter("Orders.Api")
        .AddOtlpExporter())
    .WithLogging(l => l.AddOtlpExporter());
```

Env vars: `OTEL_SERVICE_NAME`, `OTEL_EXPORTER_OTLP_ENDPOINT=http://collector:4317`, `OTEL_RESOURCE_ATTRIBUTES=deployment.environment=prod`.

## Common Pitfalls

- High-cardinality metric tags (user id, raw URL) — explodes storage cost
- Forgetting `AddSource("...")` — your custom spans won't appear
- Sampler in code overrides env (`OTEL_TRACES_SAMPLER`); pick one source of truth
- Using `SimpleExportProcessor` in prod — blocks; use batch
- Logging PII inside `Activity.SetTag` — gets shipped everywhere

## Examples in this folder

- [`Program.cs`](Program.cs) — full ASP.NET Core 10 wiring
- [`otel-collector-config.yaml`](otel-collector-config.yaml) — minimal collector config

## See also

- [../Logging](../Logging/) · [../Metrics](../Metrics/) · [../Tracing](../Tracing/) · [../HealthChecks](../HealthChecks/)
