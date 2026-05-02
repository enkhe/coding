# Test-Driven Development (TDD)

> Red → Green → Refactor. Tests drive the design, not just verify it.

## The 3 rules (Uncle Bob)

1. You may not write production code until you have a failing test
2. You may not write more of a test than is sufficient to fail (compile errors count)
3. You may not write more production code than is sufficient to pass the failing test

## Cycle

```
RED      — write a failing test
GREEN    — write the minimum code to pass
REFACTOR — clean up while staying green
```

## Walkthrough — String Calculator (classic kata)

```csharp
// Step 1 RED: empty string returns 0
[Fact] public void Empty_returns_zero() => Assert.Equal(0, new Calc().Add(""));

// Step 2 GREEN
public sealed class Calc { public int Add(string s) => 0; }

// Step 3 RED: single number
[Fact] public void Single_number_returns_itself() => Assert.Equal(5, new Calc().Add("5"));

// Step 4 GREEN
public int Add(string s) => string.IsNullOrEmpty(s) ? 0 : int.Parse(s);

// Step 5 RED: comma-separated
[Fact] public void Comma_separated_sums() => Assert.Equal(7, new Calc().Add("3,4"));

// Step 6 GREEN
public int Add(string s) =>
    string.IsNullOrEmpty(s) ? 0 : s.Split(',').Sum(int.Parse);

// Step 7 REFACTOR — extract Parse method, remove duplication
```

## When TDD shines

- Pure logic with clear inputs/outputs (parsers, calculators, domain rules)
- Bug fixes — write the failing test first
- Refactoring — tests are the safety net

## When TDD struggles

- UI / CSS — visual judgment doesn't fit unit tests
- Exploratory spikes — premature commitment
- Integration-heavy code — write integration tests first instead

## Common Pitfalls

- Skipping REFACTOR → spaghetti code with passing tests
- Tests that test the implementation, not the behavior → brittle on refactor
- Mocking everything → tests pass but the system fails

## See also

- [../Unit](../Unit/) · [../BehaviorDrivenDevelopment](../BehaviorDrivenDevelopment/)
