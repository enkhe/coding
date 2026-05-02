// Choreography variant — no central orchestrator, services react to events.
// Each handler publishes the next event in the chain.
using MassTransit;

namespace Sagas.Choreography;

// 1) Orders publishes OrderSubmitted on POST /orders.

// 2) Payments handles OrderSubmitted, charges, publishes outcome.
public sealed class ChargeOnOrderSubmitted(IPaymentGateway gateway) : IConsumer<OrderSubmitted>
{
    public async Task Consume(ConsumeContext<OrderSubmitted> ctx)
    {
        try
        {
            var tx = await gateway.ChargeAsync(ctx.Message.OrderId, ctx.Message.Amount, ctx.CancellationToken);
            await ctx.Publish(new PaymentSucceeded(ctx.Message.OrderId, tx));
        }
        catch (Exception ex)
        {
            await ctx.Publish(new PaymentFailed(ctx.Message.OrderId, ex.Message));
        }
    }
}

// 3) Shipping reacts to PaymentSucceeded.
public sealed class ShipOnPaid : IConsumer<PaymentSucceeded>
{
    public Task Consume(ConsumeContext<PaymentSucceeded> ctx) =>
        ctx.Publish(new OrderShipped(ctx.Message.OrderId, "TRACK-" + Guid.NewGuid()));
}

// 4) Inventory reacts to PaymentFailed (compensation).
public sealed class ReleaseOnFailed(IInventoryService inv) : IConsumer<PaymentFailed>
{
    public async Task Consume(ConsumeContext<PaymentFailed> ctx)
    {
        await inv.ReleaseReservationAsync(ctx.Message.OrderId, ctx.CancellationToken);
    }
}

// 5) Orders updates its read model on terminal events.
public sealed class UpdateOrderOnShipped(IOrderRepository repo) : IConsumer<OrderShipped>
{
    public async Task Consume(ConsumeContext<OrderShipped> ctx)
    {
        await repo.MarkShippedAsync(ctx.Message.OrderId, ctx.Message.TrackingNumber, ctx.CancellationToken);
    }
}

public sealed class UpdateOrderOnFailed(IOrderRepository repo) : IConsumer<PaymentFailed>
{
    public Task Consume(ConsumeContext<PaymentFailed> ctx) =>
        repo.MarkFailedAsync(ctx.Message.OrderId, ctx.Message.Reason, ctx.CancellationToken);
}

public sealed record OrderSubmitted(Guid OrderId, decimal Amount);
public sealed record PaymentSucceeded(Guid OrderId, string TransactionId);
public sealed record PaymentFailed(Guid OrderId, string Reason);
public sealed record OrderShipped(Guid OrderId, string TrackingNumber);

public interface IPaymentGateway { Task<string> ChargeAsync(Guid orderId, decimal amount, CancellationToken ct); }
public interface IInventoryService { Task ReleaseReservationAsync(Guid orderId, CancellationToken ct); }
public interface IOrderRepository
{
    Task MarkShippedAsync(Guid id, string tracking, CancellationToken ct);
    Task MarkFailedAsync(Guid id, string reason, CancellationToken ct);
}
