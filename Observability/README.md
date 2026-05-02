# Observability

> Logs, metrics, and traces - unified by OpenTelemetry. Vendor-neutral instrumentation, OTLP-first.

## Core Concepts

- **Three pillars**: Logs (discrete events), Metrics (aggregated numeric measurements), Traces (causal request flow across services).
- **OTel-first principle**: instrument once with OpenTelemetry; switch backends (Tempo, Jaeger, Datadog, Honeycomb, Azure Monitor) via exporter config.
- **Semantic conventions**: stable attribute names (`http.request.method`, `db.system`, `messaging.system`) so dashboards and alerts work across services.
- **OTLP**: gRPC/HTTP wire protocol every modern backend speaks. Send to a local OTel Collector; backend swap is a Collector config change, not a code change.
- **Correlation**: a single `trace_id` ties a log line to a metric exemplar to a span. Always log `trace_id` and `span_id`.
- **Observability vs monitoring**: monitoring answers known questions; observability lets you ask new questions about the system in production.

## "To Be Dangerous" Cheatsheet

| Need                                  | Tool / API                                         |
|---------------------------------------|----------------------------------------------------|
| Unified instrumentation               | `OpenTelemetry.Extensions.Hosting`                 |
| Logs                                  | `Microsoft.Extensions.Logging` + Serilog + OTel    |
| Metrics                               | `System.Diagnostics.Metrics` (`Meter`)             |
| Traces                                | `System.Diagnostics.ActivitySource`                |
| Source-gen logger                     | `[LoggerMessage]`                                  |
| Health probes                         | `Microsoft.Extensions.Diagnostics.HealthChecks`    |
| Resilience                            | Polly v8 + `Microsoft.Extensions.Resilience`       |
| Backend (local)                       | OTel Collector + Grafana/Tempo/Loki/Prometheus     |
| Backend (cloud)                       | Azure Monitor / Datadog / Honeycomb / New Relic    |

## Quick Reference

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("orders-api"))
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter())
    .WithLogging(l => l.AddOtlpExporter());
```

Set `OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317` and `OTEL_SERVICE_NAME=orders-api`.

## Common Pitfalls

- Logging PII / secrets - redact at the sink, not as an afterthought.
- High-cardinality metric labels (user IDs, request paths with parameters) - they explode storage cost.
- Sampling traces too aggressively in dev so you cannot debug, or not at all in prod so you go bankrupt.
- Forgetting to propagate W3C `traceparent` across HTTP / message bus boundaries.
- Mixing healthcheck endpoints with public traffic - put `/health/*` on a separate port or behind auth.
- Treating retries without idempotency as resilience.

## Examples in this folder

- [OpenTelemetry](./OpenTelemetry/README.md) - SDK wiring, Collector, sampling
- [Logging](./Logging/README.md) - structured logs, Serilog, source-gen
- [Metrics](./Metrics/README.md) - `Meter`, counters, histograms
- [Tracing](./Tracing/README.md) - `ActivitySource`, propagation, baggage
- [HealthChecks](./HealthChecks/README.md) - liveness/readiness, Kubernetes probes
- [Resilience](./Resilience/README.md) - Polly v8 pipelines
- [SLOs](./SLOs/README.md) - SLI/SLO/SLA, error budgets, RED/USE

## See also

- [../BackEnd/CSharp/Resilience](../BackEnd/CSharp/Resilience) - language-level resilience patterns
- [../Testing/README.md](../Testing/README.md) - tests that exercise instrumentation
- [../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
