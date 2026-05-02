# Minimal APIs

> Top-level statements, low-ceremony HTTP endpoints. .NET 10 makes them production-grade with route groups, typed results, filters, OpenAPI 3.1.

## Core Concepts

- **`MapGet/MapPost/MapPut/MapDelete`** — register endpoints
- **Route groups** — `app.MapGroup("/orders")` with shared filters/auth
- **Typed results** — `Results.Ok(...)`, `Results.NotFound()`, `Results.Problem(...)`, `Results.Created(...)`
- **Endpoint filters** — `IEndpointFilter` for cross-cutting (validation, auth)
- **Parameter binding** — by attribute (`[FromBody]`/`[FromQuery]`) or convention; complex types from JSON body by default
- **OpenAPI** — automatic via `AddOpenApi()`; metadata via `WithName`/`WithSummary`/`Produces`

## Quick Reference

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.MapOpenApi();
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages();

var orders = app.MapGroup("/orders")
    .WithTags("Orders")
    .RequireAuthorization()
    .AddEndpointFilter<ValidationFilter>();

orders.MapGet("/{id:guid}", async (Guid id, AppDb db, CancellationToken ct) =>
    await db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, ct) is { } o
        ? Results.Ok(o) : Results.NotFound())
    .WithName("GetOrder")
    .Produces<Order>()
    .ProducesProblem(404);

orders.MapPost("/", async (CreateOrder cmd, AppDb db, CancellationToken ct) =>
{
    var order = new Order(Guid.NewGuid(), cmd.UserId, cmd.Amount);
    db.Orders.Add(order);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("PlaceOrder");

app.Run();

public sealed record CreateOrder(Guid UserId, decimal Amount);
public sealed record Order(Guid Id, Guid UserId, decimal Amount);
```

## Endpoint filter

```csharp
public sealed class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        foreach (var arg in ctx.Arguments)
            if (arg is IValidatable v && !v.IsValid(out var errors))
                return Results.ValidationProblem(errors);
        return await next(ctx);
    }
}
```

## Common Pitfalls

- Missing `RequireAuthorization()` on a sensitive endpoint → public API
- Forgetting `await` on `SaveChangesAsync` → silent data loss
- Returning entities directly → coupling DB schema to public API; return DTOs
- Letting `ProblemDetails` leak stack traces in prod (`UseExceptionHandler` with sane formatter)

## See also

- [../WebApi](../WebApi/) — controller-based equivalent
- [../Mediator](../Mediator/) — for vertical slices
- [../../../Architecture/VerticalSlice](../../../Architecture/VerticalSlice/)
