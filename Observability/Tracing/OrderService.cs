// Tracing example — typed ActivitySource, status, errors, events.
// Register source in OTel: .WithTracing(t => t.AddSource("Orders.Api"))
using System.Diagnostics;

namespace Orders.Api.Application;

public sealed class OrderService(ActivitySource source)
{
    public async Task<Guid> PlaceAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {
        using var activity = source.StartActivity("Orders.Place", ActivityKind.Internal);
        activity?.SetTag("order.tier", cmd.Tier);
        activity?.SetTag("order.amount", cmd.Amount);
        activity?.SetTag("user.id", cmd.UserId);

        try
        {
            await ValidateAsync(cmd, ct);
            var id = await PersistAsync(cmd, ct);
            await PublishEventAsync(id, ct);

            activity?.SetTag("order.id", id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return id;
        }
        catch (ValidationException ex)
        {
            activity?.AddEvent(new ActivityEvent("validation.failed",
                tags: [.. ex.Errors.Select(e => new KeyValuePair<string, object?>("error", e))]));
            activity?.SetStatus(ActivityStatusCode.Error, "validation");
            throw;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
    }

    private async Task ValidateAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {
        using var act = source.StartActivity("Orders.Validate", ActivityKind.Internal);
        await Task.Delay(10, ct);
    }

    private async Task<Guid> PersistAsync(PlaceOrderCommand cmd, CancellationToken ct)
    {
        using var act = source.StartActivity("Orders.Persist", ActivityKind.Client);
        act?.SetTag("db.system", "postgresql");
        act?.SetTag("db.operation", "INSERT");
        await Task.Delay(20, ct);
        return Guid.NewGuid();
    }

    private async Task PublishEventAsync(Guid orderId, CancellationToken ct)
    {
        using var act = source.StartActivity("Orders.Publish", ActivityKind.Producer);
        act?.SetTag("messaging.system", "azureservicebus");
        act?.SetTag("messaging.destination.name", "orders.placed");
        await Task.Delay(5, ct);
    }
}

public sealed record PlaceOrderCommand(Guid UserId, string Tier, decimal Amount);
public sealed class ValidationException(IEnumerable<string> errors) : Exception
{
    public IReadOnlyList<string> Errors { get; } = [.. errors];
}
