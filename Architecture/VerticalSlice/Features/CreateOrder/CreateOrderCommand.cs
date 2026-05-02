using Mediator;

namespace VerticalSlice.Features.CreateOrder;

public sealed record CreateOrderCommand(Guid UserId, decimal Amount, string Tier)
    : IRequest<Guid>;
