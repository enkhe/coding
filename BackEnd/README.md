# BackEnd

> Server-side runtimes, web APIs, services, and console apps — cheatsheets across .NET, Python, Node, Go, Java, and Rust.

## Subdomains

- [`CSharp/`](CSharp/README.md) — .NET 10, C# 14, ASP.NET Core, Workers, Aspire, NativeAOT
- [`Python/`](Python/README.md) — FastAPI, Django, Flask
- [`Node/`](Node/README.md) — Express, Fastify, NestJS
- [`Go/`](Go/README.md) — `net/http` + `chi`, structured logging
- [`Java/`](Java/README.md) — Spring Boot 3.x, virtual threads
- [`Rust/`](Rust/README.md) — `axum` + `tokio`

## Cross-Language "To Be Dangerous" Cheatsheet

| Concern | C# / .NET 10 | Python 3.12+ | Node 22+ | Go 1.23+ | Java 21+ | Rust 2024 |
|---|---|---|---|---|---|---|
| HTTP framework | ASP.NET Core / Minimal API | FastAPI | Fastify / Express | `net/http` + `chi` | Spring Boot | `axum` |
| Async model | `Task` / `ValueTask` | `asyncio` | event loop / `async` | goroutines + channels | virtual threads | `tokio` |
| DI | `Microsoft.Extensions.DI` | FastAPI `Depends` | Nest providers / manual | constructor wiring | Spring `@Component` | constructor / trait objects |
| Validation | DataAnnotations / FluentValidation | Pydantic v2 | zod / typebox | `go-playground/validator` | Bean Validation | `validator` crate |
| Logging | `ILogger` + `Serilog` | `logging` / `structlog` | `pino` | `slog` | SLF4J + Logback | `tracing` |
| Observability | OpenTelemetry SDK | OpenTelemetry SDK | OpenTelemetry SDK | OTel Go | OTel Java agent | OTel Rust |
| Auth | JWT / OIDC / `AddAuthentication` | `OAuth2PasswordBearer` | `passport` / custom | `oauth2-proxy` / middleware | Spring Security | `axum-login` / JWT |
| Config | `IOptions<T>` + `appsettings.json` | `pydantic-settings` | `dotenv` + zod | `viper` / env | `application.yml` | `config` crate / env |
| Testing | `xUnit` + `WebApplicationFactory` | `pytest` + `httpx` | `vitest` + `supertest` | `testing` + `httptest` | JUnit 5 + MockMvc | `cargo test` + `tower::ServiceExt` |

## Universal Backend Patterns

- **REST**: nouns in URLs, verbs are HTTP methods, return `application/problem+json` on errors (RFC 7807 / 9457).
- **Idempotency**: `PUT` and `DELETE` idempotent; `POST` should accept an `Idempotency-Key` header for safe retries.
- **Async discipline**: pass a cancellation token / context all the way down; never block the request thread on I/O.
- **Structured logging**: log key-value, not strings; correlate with `trace_id` from W3C Trace Context.
- **Validation at the edge**: parse, don't validate — bind incoming JSON to a typed model that *cannot* be invalid.
- **12-factor config**: env vars override files; secrets from a vault, never in repo.
- **Health endpoints**: `/healthz` (liveness), `/readyz` (readiness), separate from business routes.
- **Graceful shutdown**: handle SIGTERM, drain in-flight requests, close DB pools.

## See also

- [.NET 2026 Roadmap](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
- [`Cloud/`](../Cloud/) for hosting / containers / orchestration
- [`Architecture/`](../Architecture/) for system-design patterns
