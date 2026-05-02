# Portfolio

> Flagship, production-grade projects. The bar is full SDLC, not just "it runs."

## Quality bar (from the roadmap)

Every portfolio project must demonstrate:

- [ ] **Separation of concerns** — Clean / Hexagonal / Modular Monolith
- [ ] **Domain-driven** — clear bounded context, ubiquitous language in the README
- [ ] **Comprehensive tests** — unit + integration + arch tests in CI
- [ ] **Observability** — OTel wired, dashboard screenshot in README
- [ ] **Security** — OIDC, secrets in vault, threat model artifact, SBOM in releases
- [ ] **CI/CD** — GitHub Actions or Azure DevOps; OIDC to cloud; signed images
- [ ] **IaC** — Bicep / Terraform; reproducible environments
- [ ] **Docs** — README, ADRs (`docs/adr/`), C4 diagrams, runbooks
- [ ] **License** — MIT or Apache 2.0; CONTRIBUTING; CODE_OF_CONDUCT

## Suggested capstones

### 1. `reference-distributed`

.NET Aspire + ASP.NET Core APIs + Blazor Auto frontend + worker + Service Bus + Postgres. OTel + Polly v8 + HybridCache. Bicep to Azure Container Apps.

**Demonstrates:** distributed systems literacy, modern .NET 10, observability, IaC.

### 2. `modernization-showcase`

A sample legacy .NET Framework Web Forms / WCF app, brought forward via Strangler Fig with YARP. Includes dual auth (legacy SAML + OIDC) and an expand-contract data migration.

**Demonstrates:** real-world modernization judgment, not just greenfield.

### 3. `rag-with-evals`

RAG over a real knowledge base (the .NET 2026 roadmap and supporting docs). EF Core 10 + pgvector. Microsoft.Extensions.AI for the LLM seam. Golden-set evals, regression suite, cost telemetry.

**Demonstrates:** AI as a feature with the same rigor as any other.

### 4. `arch-artifact`

A repo whose **artifact is the documentation** — ADRs, C4 diagrams, fitness function tests, threat model, SBOM, runbooks. Code is minimal; the docs are the deliverable.

**Demonstrates:** architectural communication.

### 5. `perf-case-study`

Take a slow path. Profile with PerfView / dotnet-trace. Optimize (Span/Memory/Pipelines, AOT, vectorization). Document before/after with BenchmarkDotNet.

**Demonstrates:** performance engineering depth.

## See also

- [../Demos](../Demos/) · [../Sandbox](../Sandbox/) · [../../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md](../../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
