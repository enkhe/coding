// Sender.cs - publish a message with idempotency key + session id using managed identity.
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace MyApp.Messaging;

public sealed class OrderSender(ServiceBusClient client) : IAsyncDisposable
{
    private readonly ServiceBusSender _sender = client.CreateSender("orders");

    public async Task SendAsync(OrderCreated evt, CancellationToken ct)
    {
        var msg = new ServiceBusMessage(BinaryData.FromObjectAsJson(evt))
        {
            // Idempotency: use a stable business id so receivers can dedupe.
            MessageId = evt.OrderId.ToString(),
            // Session for per-customer FIFO ordering.
            SessionId = evt.CustomerId.ToString(),
            ContentType = "application/json",
            Subject = nameof(OrderCreated),
            ApplicationProperties =
            {
                ["eventType"] = nameof(OrderCreated),
                ["schema"] = "v1"
            }
        };

        await _sender.SendMessageAsync(msg, ct);
    }

    public ValueTask DisposeAsync() => _sender.DisposeAsync();
}

public record OrderCreated(Guid OrderId, Guid CustomerId, decimal Amount);

// Wiring (Program.cs):
//   builder.Services.AddSingleton(new ServiceBusClient(
//       builder.Configuration["ServiceBus:Namespace"], new DefaultAzureCredential()));
//   builder.Services.AddSingleton<OrderSender>();
