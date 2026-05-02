# Architecture Tests

> Fitness functions in xUnit. Cross-link to [`Architecture/FitnessFunctions`](../../Architecture/FitnessFunctions/).

## Core Concepts

- **NetArchTest** — fluent C# rules over types/namespaces.
- **ArchUnitNET** — alternative with stronger language; layered/onion-friendly.
- **Run in CI** — fail the build on violation; cheaper than code review.

## Quick Reference

```csharp
[Fact]
public void Domain_should_not_reference_Infrastructure()
{
    var result = Types.InAssembly(typeof(Order).Assembly)
        .That().ResideInNamespaceStartingWith("Domain")
        .Should().NotHaveDependencyOn("Infrastructure")
        .GetResult();

    Assert.True(result.IsSuccessful,
        string.Join(", ", result.FailingTypeNames ?? []));
}
```

## Examples in this folder

- [`ArchTests.cs`](ArchTests.cs) — common rules

## See also

- [../../Architecture/FitnessFunctions](../../Architecture/FitnessFunctions/) · [../../Architecture/ModularMonolith](../../Architecture/ModularMonolith/)
