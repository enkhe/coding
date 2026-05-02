// Module bootstrap pattern — each module exposes Add* and Map* extensions.
// Internals live in Modules.Orders.* and are NOT visible outside the module.
using Microsoft.EntityFrameworkCore;

namespace Modules.Orders;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection s, IConfiguration cfg)
    {
        s.AddDbContext<OrdersDbContext>(o =>
            o.UseSqlServer(
                cfg.GetConnectionString("Orders"),
                b => b.MigrationsHistoryTable("__Migrations", schema: "orders")));

        s.AddScoped<IOrderRepository, OrderRepository>();
        return s;
    }

    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/orders").WithTags("Orders");
        grp.MapPost("/", static (Modules.Orders.Contracts.PlaceOrder _) => Results.Created());
        grp.MapGet("/{id:guid}", static (Guid id) => Results.Ok(new { id }));
        return app;
    }
}

internal sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> o) : DbContext(o)
{
    protected override void OnModelCreating(ModelBuilder b) =>
        b.HasDefaultSchema("orders");
}

internal interface IOrderRepository { }
internal sealed class OrderRepository : IOrderRepository { }

namespace Contracts
{
    // Public types — these are the seam other modules can depend on.
    public sealed record PlaceOrder(Guid UserId, decimal Amount);
    public sealed record OrderPlaced(Guid OrderId, Guid UserId, decimal Amount);
}
