// Orchestration saga via MassTransit state machine.
// Package: MassTransit
using MassTransit;

namespace Sagas.Orchestration;

public sealed class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset? ShippedAt { get; set; }
    public string? FailureReason { get; set; }
}

public sealed class OrderSaga : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; } = null!;
    public State Paid { get; private set; } = null!;
    public State Shipped { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<OrderSubmitted> SubmittedEvent { get; private set; } = null!;
    public Event<PaymentSucceeded> PaidEvent { get; private set; } = null!;
    public Event<PaymentFailed> FailedEvent { get; private set; } = null!;
    public Event<OrderShipped> ShippedEvent { get; private set; } = null!;

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => SubmittedEvent, c => c.CorrelateById(m => m.Message.OrderId));
        Event(() => PaidEvent, c => c.CorrelateById(m => m.Message.OrderId));
        Event(() => FailedEvent, c => c.CorrelateById(m => m.Message.OrderId));
        Event(() => ShippedEvent, c => c.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(SubmittedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.OrderId = ctx.Message.OrderId;
                    ctx.Saga.Amount = ctx.Message.Amount;
                })
                .Publish(ctx => new ChargePayment(ctx.Message.OrderId, ctx.Message.Amount))
                .TransitionTo(Submitted));

        During(Submitted,
            When(PaidEvent)
                .Then(ctx => ctx.Saga.PaidAt = DateTimeOffset.UtcNow)
                .Publish(ctx => new ShipOrder(ctx.Message.OrderId))
                .TransitionTo(Paid),
            When(FailedEvent)
                .Then(ctx => ctx.Saga.FailureReason = ctx.Message.Reason)
                // Compensating action: release inventory reservation.
                .Publish(ctx => new ReleaseInventory(ctx.Message.OrderId))
                .TransitionTo(Failed));

        During(Paid,
            When(ShippedEvent)
                .Then(ctx => ctx.Saga.ShippedAt = DateTimeOffset.UtcNow)
                .TransitionTo(Shipped)
                .Finalize());
    }
}

public sealed record OrderSubmitted(Guid OrderId, decimal Amount);
public sealed record ChargePayment(Guid OrderId, decimal Amount);
public sealed record PaymentSucceeded(Guid OrderId);
public sealed record PaymentFailed(Guid OrderId, string Reason);
public sealed record ShipOrder(Guid OrderId);
public sealed record OrderShipped(Guid OrderId);
public sealed record ReleaseInventory(Guid OrderId);
