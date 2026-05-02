// Aggregate root.
// - Single entry point for changes inside the consistency boundary.
// - Encapsulates invariants ("placed orders cannot be modified").
// - Records domain events; the application layer dispatches them on save.

namespace Architecture.DDD;

public sealed class Order
{
    public OrderId Id { get; }
    public CustomerId CustomerId { get; }
    public string Currency { get; }
    public bool Placed { get; private set; }

    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;

    public Money Total => _lines.Aggregate(Money.Zero(Currency), (acc, l) => acc + l.LineTotal);

    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _events;
    public void ClearEvents() => _events.Clear();

    private Order(OrderId id, CustomerId customerId, string currency)
    {
        Id = id; CustomerId = customerId; Currency = currency;
    }

    public static Order Start(CustomerId customerId, string currency) =>
        new(OrderId.New(), customerId, currency);

    public void AddLine(string sku, int quantity, Money unitPrice)
    {
        EnsureMutable();
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice.Currency != Currency) throw new InvalidOperationException("currency mismatch");

        var line = new OrderLine(sku, quantity, unitPrice);
        _lines.Add(line);
        _events.Add(new OrderLineAddedEvent(Id, sku, quantity, line.LineTotal, DateTimeOffset.UtcNow));
    }

    public void Place()
    {
        EnsureMutable();
        if (_lines.Count == 0) throw new InvalidOperationException("cannot place an empty order");
        Placed = true;
        _events.Add(new OrderPlacedEvent(Id, CustomerId, Total, DateTimeOffset.UtcNow));
    }

    private void EnsureMutable()
    {
        if (Placed) throw new InvalidOperationException("order is placed; immutable");
    }
}

public sealed record OrderLine(string Sku, int Quantity, Money UnitPrice)
{
    public Money LineTotal => UnitPrice.Multiply(Quantity);
}
