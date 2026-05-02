# Mermaid Cheatsheet

> One working example per diagram type. Copy and adapt.

## 1. Flowchart

```mermaid
flowchart TD
    Start([Start]) --> Auth{Authenticated?}
    Auth -- No --> Login[/Redirect to login/]
    Auth -- Yes --> Action[Render dashboard]
    Action --> End([End])
    Login --> Auth
```

Directions: `TB` (top-bottom), `TD` (same), `BT`, `LR`, `RL`. Node shapes: `[]` rect, `()` round, `([])` stadium, `[[]]` subroutine, `[()]` cylinder, `(())` circle, `{}` diamond, `[/.../]` parallelogram.

## 2. Sequence

```mermaid
sequenceDiagram
    autonumber
    participant U as User
    participant S as SPA
    participant A as API
    participant D as DB

    U->>S: Click "Place order"
    S->>A: POST /orders
    A->>D: BEGIN TX
    A->>D: INSERT order
    A->>D: INSERT outbox event
    A->>D: COMMIT
    A-->>S: 201 Created
    S-->>U: Show confirmation
```

Use `->>` for sync, `-->>` for response, `-)` for async fire-and-forget. `Note over A,D: ...` for callouts. `loop`, `alt/else`, `par/and` for control flow.

## 3. Class

```mermaid
classDiagram
    class Order {
        +Guid Id
        +CustomerId CustomerId
        +Money Total
        +OrderStatus Status
        +Place() Result
        +Cancel(reason) Result
    }
    class OrderStatus {
        <<enumeration>>
        Pending
        Paid
        Shipped
        Cancelled
    }
    class IOrderRepository {
        <<interface>>
        +Add(order) Task
        +GetById(id) Task~Order~
    }
    Order --> OrderStatus
    IOrderRepository ..> Order : returns
```

## 4. State

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Paid: payment captured
    Pending --> Cancelled: timeout / user cancel
    Paid --> Shipped: warehouse dispatch
    Shipped --> Delivered: carrier confirm
    Paid --> Refunded: refund issued
    Cancelled --> [*]
    Delivered --> [*]
    Refunded --> [*]
```

## 5. Entity-Relationship

```mermaid
erDiagram
    CUSTOMER ||--o{ ORDER : places
    ORDER ||--|{ ORDER_LINE : contains
    PRODUCT ||--o{ ORDER_LINE : "appears in"
    CUSTOMER {
        guid id PK
        string email UK
        string name
    }
    ORDER {
        guid id PK
        guid customer_id FK
        decimal total
        string status
        datetime placed_at
    }
    ORDER_LINE {
        guid id PK
        guid order_id FK
        guid product_id FK
        int quantity
        decimal unit_price
    }
    PRODUCT {
        guid id PK
        string sku UK
        string name
        decimal price
    }
```

Cardinality: `||--||` one-to-one, `||--o{` one-to-many, `}o--o{` many-to-many; `o` = zero, `|` = exactly one.

## 6. Gantt

```mermaid
gantt
    title .NET 10 modernization plan
    dateFormat  YYYY-MM-DD
    axisFormat  %b %Y
    section Inventory
    Audit legacy services      :done, a1, 2026-01-05, 2w
    section Strangler Fig
    Auth gateway in front       :active, b1, 2026-01-19, 4w
    Migrate Catalog module     :b2, after b1, 6w
    Migrate Orders module      :b3, after b2, 8w
    section Cutover
    Decommission .NET 4.8 host :milestone, c1, 2026-09-01, 0d
```

## 7. Mindmap

```mermaid
mindmap
  root((.NET 2026))
    Runtime
      .NET 10 LTS
      C# 14
      Native AOT
    Architecture
      Modular monolith
      DDD
      CQRS
    Cloud
      Aspire 13
      ACA / AKS
      Bicep / Terraform
    Data
      EF Core 10
      vector
      HybridCache
    AI
      M.E.AI
      Semantic Kernel
    Observability
      OpenTelemetry
      Polly v8
```

## 8. Journey

```mermaid
journey
    title Customer places an order
    section Discover
      Search catalog: 4: Customer
      View product: 5: Customer
    section Buy
      Add to cart: 5: Customer
      Checkout: 3: Customer, API
      Pay: 2: Customer, Payments
    section Post-purchase
      Receive email: 5: Customer, Email
      Track shipment: 4: Customer
```

## 9. C4 (Container)

```mermaid
C4Container
    title Order Management — Container view
    Person(c, "Customer")
    System_Boundary(oms, "OMS") {
        Container(spa, "SPA", "Blazor WASM")
        Container(api, "API", "ASP.NET Core 10")
        ContainerDb(db, "DB", "SQL Server 2025")
    }
    Rel(c, spa, "Uses", "HTTPS")
    Rel(spa, api, "Calls", "HTTPS")
    Rel(api, db, "Reads/writes", "EF Core 10")
```

See [Docs/C4](../C4/README.md) for full Context/Container/Component diagrams.

## 10. Pie

```mermaid
pie title Time spent in incident response (Q1 2026)
    "Detect" : 12
    "Triage" : 25
    "Mitigate" : 38
    "Postmortem" : 25
```

## 11. Git Graph

```mermaid
gitGraph
    commit id: "init"
    branch feature/orders
    checkout feature/orders
    commit id: "scaffold orders module"
    commit id: "add tests"
    checkout main
    merge feature/orders
    commit id: "v1.0.0" tag: "v1.0.0"
```

## See also

- [Mermaid live editor](https://mermaid.live)
- [README.md](./README.md) for embed-vs-file guidance.
