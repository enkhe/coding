# Metrics

> Aggregated numeric measurements over time. .NET 10 uses `System.Diagnostics.Metrics`.

## Core Concepts

- **Meter** — namespace for instruments (`new Meter("Orders.Api")`)
- **Instrument types:**
  - `Counter<T>` — monotonically increasing (request count)
  - `UpDownCounter<T>` — can go up/down (queue depth)
  - `Histogram<T>` — distribution (latency, payload size)
  - `ObservableCounter`/`ObservableGauge`/`ObservableUpDownCounter` — pulled by SDK at collection time
- **Tags** are dimensions; **keep cardinality bounded.** No raw user IDs / URLs / SQL.
- **RED method:** Rate, Errors, Duration — for request-driven services.
- **USE method:** Utilization, Saturation, Errors — for resources.

## "To Be Dangerous" Cheatsheet

| Need | Code |
|---|---|
| Create meter | `private static readonly Meter Meter = new("Orders.Api", "1.0.0");` |
| Counter | `private static readonly Counter<long> Placed = Meter.CreateCounter<long>("orders.placed");` |
| Histogram | `Meter.CreateHistogram<double>("orders.duration", unit: "ms");` |
| Add | `Placed.Add(1, new TagList { { "tier", tier } });` |
| Observable gauge | `Meter.CreateObservableGauge("queue.depth", () => _queue.Count);` |
| Wire to OTel | `.AddMeter("Orders.Api")` in `WithMetrics(...)` |

## Quick Reference

```csharp
public sealed class OrderMetrics
{
    public static readonly Meter Meter = new("Orders.Api", "1.0.0");

    private static readonly Counter<long> _placed =
        Meter.CreateCounter<long>("orders.placed", description: "Orders placed");

    private static readonly Histogram<double> _duration =
        Meter.CreateHistogram<double>("orders.duration", unit: "ms",
            description: "Time to place an order end-to-end");

    public void RecordPlaced(string tier) =>
        _placed.Add(1, new TagList { { "tier", tier } });

    public void RecordDuration(double ms, string outcome) =>
        _duration.Record(ms, new TagList { { "outcome", outcome } });
}
```

## Common Pitfalls

- High-cardinality tags (raw user/order/url IDs) → cost explosion.
- Recording in tight loops without batching → lock contention on instruments.
- Histograms without bucket tuning — defaults can be wrong for your latency profile.
- Forgetting to register the `Meter` name in OTel (`.AddMeter("Orders.Api")`).

## Examples in this folder

- [`OrderMetrics.cs`](OrderMetrics.cs) — RED-style metrics for an orders service

## See also

- [../OpenTelemetry](../OpenTelemetry/) · [../SLOs](../SLOs/)
