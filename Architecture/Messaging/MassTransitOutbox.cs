// MassTransit + EF Core outbox + Azure Service Bus.
// Packages: MassTransit, MassTransit.EntityFrameworkCore, MassTransit.Azure.ServiceBus.Core
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Db")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();

    // Transactional outbox — same DB as your aggregates.
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox();         // publish from the outbox to the bus
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.DuplicateDetectionWindow = TimeSpan.FromMinutes(15);
    });

    x.UsingAzureServiceBus((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

app.MapPost("/orders", async (
    PlaceOrder cmd,
    AppDbContext db,
    IPublishEndpoint publisher,
    CancellationToken ct) =>
{
    // Single DB transaction commits BOTH the order AND the outbox row containing OrderPlaced.
    var order = new Order(Guid.NewGuid(), cmd.UserId, cmd.Amount);
    db.Orders.Add(order);
    await publisher.Publish(new OrderPlaced(order.Id, order.UserId, order.Amount), ct);
    await db.SaveChangesAsync(ct);

    return Results.Created($"/orders/{order.Id}", new { order.Id });
});

app.Run();

public sealed record PlaceOrder(Guid UserId, decimal Amount);
public sealed record OrderPlaced(Guid OrderId, Guid UserId, decimal Amount);

public sealed class Order(Guid id, Guid userId, decimal amount)
{
    public Guid Id { get; } = id;
    public Guid UserId { get; } = userId;
    public decimal Amount { get; } = amount;
}

public sealed class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<Order> Orders => Set<Order>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.AddInboxStateEntity();      // dedupe consumer side
        b.AddOutboxMessageEntity();   // outbox table
        b.AddOutboxStateEntity();
        base.OnModelCreating(b);
    }
}

public sealed class OrderPlacedConsumer(ILogger<OrderPlacedConsumer> log) : IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> ctx)
    {
        log.LogInformation("Order {OrderId} placed by {UserId}", ctx.Message.OrderId, ctx.Message.UserId);
        return Task.CompletedTask;
    }
}
