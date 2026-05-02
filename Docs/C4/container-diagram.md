# C4 Level 2 — Container: Order Management System

> Zoom into the OMS box from Level 1. Each "container" is a separately deployable / runnable unit (a process, a database, a SPA bundle).

## Diagram

```mermaid
C4Container
    title Container Diagram — Order Management System

    Person(customer, "Customer", "Web browser")
    Person(warehouse, "Warehouse Operator", "Internal portal user")

    System_Boundary(oms, "Order Management System") {
        Container(spa, "Customer SPA", "Blazor WebAssembly + .NET 10", "Storefront UI customers use to browse and order.")
        Container(portal, "Warehouse Portal", "Blazor Server + .NET 10", "Internal UI for fulfillment staff.")
        Container(api, "Order API", "ASP.NET Core 10 Minimal API", "Public + internal HTTP API. Owns order lifecycle.")
        Container(worker, "Fulfillment Worker", ".NET 10 Worker Service", "Processes order events, talks to shipping and email.")
        ContainerDb(db, "Orders DB", "SQL Server 2025", "Authoritative store for orders, customers, products.")
        ContainerDb(cache, "Cache", "Redis", "Catalog cache and idempotency keys.")
        ContainerDb(bus, "Message Broker", "Azure Service Bus", "Order events, outbox dispatch.")
    }

    System_Ext(payments, "Payments Gateway", "Stripe")
    System_Ext(shipping, "Shipping Carriers", "Carrier APIs")
    System_Ext(email, "Email / SMS Provider", "SendGrid")

    Rel(customer, spa, "Uses", "HTTPS")
    Rel(warehouse, portal, "Uses", "HTTPS")
    Rel(spa, api, "Calls", "HTTPS / JSON")
    Rel(portal, api, "Calls", "HTTPS / JSON")
    Rel(api, db, "Reads / writes", "EF Core 10")
    Rel(api, cache, "Reads / writes", "StackExchange.Redis")
    Rel(api, bus, "Publishes order events", "AMQP")
    Rel(api, payments, "Authorises payments", "HTTPS / REST")
    Rel(bus, worker, "Delivers events", "AMQP")
    Rel(worker, shipping, "Creates shipping labels", "HTTPS / REST")
    Rel(worker, email, "Sends notifications", "HTTPS / REST")
    Rel(worker, db, "Updates fulfillment state", "EF Core 10")
```

## Reading the diagram

- The OMS becomes seven containers: two UIs, one API, one worker, three stateful stores.
- **Synchronous** edges (HTTP, EF Core) and **asynchronous** edges (AMQP) are distinct — call them out in real diagrams with a legend if reviewers are unfamiliar.
- Per [ADR 0003](../ADRs/0003-modular-monolith-default.md), the API is a modular monolith inside one process — modules (Catalog, Orders, Payments, Shipping) are project references, not separate containers.

## See also

- [system-context.md](./system-context.md) — zoom out.
- [component-diagram.md](./component-diagram.md) — zoom into the Order API container.
- [workspace.dsl](./workspace.dsl) — same model in Structurizr DSL.
