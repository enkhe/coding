# SOLID

> Five object-oriented design principles that, applied with judgment, keep code changeable.

## Core Concepts

- **S** — Single Responsibility: a class has one reason to change.
- **O** — Open/Closed: open for extension, closed for modification.
- **L** — Liskov Substitution: subtypes must honor the supertype contract.
- **I** — Interface Segregation: many small interfaces beat one fat interface.
- **D** — Dependency Inversion: depend on abstractions, not concretions.

## "To Be Dangerous" Cheatsheet

| Principle | Apply when | Avoid when |
|---|---|---|
| SRP | Class mixes parsing + persistence + notification | Splitting types so small that cohesion drops |
| OCP | New variants arrive often (payment providers, shippers) | Premature abstraction over a stable algorithm |
| LSP | You inherit/implement and override behavior | Forcing inheritance where composition fits |
| ISP | Clients use only 1-2 of 10 methods on an interface | Tiny apps where one interface suffices |
| DIP | High-level policy must not bind to low-level detail | Trivial code where the indirection has no payoff |

## Quick Reference

```csharp
// DIP: policy depends on abstraction; composition root wires the detail.
public interface IClock { DateTimeOffset UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTimeOffset UtcNow => DateTimeOffset.UtcNow; }

public sealed class TokenIssuer(IClock clock)
{
    public Token Issue() => new(expires: clock.UtcNow.AddMinutes(15));
}
```

## Common Pitfalls

- "SRP = one method per class." No — one *axis of change*.
- OCP via deep inheritance trees instead of strategies/composition.
- LSP violated by throwing `NotSupportedException` from an override.
- ISP turned into a swarm of one-method interfaces with no cohesion.
- DIP without a composition root — abstractions everywhere, wiring nowhere.

## Examples in this folder

- [`Srp.cs`](Srp.cs) — splitting a "User" god-class into focused services
- [`Ocp.cs`](Ocp.cs) — strategy-based pricing rules, no `switch` mutation
- [`Lsp.cs`](Lsp.cs) — Rectangle/Square trap, fixed via composition
- [`Isp.cs`](Isp.cs) — split a fat `IRepository<T>` into reader/writer
- [`Dip.cs`](Dip.cs) — invert dependency from logger detail to abstraction

## See also

- [`../DesignPatterns`](../DesignPatterns) — Strategy, Decorator, Adapter rely on SOLID
- [`../CleanArchitecture`](../CleanArchitecture) — DIP at architectural scale
- [`../FitnessFunctions`](../FitnessFunctions) — enforce DIP with arch tests
