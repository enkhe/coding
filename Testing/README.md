# Testing

> Pragmatic testing strategy and tooling for .NET 10 / C# 14 services in 2026.

## Core Concepts

- **Test pyramid**: many fast unit tests, fewer integration tests, very few end-to-end tests, plus targeted performance and contract tests.
- **AAA pattern**: Arrange / Act / Assert. One behavior per test, one assertion per concept.
- **F.I.R.S.T.**: Fast, Isolated, Repeatable, Self-validating, Timely.
- **Naming**: `MethodOrFeature_Scenario_ExpectedResult` or `Should_X_When_Y`.
- **Testability flows from design**: prefer pure functions, push side effects to the edges, depend on abstractions you control.
- **Real over fake**: prefer Testcontainers (real Postgres/Redis/Kafka) over EF InMemory. The provider differs from production and hides bugs.

## "To Be Dangerous" Cheatsheet

| Need                          | Tool (2026)                                 |
|-------------------------------|---------------------------------------------|
| Unit test runner              | xUnit v3                                    |
| Assertions                    | FluentAssertions                            |
| Mocks/stubs                   | NSubstitute (Moq has licensing issues)      |
| Snapshot/approval             | Verify                                      |
| ASP.NET Core integration      | `WebApplicationFactory<TProgram>`           |
| Real DBs/queues in tests      | Testcontainers for .NET                     |
| Browser E2E                   | Playwright for .NET                         |
| Micro-benchmark               | BenchmarkDotNet                             |
| Load test (in-proc)           | NBomber                                     |
| Load test (HTTP, scriptable)  | k6                                          |
| Architecture/layering rules   | NetArchTest (or ArchUnitNET)                |
| Consumer-driven contracts     | PactNet                                     |
| BDD                           | Reqnroll (SpecFlow is EOL)                  |
| Mutation testing              | Stryker.NET                                 |

## Quick Reference

- Run: `dotnet test --logger "console;verbosity=detailed"`
- Filter: `dotnet test --filter "Category=Integration"`
- Coverage: `dotnet test --collect:"XPlat Code Coverage"` then `reportgenerator`.
- Parallelize: xUnit v3 parallelizes by collection; mark heavy fixtures with `[CollectionDefinition(DisableParallelization = true)]`.

## Common Pitfalls

- Testing implementation details (private methods, mock interactions) instead of behavior.
- `DateTime.Now` / `Guid.NewGuid()` directly in code under test - inject `TimeProvider` and an ID generator.
- Sharing mutable state across tests via static fields.
- One giant `[Fact]` doing 10 things - split into theories.
- Asserting on logs or HTTP status only, ignoring response body.
- Treating EF `UseInMemoryDatabase` as a real DB.

## Examples in this folder

- [Unit](./Unit/README.md) - xUnit v3, FluentAssertions, NSubstitute, Verify
- [Integration](./Integration/README.md) - `WebApplicationFactory` + Testcontainers
- [EndToEnd](./EndToEnd/README.md) - Playwright for .NET
- [Performance](./Performance/README.md) - BenchmarkDotNet, NBomber, k6
- [ContractTests](./ContractTests/README.md) - PactNet consumer/provider
- [Testcontainers](./Testcontainers/README.md) - reusable container fixtures
- [ArchitectureTests](./ArchitectureTests/README.md) - NetArchTest layer rules
- [TestDrivenDevelopment](./TestDrivenDevelopment/README.md) - red/green/refactor
- [BehaviorDrivenDevelopment](./BehaviorDrivenDevelopment/README.md) - Reqnroll + Gherkin

## See also

- [../Observability/README.md](../Observability/README.md) - test what you can observe
- [../Architecture/README.md](../Architecture/README.md) - testable architecture
- [../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md](../Docs/Roadmaps/dotnet-2026-roadmap-senior-architect.md)
