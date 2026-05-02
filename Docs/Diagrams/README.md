# Diagrams

> Mermaid first (renders in GitHub, Markdown, MkDocs). draw.io / Excalidraw for richer visuals. Structurizr DSL for C4.

## Mermaid types worth knowing

```mermaid
flowchart LR
    A[Start] --> B{Decision}
    B -->|yes| C[Do it]
    B -->|no| D[Skip]
```

```mermaid
sequenceDiagram
    participant U as User
    participant W as Web
    participant A as API
    U->>W: POST /login
    W->>A: validate
    A-->>W: 200
    W-->>U: cookie
```

```mermaid
classDiagram
    class Order {
      +Guid Id
      +place(): void
    }
    Order --> User
```

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Paid: PaymentSucceeded
    Pending --> Failed: PaymentFailed
    Paid --> Shipped: OrderShipped
    Shipped --> [*]
    Failed --> [*]
```

```mermaid
erDiagram
    USER ||--o{ ORDER : places
    ORDER ||--|{ LINE_ITEM : contains
    USER {
      uuid id
      string email
    }
    ORDER {
      uuid id
      uuid user_id
      decimal total
    }
```

```mermaid
gantt
    title Migration plan
    dateFormat YYYY-MM-DD
    section Phase 1
    Set up YARP gateway     :a1, 2026-05-01, 14d
    Strangle /api/orders    :a2, after a1, 21d
```

```mermaid
mindmap
  root((Senior .NET 2026))
    Language
      C# 14
      .NET 10 LTS
    Architecture
      DDD
      CQRS
```

## Mermaid C4

```mermaid
C4Context
    title System Context — Orders
    Person(user, "Customer")
    System(orders, "Orders Service")
    System_Ext(payments, "Payments Provider")
    Rel(user, orders, "Places orders")
    Rel(orders, payments, "Charges card")
```

## Cheatsheet — full reference

See [`mermaid-cheatsheet.md`](mermaid-cheatsheet.md) for an extended reference.

## See also

- [../C4](../C4/) · [../Templates](../Templates/) · [../../FrontEnd/HTML](../../FrontEnd/HTML/)
