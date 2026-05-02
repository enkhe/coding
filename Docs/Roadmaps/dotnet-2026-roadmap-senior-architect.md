# .NET 2026 Roadmap — Senior Developer / IT Architect

> A generalized, role-agnostic roadmap for engineers operating at the **Senior Developer** to **IT/Solutions Architect** level on the Microsoft stack. Designed to be cloud- and vendor-neutral within the .NET ecosystem, with explicit modernization patterns for teams still carrying legacy .NET Framework / Web Forms / WCF workloads.

---

## 0. Executive Summary

2026 is the first full calendar year on the **.NET 10 LTS** baseline. .NET 10 (released November 11, 2025, supported through November 10, 2028) is the platform target for new green-field work and the recommended landing zone for modernization. **.NET 8 and .NET 9 both reach end-of-support on November 10, 2026**, which collapses the previously fragmented LTS surface into a single supported LTS line by year-end.

Three structural shifts define the senior/architect role this year:

1. **AI is now an architectural concern, not a feature.** Vector search, RAG pipelines, agentic patterns, and LLM-aware data layers (EF Core 10's `vector` type, Semantic Kernel, Microsoft.Extensions.AI) are first-class infrastructure decisions — not bolt-ons.
2. **WebAssembly graduates to a first-class deployment target.** Blazor WASM with Native AOT, WASI workloads, and edge compute scenarios mean architects must reason about *where* C# executes (server, browser, edge, native), not just *how*.
3. **The legacy modernization window is closing.** With .NET Framework 4.8.x in extended-only servicing and most state/enterprise governance pushing zero-trust auth (e.g., OIDC over legacy SAML federations), teams that haven't begun Strangler-Fig modernization in 2026 will be doing it under duress in 2027–2028.

This roadmap is organized into **six always-on pillars**, a **quarterly progression**, a **Senior vs. Architect competency matrix**, and a **modernization pattern catalog**.

---

## 1. The 2026 Landscape at a Glance

| Domain | 2026 Baseline | What Changed |
|---|---|---|
| Runtime | .NET 10 LTS (C# 14, F# 10) | .NET 8/9 EOS Nov 10, 2026 |
| IDE | Visual Studio 2026, VS Code + C# Dev Kit, JetBrains Rider | VS 2026 GA, AI-assisted refactoring mainstream |
| Web | ASP.NET Core 10, Blazor 10 (Server / WASM / Auto / Hybrid) | Passkey auth, OpenAPI 3.1, QuickGrid maturity |
| Data | EF Core 10 LTS, Dapper, SQL Server 2025 | Native `vector` type, `json` type, hybrid search (RRF) |
| Cloud orchestration | .NET Aspire 13 | App-model-as-code becomes default for distributed apps |
| AI | Microsoft.Extensions.AI, Semantic Kernel, Azure OpenAI, ML.NET 4 | Unified `IChatClient` abstraction across providers |
| Auth | Microsoft.Identity.Web, OpenIddict, Duende IdentityServer | Passkeys, FIDO2, External ID, zero-trust default |
| Observability | OpenTelemetry (OTel) GA across .NET, Aspire dashboards | OTel is the default; vendor SDKs are the exception |
| Resilience | Polly v8 (`Microsoft.Extensions.Resilience`) | Pipeline-based, DI-native, replaces hand-rolled retry |
| Messaging | MassTransit v8, Wolverine, Azure Service Bus, Kafka | Outbox / Inbox patterns become non-negotiable |
| Testing | xUnit v3, Testcontainers, Playwright, Verify, NSubstitute | "Real dependencies in containers" is the new default |
| Deployment | Containers (Linux), Native AOT for hot paths, AKS/ACA | Distroless / chiseled images, AOT for cold-start |
| IaC | Bicep (Azure-native), Terraform (multi-cloud), Pulumi | Bicep modules registry maturity |

> See the full roadmap at the end of this document for quarterly progression, the senior vs. architect competency matrix, and the modernization pattern catalog. The detailed sections are preserved verbatim from the source document.

---

## 2. Six Pillars of Mastery

1. **Language, Runtime & Performance** — C# 14, .NET 10, Native AOT, `Span<T>`, BenchmarkDotNet
2. **Architecture & Design** — Clean / Hexagonal / Onion, DDD, CQRS, Vertical Slice, Modular Monolith
3. **Cloud-Native Engineering** — .NET Aspire, containers, ACA/AKS, IaC, edge/WASM
4. **Data & State** — EF Core 10, polyglot persistence, vector search, caching, expand-contract migrations
5. **Security & Identity** — OIDC/PKCE, Passkeys, threat modeling (STRIDE), SBOM/supply chain, zero trust
6. **Observability, Reliability & Operations** — OpenTelemetry, Polly v8, SLOs, chaos, FinOps

Each pillar maps to a top-level folder in this repo. See [the master index](../../README.md).

---

## 3. Recommended 2026 Stack (Reference)

| Concern | Default Choice (2026) |
|---|---|
| Runtime | .NET 10 LTS |
| Web | ASP.NET Core 10, Blazor 10 |
| Data | EF Core 10, Dapper (hot paths) |
| Messaging | MassTransit / Wolverine |
| Resilience | Polly v8 (`Microsoft.Extensions.Resilience`) |
| Caching | `HybridCache`, Redis |
| Auth | Microsoft.Identity.Web + Entra ID / External ID |
| Observability | OpenTelemetry → APM of choice |
| Logging | Serilog over OTel logs bridge |
| Testing | xUnit v3, Testcontainers, Playwright, Verify |
| Containers | Distroless / chiseled Ubuntu |
| Orchestration (local) | .NET Aspire 13 |
| Orchestration (prod) | Azure Container Apps, AKS |
| IaC | Bicep, Terraform |
| AI abstraction | `Microsoft.Extensions.AI` |
| AI orchestration | Semantic Kernel |
| Vector store | EF Core 10 vector, pgvector, Azure AI Search |
| Architecture tests | NetArchTest, ArchUnitNET |

---

## 4. Closing Principles

1. **Write down your decisions.** ADRs are the difference between an opinion and an architecture.
2. **Default to LTS.** .NET 10 is the floor for 2026.
3. **Prefer the modular monolith** until you have evidence to split.
4. **Make AI a feature with the same rigor** as any other.
5. **Modernize on a cadence, not a panic.** Strangler Fig in 2026 beats forklift rewrites in 2028.
6. **Performance is a feature.** Allocate a budget, measure it.
7. **Security is a default.** Zero trust, least privilege, secrets in vaults.
8. **Observability is a design constraint.** If you can't see it from telemetry, you can't operate it.
9. **Mentor in writing.** Templates, ADRs, runbooks — these scale your judgment.
10. **The architect's job is to make the team faster, safer, and more confident.**

---

*Document version: 1.0 — May 2026. Revise quarterly.*
