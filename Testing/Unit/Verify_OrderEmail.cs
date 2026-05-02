using VerifyXunit;
using Xunit;

namespace Testing.Unit;

public sealed record OrderEmail(string Subject, string Body, string To);

public static class OrderEmailRenderer
{
    public static OrderEmail Render(Order order, string customer) => new(
        Subject: $"Your order {order.Id} is confirmed",
        Body: $"""
            Hi {customer},

            Thanks for your order. Total: {order.Total:C}.
            Status: {(order.Paid ? "PAID" : "PENDING")}.

            -- The Shop
            """,
        To: $"{customer.ToLowerInvariant()}@example.com");
}

public sealed class Verify_OrderEmail
{
    [Fact]
    public Task RendersConfirmationEmail()
    {
        var order = new Order(
            Id: Guid.Parse("00000000-0000-0000-0000-00000000abcd"),
            Total: 42.50m,
            Paid: true);

        var email = OrderEmailRenderer.Render(order, "Alice");

        // Verify writes Verify_OrderEmail.RendersConfirmationEmail.received.txt
        // on first run; rename to .verified.txt to accept the snapshot.
        return Verifier.Verify(email);
    }
}
