using Mediator;

namespace VerticalSlice.Features.CreateOrder;

public static class CreateOrderEndpoint
{
    public static void MapCreateOrder(this IEndpointRouteBuilder app) =>
        app.MapPost("/orders",
            async (CreateOrderCommand cmd, IMediator mediator, CancellationToken ct) =>
            {
                var id = await mediator.SendAsync(cmd, ct);
                return Results.Created($"/orders/{id}", new { id });
            })
            .WithName("CreateOrder")
            .WithTags("Orders");
}
