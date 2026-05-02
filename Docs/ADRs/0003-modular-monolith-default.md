# 0003. Default new systems to a modular monolith, not microservices

- **Status:** Accepted
- **Date:** 2026-02-03
- **Deciders:** Architecture guild
- **Tags:** architecture, design

## Context and Problem Statement

For the last decade the industry default for any non-trivial green-field system has drifted toward microservices. In practice, most teams under ~50 engineers pay the operational and cognitive tax of distributed systems (network failures, eventual consistency, distributed tracing, deploy choreography) without reaping the autonomy benefits. We need an explicit org-wide default so teams stop re-litigating this on every project kickoff.

What should be the default architectural style for new systems in 2026?

## Decision Drivers

- Most new systems start with one team and unclear bounded contexts.
- Distributed systems are an order of magnitude harder to operate, observe, and debug.
- We can decompose later if needed; we cannot easily re-monolith.
- Modular code (DDD bounded contexts, vertical slice features) gives most of the modularity benefit without the network.
- .NET Aspire 13, EF Core 10, and modern testing tools make the modular monolith the most productive starting point on the .NET 2026 stack.

## Considered Options

1. **Modular monolith by default.** Split when there is evidence (autonomy, scale, deploy independence).
2. **Microservices by default.** Decompose up front along anticipated bounded contexts.
3. **No default; team's choice each time.**

## Decision Outcome

**Chosen option:** **Modular monolith by default.** Teams may propose a microservices architecture but must justify it in an ADR citing concrete drivers (independent deploy cadence, distinct scale profile, separate compliance boundary, etc.) — *not* "future-proofing."

### Positive Consequences

- One deploy unit, one process to debug, one trace, one database transaction.
- Refactoring boundaries is a code change, not a network protocol change.
- Lower infra cost; aligns with FinOps pressure.
- Faster onboarding for new engineers.

### Negative Consequences

- Teams must enforce module boundaries with discipline (ArchUnitNET, NetArchTest, project references).
- Some scenarios (multi-tenant scale isolation, regulated boundaries) still warrant separate services and require carve-outs.

## Pros and Cons of the Options

### Option 1 — Modular monolith default

- Good, lowest operational complexity for the typical team size.
- Good, preserves the option to split later (Strangler Fig).
- Bad, requires module-boundary enforcement tooling and culture.

### Option 2 — Microservices default

- Good, theoretical independent deploy + scale.
- Bad, distributed-systems tax day one; usually paid before any benefit is realized.
- Bad, premature decomposition along the wrong seams is hard to undo.

### Option 3 — No default

- Good, maximum team autonomy.
- Bad, every kickoff re-runs this debate; org-wide consistency suffers.

## Links

- [ADR 0002](./0002-use-net-10-lts-baseline.md)
- [Sam Newman, "Building Microservices" 2e](https://samnewman.io/books/building_microservices_2nd_edition/)
- [Modular Monolith Primer (Kamil Grzybek)](https://www.kamilgrzybek.com/blog/posts/modular-monolith-primer)
- Roadmap closing principle #3: "Prefer the modular monolith until you have evidence to split."
