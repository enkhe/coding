# 0002. Use .NET 10 LTS as the baseline runtime for all new work

- **Status:** Accepted
- **Date:** 2026-01-22
- **Deciders:** Architecture guild, Platform engineering
- **Tags:** runtime, platform

## Context and Problem Statement

.NET 8 and .NET 9 both reach end of support on **November 10, 2026**. .NET 10 was released **November 11, 2025** as an LTS release supported through **November 10, 2028**. We need a single, defensible runtime baseline for all green-field projects in 2026 and a target for ongoing modernization of legacy .NET Framework / .NET 6 / .NET 8 workloads.

What runtime should be the default for new services and the modernization target?

## Decision Drivers

- Must be supported (no out-of-support runtimes in production).
- Must be LTS — STS releases (odd-numbered) carry only 18 months of support and force upgrade churn.
- Should consolidate the supported surface, not fragment it further.
- Must support our 2026 stack: ASP.NET Core 10, EF Core 10, .NET Aspire 13, Microsoft.Extensions.AI.
- Modernization plans should not aim at a target that is itself near EOL.

## Considered Options

1. **.NET 10 LTS (Nov 2025 - Nov 2028).**
2. **.NET 9 STS (Nov 2024 - May 2026).**
3. **.NET 8 LTS (Nov 2023 - Nov 2026).**
4. **Stay on existing mix; defer the decision to each team.**

## Decision Outcome

**Chosen option:** **.NET 10 LTS** for all new projects starting Q1 2026, and as the modernization target for legacy workloads. Existing .NET 8 services should plan migration before the November 10, 2026 EOS date.

### Positive Consequences

- Single LTS line through 2028.
- Access to C# 14, primary constructors everywhere, EF Core 10 `vector` type, ASP.NET Core 10 passkey auth, Aspire 13 app model.
- Avoids re-doing this decision in 2027.

### Negative Consequences

- Some third-party libraries lag a release cycle — we accept up to one quarter of "wait for the ecosystem" friction on net-new dependencies.
- Forces a real migration plan for every .NET 8 service in 2026.

## Pros and Cons of the Options

### Option 1 — .NET 10 LTS

- Good, longest support window of any current option.
- Good, includes every framework we depend on.
- Bad, ecosystem catch-up cost (small).

### Option 2 — .NET 9 STS

- Good, already in some services.
- Bad, EOS May 2026 — picking it now is irresponsible.

### Option 3 — .NET 8 LTS

- Good, broad library compatibility today.
- Bad, EOS November 10, 2026 — adopting it now means migrating in <12 months.

### Option 4 — Defer

- Good, no immediate effort.
- Bad, fragmented runtime estate is exactly what this decision exists to prevent.

## Links

- [.NET 10 release notes](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview)
- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy)
- [Roadmap section 1](../Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [ADR 0003](./0003-modular-monolith-default.md)
