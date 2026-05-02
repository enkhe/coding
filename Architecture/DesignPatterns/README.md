# Design Patterns

> The 23 Gang of Four patterns, condensed for senior eyes, with the 5-7 most relevant in modern .NET shown in code.

## Core Concepts

- A pattern is a **named tradeoff**, not a snippet to paste.
- In modern C#, many "GoF" patterns collapse into language features: events (Observer), `IEnumerable`/iterator (Iterator), records (Prototype), DI containers (Abstract Factory).
- Reach for a pattern when you can name the *forces* it resolves; otherwise, prefer the simpler shape.

## "To Be Dangerous" Cheatsheet

### Creational

| Pattern | Intent | When to use | When to avoid |
|---|---|---|---|
| Singleton | One instance, global access | Stateless cross-cutting service | Hides dependencies; prefer DI singleton lifetime |
| Factory Method | Defer instantiation to subclasses | Parallel class hierarchies | When a delegate `Func<T>` is enough |
| Abstract Factory | Family of related products | Theming, multi-DB drivers | Modern DI containers cover most cases |
| Builder | Stepwise build of complex objects | Many optional fields, fluent config | Records with `with` covers simple cases |
| Prototype | Clone an existing instance | Deep copies of expensive graphs | Records + `with` or `JsonSerializer.Deserialize` |

### Structural

| Pattern | Intent | When to use | When to avoid |
|---|---|---|---|
| Adapter | Bridge incompatible interfaces | Wrapping a 3rd-party SDK | When you control both sides |
| Bridge | Decouple abstraction from implementation | Crossing two axes of variation | One axis only — overkill |
| Composite | Tree of part-whole | File systems, AST, UI trees | Flat collections |
| Decorator | Add behavior without subclassing | Cross-cutting (caching, logging, retry) | When middleware/pipeline already exists |
| Facade | Simplified entry to a subsystem | Hiding a noisy library | When the subsystem is already small |
| Flyweight | Share intrinsic state | Massive numbers of similar objects | Premature; profile first |
| Proxy | Stand-in for another object | Lazy loading, remoting, access control | When DI interception covers it |

### Behavioral

| Pattern | Intent | When to use | When to avoid |
|---|---|---|---|
| Chain of Responsibility | Pass request along handlers | ASP.NET middleware, validators | When order of handlers is not meaningful |
| Command | Encapsulate a request | CQRS, undo, queueing | Trivial direct calls |
| Interpreter | Evaluate a small grammar | Rule engines, DSLs | Use a parser/expression library |
| Iterator | Sequential access | Use `IEnumerable<T>`/`yield` | Building one from scratch in C# |
| Mediator | Decouple peers via hub | Cross-feature coordination, MediatR | When two services can call directly |
| Memento | Capture/restore state | Undo, snapshots | When event sourcing fits better |
| Observer | One-to-many notifications | Use C# `event`/`IObservable<T>` | When request/response is enough |
| State | Behavior changes with state | Workflow / state machines | If `if/else` is short and stable |
| Strategy | Interchangeable algorithms | Pricing, sorting, validation | One stable algorithm |
| Template Method | Skeleton with overridable steps | Stable algorithm, varying steps | Composition usually wins |
| Visitor | Operation across a type hierarchy | AST traversal | Closed hierarchies — prefer pattern matching |
| Specification | Composable predicate | Reusable filter rules | Trivial predicates |

## Quick Reference

```csharp
// Strategy via DI keyed services (.NET 8+)
services.AddKeyedScoped<IShipping, FedExShipping>("fedex");
services.AddKeyedScoped<IShipping, UpsShipping>("ups");

// Resolved per-request:
public sealed class OrderShipper(IServiceProvider sp)
{
    public Task ShipAsync(Order o, string carrier, CancellationToken ct) =>
        sp.GetRequiredKeyedService<IShipping>(carrier).ShipAsync(o, ct);
}
```

## Common Pitfalls

- Using **Singleton** as a substitute for DI — hides dependencies, hostile to tests.
- **Decorator** chains so deep that latency becomes mysterious.
- **Visitor** for an open hierarchy — every new type forces every visitor to change.
- **Mediator** turning into a god-bus where every call hops through it.
- Reinventing **Observer** instead of using `event` / `IObservable<T>` / channels.

## Examples in this folder

- [`Strategy.cs`](Strategy.cs) — interchangeable shipping algorithms
- [`Decorator.cs`](Decorator.cs) — caching + logging on a repository
- [`Observer.cs`](Observer.cs) — C# `event` and `IObservable<T>`
- [`Adapter.cs`](Adapter.cs) — adapt a legacy SDK to a clean port
- [`Composite.cs`](Composite.cs) — file-system tree
- [`Builder.cs`](Builder.cs) — fluent builder vs records-with
- [`Specification.cs`](Specification.cs) — composable predicates with `&` / `|`

## See also

- [`../SOLID`](../SOLID) — patterns are SOLID applied
- [`../CQRS`](../CQRS) — Command + Mediator at architectural scale
- [`../EventDriven`](../EventDriven) — Observer at integration scale
