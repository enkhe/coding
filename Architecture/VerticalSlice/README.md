# Vertical Slice Architecture

> Organize by feature, not by layer. Each slice owns its own request, handler, validator, DTO, and SQL.

## Core Concepts

- **One slice = one folder** containing everything that feature needs
- **Minimize sharing** between slices — duplication is cheaper than the wrong abstraction
- **Compose at the edge** (Minimal API endpoint) — wire request → handler → response
- **Cross-cutting concerns** (auth, logging, validation) live in pipeline behaviors, not slices
- Best paired with **CQRS** (commands and queries are separate slices) and a **mediator**

## Layered vs Vertical (the difference)

```
Layered                          Vertical Slice
src/                             src/
├── Controllers/                 ├── Features/
│   └── OrdersController.cs      │   ├── CreateOrder/
├── Services/                    │   │   ├── CreateOrderEndpoint.cs
│   └── OrderService.cs          │   │   ├── CreateOrderCommand.cs
├── Repositories/                │   │   ├── CreateOrderHandler.cs
│   └── OrderRepository.cs       │   │   ├── CreateOrderValidator.cs
├── Domain/                      │   │   └── CreateOrderResponse.cs
│   └── Order.cs                 │   ├── GetOrders/
└── ...                          │   │   ├── GetOrdersEndpoint.cs
                                 │   │   ├── GetOrdersQuery.cs
                                 │   │   └── GetOrdersHandler.cs
                                 │   └── ...
                                 ├── Domain/
                                 │   └── Order.cs       (shared invariants)
                                 └── Infrastructure/    (shared infra)
```

## Quick Reference

```csharp
// Features/CreateOrder/CreateOrderEndpoint.cs
public static class CreateOrderEndpoint
{
    public static void MapCreateOrder(this IEndpointRouteBuilder app) =>
        app.MapPost("/orders",
            async (CreateOrderCommand cmd, IMediator mediator, CancellationToken ct) =>
            {
                var id = await mediator.SendAsync(cmd, ct);
                return Results.Created($"/orders/{id}", new { id });
            })
            .WithName("CreateOrder");
}

// Features/CreateOrder/CreateOrderCommand.cs
public sealed record CreateOrderCommand(Guid UserId, decimal Amount, string Tier)
    : IRequest<Guid>;
```

## When NOT to use it

- Tiny CRUD apps — overhead doesn't pay off.
- Heavy code reuse between similar slices — better extracted to a domain service.
- Team hostile to file proliferation — preview the structure first.

## Examples in this folder

- [`Features/CreateOrder/CreateOrderEndpoint.cs`](Features/CreateOrder/CreateOrderEndpoint.cs)
- [`Features/CreateOrder/CreateOrderCommand.cs`](Features/CreateOrder/CreateOrderCommand.cs)
- [`Features/CreateOrder/CreateOrderHandler.cs`](Features/CreateOrder/CreateOrderHandler.cs)
- [`Features/CreateOrder/CreateOrderValidator.cs`](Features/CreateOrder/CreateOrderValidator.cs)
- [`Features/GetOrders/GetOrdersHandler.cs`](Features/GetOrders/GetOrdersHandler.cs)

## See also

- [../CQRS](../CQRS/) · [../../BackEnd/CSharp/Mediator](../../BackEnd/CSharp/Mediator/)
