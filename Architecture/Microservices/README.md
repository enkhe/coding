# Microservices

> Independent services owning their domain and data. **A tax**, not a goal — pay it only when evidence demands.

## When microservices win

- Independent scaling needs (one service needs 10× compute)
- Independent release cadence forced by team scale (Conway's Law)
- Different runtime requirements (GPU, memory)
- Strong compliance / data isolation requirement

## When microservices lose

- Single team / < 30 engineers
- Boundaries unproven — premature decomposition is expensive to reverse
- Shared transactional data — distributed transactions are usually a mistake

## Non-negotiables (the tax)

- **Auth at every hop** (zero trust)
- **Observability everywhere** — distributed tracing or you're flying blind
- **Outbox/Inbox** for cross-service events
- **Idempotent consumers** — assume at-least-once delivery
- **Schema versioning** — events + APIs both
- **DB per service** — no shared schema
- **Service templates** — bake in cross-cutting from day 1
- **Architecture fitness functions** — enforce boundaries in CI

## Sync vs async

| Use sync (REST/gRPC) when | Use async (events) when |
|---|---|
| Caller needs the answer now | Eventual consistency OK |
| Latency budget tight | Decoupled scaling |
| Failure should fast-fail | Pub/sub / fan-out |

## Patterns to know

- **API Gateway / BFF** — see [`Architecture/ApiGateway`](../ApiGateway/)
- **Service Discovery** — DNS in K8s; Consul / Eureka outside
- **Circuit Breaker** — Polly v8 — see [`Observability/Resilience`](../../Observability/Resilience/)
- **Bulkhead** — connection pool isolation per dep
- **Saga** — see [`Architecture/Saga`](../Saga/)
- **Sidecar** — service mesh (Linkerd/Istio) for mTLS, retries, observability without code

## Common Pitfalls

- Distributed monolith — services that must deploy together
- Shared DB — coupling via DB is worse than coupling in code
- "Microservice == small" — services should match bounded contexts, not LOC limits
- One database per service... except you keep adding "just one more shared table"

## See also

- [../ModularMonolith](../ModularMonolith/) · [../DomainDrivenDesign](../DomainDrivenDesign/) · [../Messaging](../Messaging/) · [../FitnessFunctions](../FitnessFunctions/) · [../../Modernization/MonolithToMicroservices](../../Modernization/MonolithToMicroservices/)
