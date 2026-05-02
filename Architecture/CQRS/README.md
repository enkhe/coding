# CQRS

> Command-Query Responsibility Segregation: separate write model (commands) from read model (queries). Use selectively.

## Core Concepts

- **Command** — intent to change state; returns nothing or a minimal id/version
- **Query** — read; returns a DTO shaped for the consumer
- **Read model** — denormalized projection optimized for queries (sometimes a different store)
- **Eventual consistency** — projections may lag the write model
- **CQRS ≠ event sourcing** — they pair well but are independent decisions

## When to use

- Read/write workload asymmetry (10× more reads than writes)
- Different shapes for read vs write (write = aggregate, read = denormalized table/cache)
- Different scale needs (cache reads massively)

## When NOT to use

- Simple CRUD — extra ceremony with no payoff
- Strong consistency required — projections lag

## Quick Reference

```csharp
// Command
public sealed record CreateOrder(Guid UserId, decimal Amount) : IRequest<Guid>;

public sealed class CreateOrderHandler(IOrderRepository repo) : IRequestHandler<CreateOrder, Guid>
{
    public async Task<Guid> HandleAsync(CreateOrder cmd, CancellationToken ct)
    {
        var order = Order.Place(cmd.UserId, cmd.Amount);
        await repo.AddAsync(order, ct);
        return order.Id;
    }
}

// Query
public sealed record GetOrders(Guid UserId) : IRequest<IReadOnlyList<OrderListItemDto>>;

public sealed class GetOrdersHandler(IDbConnectionFactory db) : IRequestHandler<GetOrders, IReadOnlyList<OrderListItemDto>>
{
    public async Task<IReadOnlyList<OrderListItemDto>> HandleAsync(GetOrders q, CancellationToken ct)
    {
        // Dapper against the read-side view — bypass EF for raw speed.
        await using var conn = db.Open();
        return (await conn.QueryAsync<OrderListItemDto>(
            "SELECT id, total, placed_at FROM v_orders_for_user WHERE user_id = @UserId",
            new { q.UserId })).AsList();
    }
}
```

## Common Pitfalls

- "All commands are events, all queries hit the same DB" — that's not CQRS.
- Maintaining two models for nothing — only worth it under workload asymmetry.
- Forgetting to handle projection lag in the UI (loading state / "your order is processing").

## See also

- [../VerticalSlice](../VerticalSlice/) · [../EventDriven](../EventDriven/) · [../../BackEnd/CSharp/Mediator](../../BackEnd/CSharp/Mediator/)
