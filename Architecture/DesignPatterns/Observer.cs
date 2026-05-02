// Observer: one-to-many notification.
// In C#, use `event` for in-process push, `IObservable<T>` for stream composition,
// `Channel<T>` for producer/consumer with backpressure.

namespace Architecture.DesignPatterns.Observer;

public sealed record StockTick(string Symbol, decimal Price, DateTimeOffset At);

// 1) Idiomatic C# events
public sealed class StockTicker
{
    public event EventHandler<StockTick>? Ticked;

    public void Publish(StockTick tick) => Ticked?.Invoke(this, tick);
}

// Subscriber:
// var ticker = new StockTicker();
// ticker.Ticked += (_, t) => Console.WriteLine($"{t.Symbol} {t.Price}");

// 2) IObservable<T> for composition (Reactive Extensions)
public sealed class ObservableTicker : IObservable<StockTick>
{
    private readonly List<IObserver<StockTick>> _subs = new();

    public IDisposable Subscribe(IObserver<StockTick> observer)
    {
        _subs.Add(observer);
        return new Unsub(() => _subs.Remove(observer));
    }

    public void Publish(StockTick t)
    {
        foreach (var s in _subs.ToArray()) s.OnNext(t);
    }

    private sealed class Unsub(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}
