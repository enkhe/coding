# C4 Model

> Simon Brown's C4 model: zoom-able architecture diagrams at four levels of abstraction — Context, Container, Component, Code — kept as code via Mermaid or Structurizr DSL.

## Core Concepts

The **C4 model** answers "what diagram should I draw?" with a fixed set of four levels. You almost always need the first three; the fourth is usually code itself.

| Level | Audience | Shows | When to use |
|---|---|---|---|
| **1. System Context** | Anyone, including non-technical | The system as one box, its users, and the external systems it talks to. | Always. First diagram on any project. |
| **2. Container** | Engineers, architects | Deployable / runnable units (web app, API, database, queue, SPA). | Always. The most useful level for day-to-day architecture. |
| **3. Component** | Engineers on the system | Major logical components inside one container (controllers, services, repositories, modules). | When a container is non-trivial. |
| **4. Code** | Engineers in the file | Classes, interfaces, calls. Usually generated from code (don't draw by hand). | Rarely. UML class diagram or IDE-generated. |

**Supplementary diagrams** that complement C4: Deployment (where containers run on infra), Dynamic (sequence-style for a specific flow), Landscape (multiple systems together).

### Mermaid C4 syntax

Mermaid supports `C4Context`, `C4Container`, `C4Component`, `C4Dynamic`, `C4Deployment`. Core primitives:

```text
Person(alias, "Label", "Description")
System(alias, "Label", "Description")
System_Ext(alias, "Label", "Description")
Container(alias, "Label", "Tech", "Description")
ContainerDb(alias, "Label", "Tech", "Description")
Component(alias, "Label", "Tech", "Description")
Rel(from, to, "Label", "Tech")
Boundary(alias, "Label") { ... }
```

### Structurizr DSL

Structurizr is the canonical "C4 as code" tool. One DSL file -> Context, Container, Component, Deployment, and Dynamic views. See [workspace.dsl](./workspace.dsl).

## "To Be Dangerous" Cheatsheet

| Want to... | Reach for |
|---|---|
| Show "what does this system do for whom?" | [system-context.md](./system-context.md) |
| Show "what are the moving parts?" | [container-diagram.md](./container-diagram.md) |
| Show "what's inside the API service?" | [component-diagram.md](./component-diagram.md) |
| Generate all views from one source | [workspace.dsl](./workspace.dsl) (Structurizr) |
| Sketch a one-off flow | mermaid `sequenceDiagram` (see [Diagrams/](../Diagrams/README.md)) |

### Worked example: an Order Management System

Throughout this folder we use the same example — an **Order Management System (OMS)** for an online retailer. It has:

- **Customers** placing orders via a web SPA.
- **Warehouse staff** managing fulfillment via an internal portal.
- A **Payments** external system (Stripe).
- A **Shipping** external system (carrier APIs).
- An **email/notification** external system (SendGrid).

The three C4 diagrams in this folder progressively zoom in on this system.

## Quick Reference / Examples in this folder

- [system-context.md](./system-context.md) — Level 1: OMS in its environment.
- [container-diagram.md](./container-diagram.md) — Level 2: SPA, API, worker, DB, cache, queue.
- [component-diagram.md](./component-diagram.md) — Level 3: components inside the Order API.
- [workspace.dsl](./workspace.dsl) — Structurizr DSL covering all three levels.

## See also

- [C4 model official site](https://c4model.com/)
- [Mermaid C4 docs](https://mermaid.js.org/syntax/c4.html)
- [Structurizr DSL](https://docs.structurizr.com/dsl)
- [Diagrams/](../Diagrams/README.md) for non-C4 mermaid usage.
