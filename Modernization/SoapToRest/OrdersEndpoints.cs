// Minimal API — REST equivalent of the legacy SOAP PlaceOrder operation.
// Produces RFC 7807 ProblemDetails for errors; idempotency on POST.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

var orders = app.MapGroup("/api/orders").WithTags("Orders");

orders.MapPost("/", (
    PlaceOrderRequest req,
    HttpContext ctx,
    IOrdersService svc,
    CancellationToken ct) =>
{
    var idempotencyKey = ctx.Request.Headers["Idempotency-Key"].ToString();
    if (string.IsNullOrEmpty(idempotencyKey))
        return Results.Problem(title: "Idempotency-Key header required", statusCode: 400);

    if (req.Amount <= 0)
        return Results.Problem(title: "Invalid amount", detail: "Amount must be > 0", statusCode: 400);

    var orderId = svc.Place(req.UserId, req.Amount, idempotencyKey, ct);
    return Results.Created($"/api/orders/{orderId}", new { id = orderId, amount = req.Amount });
})
.WithName("PlaceOrder")
.Produces(201)
.ProducesProblem(400)
.RequireAuthorization();

orders.MapGet("/{id:guid}", (Guid id, IOrdersService svc) =>
{
    var dto = svc.Get(id);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.Run();

public sealed record PlaceOrderRequest(Guid UserId, decimal Amount);
public interface IOrdersService
{
    Guid Place(Guid userId, decimal amount, string idempotencyKey, CancellationToken ct);
    object? Get(Guid id);
}
