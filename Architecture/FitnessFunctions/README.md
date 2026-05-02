# Fitness Functions

> Automated checks that the architecture you decided on is the architecture you still have. Run in CI.

## Core Concepts

- **Architecture decay is invisible** without enforcement. Reviewers can't catch every violation.
- **Test-shaped** — fitness functions are unit tests over your *types*, not your *behavior*.
- **NetArchTest** — fluent C# API to assert dependency rules.
- **ArchUnitNET** — alternative with stronger language for layered/onion enforcement.
- **Always atomic** — one assertion per test, clear error message naming the violator.

## What to enforce

| Rule | Why |
|---|---|
| Domain ↛ Infrastructure | Keep the domain pure |
| Module A ↛ Module B internals | Modular monolith boundaries |
| Controllers should not call Repositories | Force flow through Application layer |
| `*Handler` types are `sealed` | Prevent accidental inheritance |
| `*Command`/`*Query` records are `public` | Stable contracts |
| No `static` mutable state outside Configuration | Avoid hidden globals |
| `public` types only in approved namespaces | Surface area discipline |

## Quick Reference (NetArchTest)

```csharp
[Fact]
public void Domain_does_not_reference_EF()
{
    var result = Types.InAssembly(typeof(Order).Assembly)
        .That().ResideInNamespaceStartingWith("Domain")
        .Should().NotHaveDependencyOnAny(
            "Microsoft.EntityFrameworkCore",
            "Dapper")
        .GetResult();

    Assert.True(result.IsSuccessful,
        "Domain leaks: " + string.Join(", ", result.FailingTypeNames ?? []));
}
```

## ArchUnitNET (alternative)

```csharp
private static readonly Architecture Architecture =
    new ArchLoader().LoadAssemblies(typeof(Order).Assembly).Build();

private static readonly IObjectProvider<IType> DomainLayer =
    Types().That().ResideInNamespace("Domain.*", true).As("Domain");

private static readonly IObjectProvider<IType> InfraLayer =
    Types().That().ResideInNamespace("Infrastructure.*", true).As("Infrastructure");

[Fact]
public void Domain_does_not_depend_on_Infrastructure() =>
    Types().That().Are(DomainLayer).Should().NotDependOnAny(InfraLayer)
        .Check(Architecture);
```

## Common Pitfalls

- Tests too broad ("must not reference *anything*") — false positives.
- Re-running architecture tests on every micro-PR slows CI. Consider a separate fast lane.
- Allowing exceptions via attributes/regex without writing the *why* — exceptions become the rule.

## Examples in this folder

- [`ArchitectureTests.cs`](ArchitectureTests.cs) — NetArchTest examples
- [`ArchUnitTests.cs`](ArchUnitTests.cs) — ArchUnitNET equivalent

## See also

- [../ModularMonolith](../ModularMonolith/) · [../../Testing/ArchitectureTests](../../Testing/ArchitectureTests/)
