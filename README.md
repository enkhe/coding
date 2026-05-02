# Coding — Reference Library & Cheatsheets

> Personal reference repo: high-level concepts, "to be dangerous" cheatsheets, and runnable code examples across every major domain a senior .NET engineer / IT architect touches in 2026.

Optimized for **quick lookup** and **self-study**. Aligned with the [.NET 2026 Senior/Architect Roadmap](Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md).

---

## How to read this repo

Each domain folder contains:

- `README.md` — concepts, "to be dangerous" cheatsheet, quick-reference syntax
- Code examples organized by sub-topic
- Links to deeper subfolders or external resources

Naming conventions:

- `*.cs`, `*.csproj` — .NET / C# 14 / .NET 10
- `*.ts`, `*.tsx` — TypeScript / React
- `*.razor` — Blazor
- `*.sql` — T-SQL / PL/pgSQL
- `*.bicep`, `*.tf`, `*.yaml` — IaC / pipelines
- `Program.cs` or `main.*` — runnable entrypoints

---

## Master Index

### 1 — Language, Runtime & Performance

- [BackEnd/CSharp](BackEnd/CSharp/) — .NET 10, C# 14, ASP.NET Core, Minimal APIs, Workers, WCF (legacy)
  - [CSharp14](BackEnd/CSharp/CSharp14/) — language features (field-backed props, extension members, partial events)
  - [AsyncPatterns](BackEnd/CSharp/AsyncPatterns/) — `ValueTask`, `IAsyncEnumerable`, cancellation discipline
  - [Performance](BackEnd/CSharp/Performance/) — `Span<T>`, `Memory<T>`, `Pipelines`, BenchmarkDotNet
  - [NativeAOT](BackEnd/CSharp/NativeAOT/) — when to AOT, what it costs
  - [SourceGenerators](BackEnd/CSharp/SourceGenerators/) — Roslyn source gens
  - [Aspire](BackEnd/CSharp/Aspire/) — distributed app model
  - [Resilience](BackEnd/CSharp/Resilience/) — Polly v8 pipelines
  - [Mediator](BackEnd/CSharp/Mediator/) — MediatR / Wolverine handlers
  - [AspNetCore](BackEnd/CSharp/AspNetCore/), [MinimalApi](BackEnd/CSharp/MinimalApi/), [WebApi](BackEnd/CSharp/WebApi/), [Workers](BackEnd/CSharp/Workers/), [ConsoleApps](BackEnd/CSharp/ConsoleApps/), [ClassLibraries](BackEnd/CSharp/ClassLibraries/), [WCF (legacy)](BackEnd/CSharp/WCF/)
- [BackEnd/Python](BackEnd/Python/) — FastAPI, Django, Flask
- [BackEnd/Node](BackEnd/Node/) — Express, Fastify, NestJS
- [BackEnd/Go](BackEnd/Go/), [BackEnd/Java](BackEnd/Java/), [BackEnd/Rust](BackEnd/Rust/)

### 2 — Frontend & UI

- [FrontEnd/Blazor](FrontEnd/Blazor/) — Server / WASM / Auto / Hybrid
- [FrontEnd/React](FrontEnd/React/) — Hooks, Vite, NextJS
- [FrontEnd/Angular](FrontEnd/Angular/), [FrontEnd/Vue](FrontEnd/Vue/), [FrontEnd/Svelte](FrontEnd/Svelte/)
- [FrontEnd/JavaScript](FrontEnd/JavaScript/), [FrontEnd/TypeScript](FrontEnd/TypeScript/)
- [FrontEnd/HTML](FrontEnd/HTML/), [FrontEnd/CSS](FrontEnd/CSS/), [FrontEnd/CSSFrameworks](FrontEnd/CSSFrameworks/) (Tailwind, Bootstrap, SCSS)
- [FrontEnd/StateManagement](FrontEnd/StateManagement/), [FrontEnd/Performance](FrontEnd/Performance/), [FrontEnd/Testing](FrontEnd/Testing/), [FrontEnd/Accessibility](FrontEnd/Accessibility/)
- [FrontEnd/WebForms](FrontEnd/WebForms/) — legacy reference
- [FrontEnd/jQuery](FrontEnd/jQuery/) — legacy reference

### 3 — Architecture & Design

- [Architecture/SOLID](Architecture/SOLID/), [Architecture/DesignPatterns](Architecture/DesignPatterns/)
- [Architecture/CleanArchitecture](Architecture/CleanArchitecture/), [Architecture/HexagonalArchitecture](Architecture/HexagonalArchitecture/)
- [Architecture/DomainDrivenDesign](Architecture/DomainDrivenDesign/), [Architecture/CQRS](Architecture/CQRS/), [Architecture/EventDriven](Architecture/EventDriven/)
- [Architecture/VerticalSlice](Architecture/VerticalSlice/), [Architecture/ModularMonolith](Architecture/ModularMonolith/), [Architecture/Microservices](Architecture/Microservices/)
- [Architecture/ApiGateway](Architecture/ApiGateway/) — YARP, BFF
- [Architecture/Messaging](Architecture/Messaging/) — MassTransit, Outbox/Inbox, Wolverine
- [Architecture/Saga](Architecture/Saga/), [Architecture/StranglerFig](Architecture/StranglerFig/)
- [Architecture/FitnessFunctions](Architecture/FitnessFunctions/) — NetArchTest, ArchUnitNET

### 4 — Data & State

- [Database/SqlServer](Database/SqlServer/) — schemas, indexes, procs, functions, views, migrations
- [Database/EntityFramework](Database/EntityFramework/) — EF Core 10
- [Database/Dapper](Database/Dapper/) — micro-ORM hot paths
- [Database/PostgreSQL](Database/PostgreSQL/), [Database/MySQL](Database/MySQL/), [Database/SQLite](Database/SQLite/)
- [Database/MongoDB](Database/MongoDB/), [Database/Redis](Database/Redis/)
- [Database/Caching](Database/Caching/) — `HybridCache`, distributed cache, stampede protection
- [Database/VectorDb](Database/VectorDb/) — pgvector, Qdrant, Azure AI Search

### 5 — AI / ML

- [AI-ML/LLMs](AI-ML/LLMs/), [AI-ML/MicrosoftExtensionsAI](AI-ML/MicrosoftExtensionsAI/), [AI-ML/SemanticKernel](AI-ML/SemanticKernel/)
- [AI-ML/RAG](AI-ML/RAG/), [AI-ML/VectorSearch](AI-ML/VectorSearch/), [AI-ML/Agents](AI-ML/Agents/)
- [AI-ML/PromptEngineering](AI-ML/PromptEngineering/), [AI-ML/Evaluation](AI-ML/Evaluation/)
- [AI-ML/MachineLearning](AI-ML/MachineLearning/), [AI-ML/MLNet](AI-ML/MLNet/), [AI-ML/DeepLearning](AI-ML/DeepLearning/)
- [AI-ML/NLP](AI-ML/NLP/), [AI-ML/ComputerVision](AI-ML/ComputerVision/), [AI-ML/MLOps](AI-ML/MLOps/)
- [AI-ML/Notebooks](AI-ML/Notebooks/) — Jupyter / Polyglot Notebooks

### 6 — Cloud-Native Engineering

- [Cloud/Azure](Cloud/Azure/) — App Service, Container Apps, AKS, Functions, Service Bus, Storage, Key Vault, Entra ID, AI Search, SQL Database, Aspire
- [Cloud/AWS](Cloud/AWS/), [Cloud/GCP](Cloud/GCP/)
- [Cloud/FinOps](Cloud/FinOps/) — tagging, cost dashboards, scale-to-zero

### 7 — DevOps / Platform

- [DevOps/Docker](DevOps/Docker/) — multi-stage, distroless, chiseled
- [DevOps/Kubernetes](DevOps/Kubernetes/), [DevOps/Helm](DevOps/Helm/), [DevOps/GitOps](DevOps/GitOps/)
- [DevOps/Bicep](DevOps/Bicep/), [DevOps/Terraform](DevOps/Terraform/), [DevOps/Pulumi](DevOps/Pulumi/), [DevOps/ArmTemplates](DevOps/ArmTemplates/)
- [DevOps/GitHubActions](DevOps/GitHubActions/), [DevOps/AzureDevOps](DevOps/AzureDevOps/), [DevOps/Jenkins](DevOps/Jenkins/)

### 8 — Security & Identity

- [Security/Authentication](Security/Authentication/) — [OIDC](Security/Authentication/OpenIdConnect/), [OAuth2](Security/Authentication/OAuth2/), [JWT](Security/Authentication/JWT/), [SAML](Security/Authentication/SAML/), [Passkeys/FIDO2](Security/Authentication/Passkeys/), [Entra](Security/Authentication/Entra/), [DualAuth](Security/Authentication/DualAuth/)
- [Security/Authorization](Security/Authorization/) — RBAC, ABAC, policy
- [Security/Cryptography](Security/Cryptography/), [Security/DataProtection](Security/DataProtection/)
- [Security/SecretsManagement](Security/SecretsManagement/) — Key Vault, Secrets Manager
- [Security/OWASP](Security/OWASP/) — Top 10 patterns
- [Security/ThreatModeling](Security/ThreatModeling/) — STRIDE, DREAD
- [Security/SupplyChain](Security/SupplyChain/) — SBOM (CycloneDX), Sigstore, SLSA
- [Security/ZeroTrust](Security/ZeroTrust/)

### 9 — Observability, Reliability & Operations

- [Observability/OpenTelemetry](Observability/OpenTelemetry/) — traces, metrics, logs
- [Observability/Logging](Observability/Logging/) — Serilog, structured logging
- [Observability/Metrics](Observability/Metrics/), [Observability/Tracing](Observability/Tracing/)
- [Observability/HealthChecks](Observability/HealthChecks/)
- [Observability/Resilience](Observability/Resilience/) — Polly v8 pipelines (cross-link to BackEnd/CSharp/Resilience)
- [Observability/SLOs](Observability/SLOs/) — error budgets, runbooks

### 10 — Testing

- [Testing/Unit](Testing/Unit/) — xUnit v3, NSubstitute, FluentAssertions, Verify
- [Testing/Integration](Testing/Integration/) — `WebApplicationFactory`, Testcontainers
- [Testing/Testcontainers](Testing/Testcontainers/) — real dependencies in containers
- [Testing/EndToEnd](Testing/EndToEnd/) — Playwright
- [Testing/ContractTests](Testing/ContractTests/) — Pact
- [Testing/Performance](Testing/Performance/) — k6, NBomber, BenchmarkDotNet
- [Testing/ArchitectureTests](Testing/ArchitectureTests/) — NetArchTest, ArchUnitNET
- [Testing/TestDrivenDevelopment](Testing/TestDrivenDevelopment/), [Testing/BehaviorDrivenDevelopment](Testing/BehaviorDrivenDevelopment/)

### 11 — Mobile

- [Mobile/MAUI](Mobile/MAUI/) — .NET MAUI
- [Mobile/Flutter](Mobile/Flutter/), [Mobile/ReactNative](Mobile/ReactNative/)
- [Mobile/iOS](Mobile/iOS/) (Swift/SwiftUI), [Mobile/Android](Mobile/Android/) (Kotlin)

### 12 — Algorithms, Data Structures & System Design

- [AlgorithmsAndDataStructures/Algorithms](AlgorithmsAndDataStructures/Algorithms/)
- [AlgorithmsAndDataStructures/DataStructures](AlgorithmsAndDataStructures/DataStructures/)
- [AlgorithmsAndDataStructures/SystemDesign](AlgorithmsAndDataStructures/SystemDesign/)
- [LeetCode](AlgorithmsAndDataStructures/LeetCode/), [HackerRank](AlgorithmsAndDataStructures/HackerRank/), [CodeSignal](AlgorithmsAndDataStructures/CodeSignal/)

### 13 — Modernization (legacy → modern)

- [Modernization/WebFormsToBlazor](Modernization/WebFormsToBlazor/)
- [Modernization/WcfToAspNetCore](Modernization/WcfToAspNetCore/)
- [Modernization/MonolithToMicroservices](Modernization/MonolithToMicroservices/)
- [Modernization/SawToEntraExternalId](Modernization/SawToEntraExternalId/) — SAML/SAW → Entra External ID
- [Modernization/TfsToAzureGit](Modernization/TfsToAzureGit/)
- [Modernization/DotNetFrameworkToNet10](Modernization/DotNetFrameworkToNet10/)
- [Modernization/SoapToRest](Modernization/SoapToRest/)

### 14 — Tools

- [Tools/Git](Tools/Git/), [Tools/CLI](Tools/CLI/)
- [Tools/Bash](Tools/Bash/), [Tools/PowerShell](Tools/PowerShell/), [Tools/Python](Tools/Python/) (scripting)
- [Tools/PackageManagers](Tools/PackageManagers/) — NuGet, npm, pnpm, uv, pip, cargo, brew
- [Tools/Editors](Tools/Editors/) — VS 2026, VS Code, Rider
- [Tools/VsCodeExtensions](Tools/VsCodeExtensions/)

### 15 — Documentation Practices

- [Docs/ADRs](Docs/ADRs/) — Architecture Decision Records
- [Docs/C4](Docs/C4/) — C4 model diagrams
- [Docs/Diagrams](Docs/Diagrams/) — Mermaid, draw.io
- [Docs/Roadmaps](Docs/Roadmaps/) — current: [.NET 2026](Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [Docs/Templates](Docs/Templates/) — ADR, RFC, runbook, threat model
- [Docs/Runbooks](Docs/Runbooks/), [Docs/CheatSheets](Docs/CheatSheets/)

### 16 — Full-Stack & Capstone

- [FullStack/Demos](FullStack/Demos/), [FullStack/Sandbox](FullStack/Sandbox/), [FullStack/Portfolio](FullStack/Portfolio/)

---

## "To be dangerous" — top concepts per pillar

A one-screen cheatsheet. Each item links into a deeper folder when you need detail.

### .NET 10 / C# 14 (must-know)

- `record` types, `init`-only setters, primary constructors, **field-backed properties (C# 14)**
- `Span<T>` / `ReadOnlySpan<T>`, `Memory<T>`, `ArrayPool<T>`, `IBufferWriter<T>`
- `async`/`await`, `ValueTask`, `IAsyncEnumerable<T>`, **`CancellationToken` everywhere**
- DI via `IServiceCollection`, options pattern (`IOptions<T>`/`IOptionsMonitor<T>`)
- Source generators (logger, JSON, regex), trimming, **Native AOT**
- BenchmarkDotNet for perf claims; `MemoryDiagnoser` always

### ASP.NET Core 10

- Minimal APIs, route groups, typed results, OpenAPI 3.1
- Blazor **Server / WASM / Auto** render modes; QuickGrid for tables
- `IHttpClientFactory` + Polly v8; never `new HttpClient()`
- `Microsoft.Identity.Web` for Entra; Passkeys / WebAuthn for CIAM
- Output cache, response compression, rate limiting

### Data

- EF Core 10: `AsNoTracking`, query splitting, compiled queries, `ExecuteUpdate`/`ExecuteDelete`
- T-SQL fluency: query plans, indexes, columnstore, partitioning, temporal tables
- **Expand-contract** schema migrations; never combine in same release
- `HybridCache` over `IMemoryCache`/`IDistributedCache`
- Vector search via SQL Server 2025 `vector` type, pgvector, or Azure AI Search

### Architecture

- Clean / Hexagonal — depend on abstractions, not frameworks
- DDD: bounded contexts, aggregates, ubiquitous language, anti-corruption layers
- **Modular monolith first**; microservices only when evidence demands
- Outbox/Inbox for cross-service events; Saga (orchestration) for long workflows
- Strangler Fig with YARP for legacy migration

### Cloud-native

- .NET Aspire as dev-time orchestrator; container manifests as deploy-time
- Distroless / chiseled Ubuntu base images
- Bicep (Azure) or Terraform (multi-cloud); Pulumi when typed IaC wins
- Managed identity end-to-end; **no secrets in config**

### Security

- OAuth 2.0 / OIDC + PKCE; never implicit flow
- Passkeys / FIDO2 / WebAuthn as new baseline
- `IDataProtector` with key rotation; never roll your own crypto
- STRIDE per bounded context; SBOM (CycloneDX) per release
- Zero trust, least privilege, audit logging

### Observability

- OpenTelemetry over OTLP — traces, metrics, logs unified
- Serilog → OTel logs bridge; structured logs with semantic conventions
- Polly v8 pipelines: retry → circuit breaker → hedging → timeout
- Health checks (liveness vs. readiness) wired to k8s probes
- SLI / SLO / error-budget framing; runbooks as code

### Testing

- xUnit v3 + FluentAssertions + Verify + NSubstitute
- `WebApplicationFactory` + Testcontainers — real dependencies, not mocks
- Playwright for E2E; NetArchTest for fitness functions
- One assertion concept per test; AAA layout

---

## Status

This is a living reference. Domains in active build-out — see each folder's `README.md` for its own concepts + cheatsheet.

## License

[MIT](LICENSE)
