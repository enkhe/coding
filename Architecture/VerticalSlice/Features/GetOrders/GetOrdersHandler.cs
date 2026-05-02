using Mediator;
using Microsoft.EntityFrameworkCore;
using VerticalSlice.Features.CreateOrder;

namespace VerticalSlice.Features.GetOrders;

public sealed record GetOrdersQuery(Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<IReadOnlyList<OrderDto>>;

public sealed record OrderDto(Guid Id, decimal Amount, string Tier);

public sealed class GetOrdersHandler(AppDbContext db) : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> HandleAsync(GetOrdersQuery q, CancellationToken ct) =>
        await db.Orders
            .AsNoTracking()
            .Where(o => o.UserId == q.UserId)
            .OrderByDescending(o => o.Id)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(o => new OrderDto(o.Id, o.Amount, o.Tier))
            .ToListAsync(ct);
}

public static class GetOrdersEndpoint
{
    public static void MapGetOrders(this IEndpointRouteBuilder app) =>
        app.MapGet("/orders", async (
            Guid userId, int page, int pageSize,
            IMediator mediator, CancellationToken ct) =>
        {
            var dtos = await mediator.SendAsync(new GetOrdersQuery(userId, page, pageSize), ct);
            return Results.Ok(dtos);
        }).WithName("GetOrders").WithTags("Orders");
}
