# FullStack

> End-to-end projects spanning frontend + backend + data + infra. The portfolio that proves the breadth.

## Three subfolders by maturity

| Folder | Purpose | Bar |
|---|---|---|
| [Demos](Demos/) | Small, focused, deliberate single-feature demos | One-screen happy path; clean code |
| [Sandbox](Sandbox/) | Throwaway experiments | Zero polish; date-prefixed |
| [Portfolio](Portfolio/) | Production-grade flagship work | ADRs, tests, observability, threat model |

## Recommended capstone projects (from .NET 2026 roadmap §9)

1. **Reference distributed system** — Aspire + ASP.NET Core APIs + Blazor frontend + worker + Service Bus, fully instrumented with OTel, Bicep to ACA.
2. **Modernization showcase** — legacy Web Forms / WCF brought forward via Strangler Fig with YARP, dual auth, expand-contract migration.
3. **AI-integrated feature** — RAG against a real corpus, evaluation harness, citations, cost telemetry.
4. **Architecture artifact** — public repo with ADRs, C4, fitness functions, threat model, SBOM, runbooks.
5. **Performance case study** — slow path → profile → optimize → BenchmarkDotNet before/after.

## What makes a portfolio piece

- README with motivation, architecture (C4), and tradeoffs
- ADRs in `docs/adr/`
- CI green on tests + architecture tests + security scan
- Observability wired (OTel, dashboard screenshot in README)
- Threat model artifact
- License and conventional commits

## See also

- [../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
