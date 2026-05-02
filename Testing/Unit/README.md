# Unit Tests

> Fast, deterministic, in-process tests of single components in isolation.

## Core Concepts

- One unit = one behavior. The "unit" is a behavior, not necessarily a single class.
- Test public API; never make internals public just to test them. Use `[InternalsVisibleTo]` if you must.
- Replace external dependencies (DB, HTTP, time, randomness) with test doubles.
- Snapshot tests (Verify) lock in complex output (emails, JSON, generated code).

## "To Be Dangerous" Cheatsheet

```csharp
// xUnit v3
[Fact]                                       // single test
public void Should_DoX_When_Y() { }

[Theory]                                     // parameterized
[InlineData(1, 2, 3)]
[InlineData(2, 3, 5)]
public void Add(int a, int b, int sum) =>
    (a + b).Should().Be(sum);

[Theory]
[ClassData(typeof(BigCases))]                // typed test data
public void Bigger(Order o, decimal expected) { }

public sealed class DbFixture : IAsyncLifetime { ... }
public sealed class MyTests(DbFixture fx) : IClassFixture<DbFixture> { } // shared per class
```

| Library          | Use                                                  |
|------------------|------------------------------------------------------|
| xUnit v3         | `[Fact]`, `[Theory]`, fixtures, parallelism          |
| FluentAssertions | `result.Should().Be(...)` readable failures          |
| NSubstitute      | `Substitute.For<IRepo>()`, `.Returns(...)`           |
| Verify           | `Verifier.Verify(obj)` snapshots `*.received.txt`    |
| AutoFixture      | object-mother / property-based seed data             |

## Quick Reference

- `should` chain: `.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Id))`
- NSubstitute: `repo.GetAsync(Arg.Any<Guid>()).Returns(order);`
- Verify: first run writes `*.received.txt`; rename to `*.verified.txt` to accept.
- `TimeProvider.System` in prod, `FakeTimeProvider` (Microsoft.Extensions.TimeProvider.Testing) in tests.

## Common Pitfalls

- Mocking what you do not own (e.g., `HttpClient`) - wrap behind your own interface.
- Asserting on mock interactions (`Received(1).Save(...)`) when a state assertion would do.
- `Moq` 4.20+ shipped SponsorLink telemetry - use NSubstitute.
- Snapshot tests committed without review - they pass forever even when wrong.
- Hidden time/random dependencies making flaky tests.

## Examples in this folder

- [OrderServiceTests.cs](./OrderServiceTests.cs) - xUnit v3 + FluentAssertions + NSubstitute
- [Verify_OrderEmail.cs](./Verify_OrderEmail.cs) - snapshot of generated email body

## See also

- [../Integration/README.md](../Integration/README.md)
- [../TestDrivenDevelopment/README.md](../TestDrivenDevelopment/README.md)
