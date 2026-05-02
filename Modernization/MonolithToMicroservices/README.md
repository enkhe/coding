# Monolith → Microservices

> Don't unless you have evidence. Modular monolith first; microservices when scaling/teaming forces it.

## When microservices win

- Independent scaling needs (one module needs 10× compute)
- Independent release cadence forced by team scale
- Different runtime requirements (GPU, memory)
- Strong compliance / data isolation requirement

## When microservices lose

- Single team / < 30 engineers
- Service boundaries unproven — premature decomposition is expensive to reverse
- Shared transactional data — distributed transactions are usually a mistake

## Cutover sequence

```mermaid
flowchart LR
    Mono[Monolith DB + app] --> Modular[Modular monolith<br/>boundaries enforced]
    Modular --> ExtractA[Extract bounded context<br/>via Strangler]
    ExtractA --> CDC[CDC keeps both DBs in sync]
    CDC --> Cutover[Read+write switches to new service]
    Cutover --> RemoveOld[Remove old code paths]
```

## Steps

1. **Modularize first.** Refactor inside the monolith into modules with public contracts. Add fitness functions ([NetArchTest](../../Architecture/FitnessFunctions/)) to enforce.
2. **Identify the first cut.** Pick a bounded context with clear boundaries, lower-than-average coupling.
3. **Stand up the new service.** Same domain language; ACL at the boundary.
4. **Extract the data.** Expand-contract migration; CDC (Debezium-style) replicates from the old DB.
5. **Strangle traffic.** Gateway routes new paths to new service; rollback by route.
6. **Remove the old code.** Burn the bridges only after observability shows traffic stable.

## "To Be Dangerous" Cheatsheet

| Need | Tool / pattern |
|---|---|
| Module boundaries | Modular monolith + NetArchTest |
| Sync data | Debezium + Kafka, or vendor CDC |
| Sync API edges | YARP + per-route policies |
| Async events | Outbox/Inbox + Service Bus / Kafka |
| Cross-service workflows | Saga (orchestration preferred for monolith→ms transitions) |

## Common Pitfalls

- Distributed monolith — you split, but every change still requires coordinated deploys
- Shared DB across services — coupling via DB is worse than coupling in code
- No service boundary tests → boundaries erode quietly
- Premature microservices on small teams → ops complexity dwarfs the benefit
- Forgetting cross-cutting (auth, telemetry, secrets) in each new service template

## See also

- [../../Architecture/Microservices](../../Architecture/Microservices/) · [../../Architecture/ModularMonolith](../../Architecture/ModularMonolith/) · [../../Architecture/Saga](../../Architecture/Saga/)
