# Event-Driven Architecture

> Services react to *facts* about what happened, not *requests* to do work.

## Three flavors

| Pattern | What | When |
|---|---|---|
| **Event Notification** | "X happened, look it up if you care" | Loose coupling, rare consumers |
| **Event-Carried State Transfer** | Event includes the state needed to react | Avoid synchronous lookups |
| **Event Sourcing** | The event log IS the state | Audit-heavy domains |

## Domain events vs Integration events

- **Domain event** — in-process within a bounded context. Cheap, synchronous to dispatch.
- **Integration event** — across services, via the bus. Versioned, idempotent, durable.

```
Aggregate raises domain event
   → in-process handler can update read model / fire saga step
   → mapper transforms to integration event
   → outbox table → bus → other services
```

## Quick Reference (domain event in C#)

```csharp
public sealed class Order
{
    private readonly List<DomainEvent> _events = [];
    public IReadOnlyList<DomainEvent> Events => _events;

    public static Order Place(Guid userId, decimal amount)
    {
        var o = new Order(Guid.NewGuid(), userId, amount);
        o._events.Add(new OrderPlaced(o.Id, userId, amount));
        return o;
    }
}

public sealed record OrderPlaced(Guid OrderId, Guid UserId, decimal Amount) : DomainEvent;
public abstract record DomainEvent;
```

## Common Pitfalls

- Treating domain events like integration events (transactional vs eventually consistent)
- No versioning on integration event schemas
- Pub/sub everything → traceability nightmare; reserve for genuinely async flows
- "We'll figure out replay later" → without idempotent consumers, you can't

## See also

- [../Messaging](../Messaging/) · [../Saga](../Saga/) · [../CQRS](../CQRS/)
