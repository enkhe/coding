using Mediator;
using Microsoft.EntityFrameworkCore;

namespace VerticalSlice.Features.CreateOrder;

public sealed class CreateOrderHandler(AppDbContext db, ILogger<CreateOrderHandler> log)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateOrderCommand cmd, CancellationToken ct)
    {
        var order = Order.Place(cmd.UserId, cmd.Amount, cmd.Tier);
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        log.LogInformation("Order {OrderId} placed", order.Id);
        return order.Id;
    }
}

// Domain object — would normally live in /Domain.
public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Tier { get; private set; } = "";

    public static Order Place(Guid userId, decimal amount, string tier) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Amount = amount,
        Tier = tier,
    };
}

public sealed class AppDbContext(DbContextOptions<AppDbContext> o) : DbContext(o)
{
    public DbSet<Order> Orders => Set<Order>();
}
