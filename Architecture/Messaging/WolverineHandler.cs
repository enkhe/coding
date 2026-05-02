// Wolverine — handlers as plain methods, transport-agnostic. NuGet: WolverineFx.
using Wolverine;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Db")));

builder.Host.UseWolverine(opts =>
{
    opts.PersistMessagesWithPostgresql(builder.Configuration.GetConnectionString("Db")!);
    opts.UseRabbitMq("amqp://rabbit");
    opts.Policies.AutoApplyTransactions();
    opts.Policies.UseDurableLocalQueues();
    opts.Policies.UseDurableInboxOnAllListeners();
});

var app = builder.Build();

app.MapPost("/orders", async (PlaceOrder cmd, IMessageBus bus) =>
{
    await bus.SendAsync(cmd);
    return Results.Accepted();
});

app.Run();

// --- handlers ---
public sealed record PlaceOrder(Guid UserId, decimal Amount);
public sealed record OrderPlaced(Guid OrderId, Guid UserId, decimal Amount);

public static class PlaceOrderHandler
{
    // Wolverine discovers this by convention (Handle / Consume / Handles).
    public static async Task<OrderPlaced> Handle(PlaceOrder cmd, AppDbContext db, CancellationToken ct)
    {
        var order = new Order(Guid.NewGuid(), cmd.UserId, cmd.Amount);
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        return new OrderPlaced(order.Id, order.UserId, order.Amount);
    }
}

public static class OrderPlacedHandler
{
    public static void Handle(OrderPlaced evt, ILogger<Program> log) =>
        log.LogInformation("Order {OrderId} placed", evt.OrderId);
}

public sealed record Order(Guid Id, Guid UserId, decimal Amount);
public sealed class AppDbContext(DbContextOptions<AppDbContext> o) : DbContext(o)
{
    public DbSet<Order> Orders => Set<Order>();
}
