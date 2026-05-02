# Modular Monolith

> Multiple modules in one deployable. Each module has its own domain, schema, and public contracts. Best default for new systems in 2026.

## Core Concepts

- **Module = bounded context** — owns its domain model, persistence, and API surface
- **Public contracts only** — modules talk via published interfaces and events; *never* direct table access
- **Internal seal** — `internal` everywhere except the contract types
- **In-process events** — domain events handled in-proc; integration events serialized for outbox if external
- **Single deployment** — one CI, one release; lower ops complexity than microservices
- **Shared infra** is fine (DB host, OTel, IdP); shared *schemas* are not (own schema per module)

## When this beats microservices

- Team size < ~30 engineers
- Coupling is still being discovered
- You don't need independent scaling per module yet
- You haven't proven service boundaries

## When to extract a module to a service

- Independent scaling needs (10× compute on one module)
- Independent release cadence forced by team scale
- Different runtime requirements (e.g., GPU)
- Compliance / data isolation requirement

## Folder layout

```
src/
├── Modules/
│   ├── Orders/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   ├── Api/
│   │   ├── Contracts/                  ← public, others import
│   │   └── OrdersModule.cs             ← module bootstrap (DI, EF, endpoints)
│   ├── Billing/
│   │   ├── ...
│   │   └── BillingModule.cs
│   └── Notifications/
│       └── ...
├── Shared/
│   ├── Kernel/                         ← VOs, base classes, no business
│   └── Infrastructure/                 ← OTel, auth, error handling
└── Host/
    └── Program.cs                      ← composes modules
```

## Module bootstrap pattern

```csharp
public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection s, IConfiguration cfg)
    {
        s.AddDbContext<OrdersDbContext>(o =>
            o.UseSqlServer(cfg.GetConnectionString("Orders")));
        s.AddMediator(typeof(OrdersModule));
        // module-private services
        s.AddScoped<IOrderRepository, OrderRepository>();
        return s;
    }

    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/orders");
        grp.MapCreateOrder();
        grp.MapGetOrders();
        return app;
    }
}
```

## Enforcing boundaries

Use **NetArchTest** in CI to fail builds when a module reaches into another's internals (see `../FitnessFunctions`).

```csharp
[Fact]
public void Orders_module_does_not_reference_Billing_internals()
{
    var result = Types.InAssembly(typeof(OrdersModule).Assembly)
        .That().ResideInNamespaceStartingWith("Modules.Orders")
        .Should().NotHaveDependencyOn("Modules.Billing.Infrastructure")
        .GetResult();

    Assert.True(result.IsSuccessful, string.Join(",", result.FailingTypeNames ?? []));
}
```

## Common Pitfalls

- "Modular" but everything reaches into everyone else's tables. → enforce.
- One DbContext shared by all modules. → one per module, separate schema.
- Cyclic module dependencies. → Domain events + integration interfaces fix this.
- "We'll split later" with no boundary tests. → boundary erosion is invisible without them.

## Examples in this folder

- [`OrdersModule.cs`](OrdersModule.cs) — bootstrap pattern
- [`Program.cs`](Program.cs) — composition root
- [`OrdersBoundaryTests.cs`](OrdersBoundaryTests.cs) — fitness function

## See also

- [../VerticalSlice](../VerticalSlice/) · [../DomainDrivenDesign](../DomainDrivenDesign/) · [../FitnessFunctions](../FitnessFunctions/)
