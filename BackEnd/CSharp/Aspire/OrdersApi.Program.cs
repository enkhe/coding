// OrdersApi/Program.cs
// Sample microservice that consumes Aspire-provided resources.

var builder = WebApplication.CreateBuilder(args);

// Wire OTel, health, discovery, resilient HttpClient defaults.
builder.AddServiceDefaults();

// Aspire injects ConnectionStrings__cache and ConnectionStrings__orders.
builder.AddRedisClient(connectionName: "cache");
builder.AddNpgsqlDbContext<OrdersDbContext>(connectionName: "orders");

builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
app.MapDefaultEndpoints();

app.MapGet("/orders/{id:int}",
    async (int id, OrdersDbContext db, CancellationToken ct) =>
        await db.Orders.FindAsync([id], ct) is { } o ? Results.Ok(o) : Results.NotFound());

app.Run();

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> opts) : DbContext(opts)
{
    public DbSet<Order> Orders => Set<Order>();
}

public sealed record Order(int Id, decimal Total);
