# Tracing

> Causal request flow across services. .NET uses `System.Diagnostics.ActivitySource` (= OTel `Tracer`).

## Core Concepts

- **Activity = Span.** A unit of work with a start/stop, attributes, status, events.
- **TraceId / SpanId / ParentSpanId** — together form the call tree.
- **Propagation** — W3C `traceparent` + `tracestate` headers cross HTTP / message boundaries.
- **Baggage** — key/values that travel with the trace (use sparingly; not searchable like attributes).
- **Status** — `ActivityStatusCode.Error` for failures; set the description.
- **Sampling** — head-based (decide at root) vs tail-based (decide after seeing whole trace, requires Collector).

## "To Be Dangerous" Cheatsheet

| Need | Code |
|---|---|
| Source | `private static readonly ActivitySource Source = new("Orders.Api");` |
| Start span | `using var act = Source.StartActivity("place-order");` |
| Tag | `act?.SetTag("order.id", id);` |
| Status | `act?.SetStatus(ActivityStatusCode.Error, ex.Message);` |
| Event | `act?.AddEvent(new ActivityEvent("validation.failed"));` |
| Register | `.AddSource("Orders.Api")` in `WithTracing(...)` |
| Propagate manually | `Activity.Current?.Context` + `Propagators.DefaultTextMapPropagator.Inject(...)` |

## Quick Reference

```csharp
public sealed class OrderService(ActivitySource source, IOrderRepository repo)
{
    public async Task<Guid> PlaceAsync(PlaceOrder cmd, CancellationToken ct)
    {
        using var activity = source.StartActivity("Orders.Place", ActivityKind.Internal);
        activity?.SetTag("order.tier", cmd.Tier);
        activity?.SetTag("order.amount", cmd.Amount);

        try
        {
            var id = await repo.SaveAsync(cmd, ct);
            activity?.SetTag("order.id", id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return id;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}
```

## Common Pitfalls

- Using `string` interpolation instead of tags — search/filter doesn't work.
- Manually constructing trace headers — let OTel SDK propagate; don't roll your own.
- High-cardinality tags (raw payloads).
- Spans that span a whole request (`activity.Stop()` only on completion) but inner work isn't separately spanned — no granularity.
- Sampling-at-root + child-throws-error: error traces get dropped. Use `ParentBased(remote=AlwaysOn for errors)` or tail sampling.

## Examples in this folder

- [`OrderService.cs`](OrderService.cs) — typed `ActivitySource` use, status, errors

## See also

- [../OpenTelemetry](../OpenTelemetry/) · [../Logging](../Logging/) — correlation
