# C4 Level 3 — Component: Order API

> Zoom into the **Order API** container. Components are major logical groupings inside one process — typically aligned to bounded contexts in a modular monolith.

## Diagram

```mermaid
C4Component
    title Component Diagram — Order API (modular monolith)

    Container(spa, "Customer SPA", "Blazor WASM")
    ContainerDb(db, "Orders DB", "SQL Server 2025")
    ContainerDb(bus, "Message Broker", "Azure Service Bus")
    System_Ext(payments, "Payments Gateway", "Stripe")

    Container_Boundary(api, "Order API (ASP.NET Core 10)") {
        Component(endpoints, "HTTP Endpoints", "Minimal APIs", "Public REST + OpenAPI surface. AuthN/AuthZ middleware.")
        Component(catalog, "Catalog Module", "C# 14, EF Core 10", "Products, prices, availability.")
        Component(orders, "Orders Module", "C# 14, EF Core 10", "Cart, order placement, status, history.")
        Component(paymentsMod, "Payments Module", "C# 14", "Wraps the payments gateway, idempotent.")
        Component(outbox, "Outbox Dispatcher", "Background Service", "Publishes domain events to the broker, exactly-once.")
        Component(telemetry, "Telemetry", "OpenTelemetry", "Traces, metrics, logs across modules.")
    }

    Rel(spa, endpoints, "Calls", "HTTPS / JSON")
    Rel(endpoints, catalog, "Reads catalog")
    Rel(endpoints, orders, "Places / queries orders")
    Rel(orders, paymentsMod, "Authorises payment")
    Rel(paymentsMod, payments, "Charges card", "HTTPS")
    Rel(orders, db, "Persists orders", "EF Core 10")
    Rel(catalog, db, "Reads products", "EF Core 10")
    Rel(orders, outbox, "Writes domain events", "in-process")
    Rel(outbox, bus, "Publishes events", "AMQP")
    Rel(endpoints, telemetry, "Emits", "OTel")
    Rel(orders, telemetry, "Emits", "OTel")
```

## Reading the diagram

- Each module is a project / namespace inside the same deployable. Boundaries are enforced with NetArchTest / ArchUnitNET.
- **Outbox pattern** is explicit: the Orders module never publishes to the broker directly; it writes events to its own DB, and the dispatcher relays them. This gives exactly-once semantics across the DB+broker boundary.
- **Telemetry** is shown as a component to make observability a first-class concern (per roadmap principle #8).

## See also

- [container-diagram.md](./container-diagram.md) — zoom out.
- [ADR 0003](../ADRs/0003-modular-monolith-default.md) — why modular monolith.
- [workspace.dsl](./workspace.dsl) — same components in Structurizr DSL.
