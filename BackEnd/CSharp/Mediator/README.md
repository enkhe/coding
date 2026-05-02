# Mediator

> Hand-rolled in-process mediator for commands/queries/notifications + pipeline behaviors. MediatR v13+ is now paid; build your own or use Wolverine.

## Core Concepts

- **Request / Handler** — `IRequest<TResponse>` + `IRequestHandler<TRequest, TResponse>`
- **Notification / Handlers** — fire-and-forget, multiple handlers per event
- **Pipeline behaviors** — cross-cutting (logging, validation, transactions, caching) wrap the handler
- **Why use it?** Vertical-slice clarity, decoupled wiring, single seam for cross-cutting concerns
- **Why skip it?** Sometimes a direct service call is clearer. Don't add ceremony to ceremony.

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| Command | `record CreateOrder(...) : IRequest<Guid>` |
| Query | `record GetOrder(Guid Id) : IRequest<OrderDto?>` |
| Notification | `record OrderPlaced(Guid Id) : INotification` |
| Pipeline behavior | `class LoggingBehavior<TReq,TRes> : IPipelineBehavior<TReq,TRes>` |
| Validation behavior | use FluentValidation; throw `ValidationException` to short-circuit |

## Quick Reference

```csharp
public sealed record CreateOrder(Guid UserId, decimal Amount) : IRequest<Guid>;

public sealed class CreateOrderHandler(IOrderRepository repo, IMediator mediator)
    : IRequestHandler<CreateOrder, Guid>
{
    public async Task<Guid> HandleAsync(CreateOrder cmd, CancellationToken ct)
    {
        var order = Order.Place(cmd.UserId, cmd.Amount);
        await repo.AddAsync(order, ct);
        await mediator.PublishAsync(new OrderPlaced(order.Id), ct);
        return order.Id;
    }
}
```

## Common Pitfalls

- Massive God-handlers — one slice, one job. Refactor when a handler grows past ~50 lines.
- Commands returning rich domain objects — return DTOs/IDs.
- Behaviors that swallow exceptions — let them bubble; observability layer captures them.
- Async-void notification handlers — use `Task` and `await` them all.

## Examples in this folder

- [`Mediator.cs`](Mediator.cs) — minimal mediator + behaviors (no external dep)
- [`Behaviors.cs`](Behaviors.cs) — logging, validation, transaction
- [`Usage.cs`](Usage.cs) — DI registration + endpoint use

## See also

- [../../../Architecture/VerticalSlice](../../../Architecture/VerticalSlice/)
- [../../../Architecture/CQRS](../../../Architecture/CQRS/)
