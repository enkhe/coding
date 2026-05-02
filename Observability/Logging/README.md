# Logging

> Structured logging in .NET 10 — `Microsoft.Extensions.Logging` + Serilog → OTel logs bridge.

## Core Concepts

- **Structured, not stringly-typed.** Pass values as named template parameters, not interpolation.
- **Levels:** `Trace` < `Debug` < `Information` < `Warning` < `Error` < `Critical`. Default `Information`.
- **Scopes** add ambient context (correlation id, tenant) to every log inside the scope.
- **`[LoggerMessage]` source-gen logger** — zero-alloc, compile-time validated, no boxing.
- **PII redaction** at the sink, not in the call site (people forget).
- **One sink to rule them all** — Serilog → OTel logs bridge → OTLP → backend of choice.

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Inject logger | `ILogger<T> logger` (typed) |
| Log with values | `logger.LogInformation("Order {OrderId} placed by {UserId}", id, userId)` |
| Source-gen logger | `[LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "...")]` |
| Scope (ambient context) | `using (logger.BeginScope(new Dictionary<string, object> { ["TenantId"] = tenantId })) { ... }` |
| Serilog setup | `builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration))` |
| OTel logs | `builder.Logging.AddOpenTelemetry(l => l.AddOtlpExporter())` |

## Quick Reference

```csharp
// 1. Source-generated logger (preferred for hot paths)
public static partial class Log
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information,
        Message = "Order {OrderId} placed by {UserId} for {Amount:C}")]
    public static partial void OrderPlaced(ILogger logger, Guid orderId, Guid userId, decimal amount);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning,
        Message = "Payment retry {Attempt}/{Max} for {OrderId}")]
    public static partial void PaymentRetry(ILogger logger, int attempt, int max, Guid orderId);
}

// 2. Usage
Log.OrderPlaced(logger, order.Id, order.UserId, order.Total);
```

## Common Pitfalls

- `LogInformation($"User {id} logged in")` — interpolation strips structure. Use `"... {UserId} ..."` template.
- Logging exceptions without the `ex` parameter — stack trace lost.
- Logging full request bodies (PII, secrets, payment data).
- Per-request log level filters that fire late — set min-level early.
- Forgetting to flush on shutdown — async sinks lose tail logs (`await Log.CloseAndFlushAsync()`).

## Examples in this folder

- [`Log.cs`](Log.cs) — `[LoggerMessage]` source-gen patterns
- [`Program.Serilog.cs`](Program.Serilog.cs) — Serilog → OTel bridge

## See also

- [../OpenTelemetry](../OpenTelemetry/) · [../Tracing](../Tracing/) — correlation
