// RED-style metrics (Rate, Errors, Duration) for a service.
// Wire to OTel via builder.Services.AddOpenTelemetry().WithMetrics(m => m.AddMeter("Orders.Api"));
using System.Diagnostics.Metrics;

namespace Orders.Api.Observability;

public sealed class OrderMetrics
{
    public static readonly Meter Meter = new("Orders.Api", "1.0.0");

    private static readonly Counter<long> _placed =
        Meter.CreateCounter<long>(
            name: "orders.placed",
            unit: "{order}",
            description: "Total orders successfully placed.");

    private static readonly Counter<long> _failed =
        Meter.CreateCounter<long>(
            name: "orders.failed",
            unit: "{order}",
            description: "Total orders that failed.");

    private static readonly Histogram<double> _duration =
        Meter.CreateHistogram<double>(
            name: "orders.place_duration",
            unit: "ms",
            description: "End-to-end duration of placing an order.");

    private static int _inFlight;
    static OrderMetrics()
    {
        Meter.CreateObservableUpDownCounter(
            name: "orders.in_flight",
            observeValue: () => Volatile.Read(ref _inFlight),
            unit: "{order}",
            description: "Orders currently being processed.");
    }

    public void RecordPlaced(string tier, string region) =>
        _placed.Add(1,
            new KeyValuePair<string, object?>("tier", tier),
            new KeyValuePair<string, object?>("region", region));

    public void RecordFailed(string reason) =>
        _failed.Add(1, new KeyValuePair<string, object?>("reason", reason));

    public void RecordDuration(double ms, string outcome) =>
        _duration.Record(ms, new KeyValuePair<string, object?>("outcome", outcome));

    public IDisposable TrackInFlight()
    {
        Interlocked.Increment(ref _inFlight);
        return new Decrement();
    }

    private sealed class Decrement : IDisposable
    {
        public void Dispose() => Interlocked.Decrement(ref _inFlight);
    }
}
