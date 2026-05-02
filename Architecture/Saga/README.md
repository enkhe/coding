# Saga

> Long-running business processes that span services, with compensating actions. Two flavors: **orchestration** (one boss) and **choreography** (everyone listens).

## Core Concepts

- **Saga** = a sequence of local transactions, each compensatable.
- **Compensating action** = the inverse of a step (refund, release reservation). Not always perfect; sometimes humans must.
- **Orchestration** = a central state machine drives the steps. Easier to reason about; central point of failure/coupling.
- **Choreography** = each service emits/consumes events. More decoupled; harder to see the whole flow.
- **State** must be **persistent** — the saga survives restarts.

## Decision

| Use orchestration when… | Use choreography when… |
|---|---|
| Steps are conditional / branching | Each step is independent |
| You need a single audit trail | You want loose coupling |
| Few participants | Many participants |
| Workflow rarely changes | Workflows evolve fast per-team |

## Quick Reference (orchestration with MassTransit state machine)

```csharp
public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
}

public sealed class OrderSaga : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; } = null!;
    public State Paid { get; private set; } = null!;
    public State Shipped { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<OrderSubmitted> Submitted_ { get; private set; } = null!;
    public Event<PaymentSucceeded> Paid_ { get; private set; } = null!;
    public Event<PaymentFailed> PaymentFailed_ { get; private set; } = null!;
    public Event<OrderShipped> Shipped_ { get; private set; } = null!;

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);
        Event(() => Submitted_, c => c.CorrelateById(m => m.Message.OrderId));
        Event(() => Paid_, c => c.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentFailed_, c => c.CorrelateById(m => m.Message.OrderId));
        Event(() => Shipped_, c => c.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(Submitted_)
                .Publish(ctx => new ChargePayment(ctx.Message.OrderId, ctx.Message.Amount))
                .TransitionTo(Submitted));

        During(Submitted,
            When(Paid_)
                .Then(ctx => ctx.Saga.PaidAt = DateTimeOffset.UtcNow)
                .Publish(ctx => new ShipOrder(ctx.Message.OrderId))
                .TransitionTo(Paid),
            When(PaymentFailed_)
                .TransitionTo(Failed));

        During(Paid,
            When(Shipped_)
                .Then(ctx => ctx.Saga.ShippedAt = DateTimeOffset.UtcNow)
                .Finalize());
    }
}
```

## Choreography variant (no orchestrator)

Each service handles its own piece and publishes the next event:
- `OrderService` publishes `OrderSubmitted`
- `PaymentService` consumes it, charges, publishes `PaymentSucceeded` or `PaymentFailed`
- `ShippingService` consumes `PaymentSucceeded`, publishes `OrderShipped`
- `OrderService` consumes terminal events, updates the read model

**Compensation flow:** `PaymentFailed` → `ReleaseInventory` event → `RestockHandler` releases reservations.

## Common Pitfalls

- Forgetting compensations are **best-effort** — design for partial recovery.
- Sagas that hold business logic outside the aggregates — domain logic belongs in domains.
- Long-running sagas with no timeout — orphan instances pile up. Add `Schedule` for deadlines.
- Saga as a "workflow engine" — if it's a complex BPMN, use a workflow engine (Elsa, Camunda).

## Examples in this folder

- [`OrderSaga.cs`](OrderSaga.cs) — MassTransit state machine
- [`Choreography.cs`](Choreography.cs) — event-driven variant

## See also

- [../Messaging](../Messaging/) · [../EventDriven](../EventDriven/)
