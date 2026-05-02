// Wire it all up + use it in a Minimal API endpoint.
using FluentValidation;
using Mediator;
using Mediator.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator(typeof(Program));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.MapPost("/orders", async (CreateOrder cmd, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.SendAsync(cmd, ct);
    return Results.Created($"/orders/{id}", new { id });
});

app.MapGet("/orders/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var dto = await mediator.SendAsync(new GetOrderQuery(id), ct);
    return dto is null ? Results.NotFound() : Results.Ok(dto);
});

app.Run();

// --- slice: CreateOrder ---
public sealed record CreateOrder(Guid UserId, decimal Amount) : IRequest<Guid>;

public sealed class CreateOrderValidator : AbstractValidator<CreateOrder>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public sealed class CreateOrderHandler : IRequestHandler<CreateOrder, Guid>
{
    public Task<Guid> HandleAsync(CreateOrder request, CancellationToken ct)
        => Task.FromResult(Guid.NewGuid());
}

// --- slice: GetOrder ---
public sealed record GetOrderQuery(Guid Id) : IRequest<OrderDto?>;
public sealed record OrderDto(Guid Id, decimal Amount);
public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderDto?>
{
    public Task<OrderDto?> HandleAsync(GetOrderQuery request, CancellationToken ct)
        => Task.FromResult<OrderDto?>(new OrderDto(request.Id, 99.99m));
}
