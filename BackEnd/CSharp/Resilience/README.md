# Resilience (C#)

> Polly v8 — language-level resilience patterns. See also [`Observability/Resilience`](../../../Observability/Resilience/) for ops view.

## Core Concepts

- **Pipeline order matters.** Outermost executes first: `Retry → CircuitBreaker → Timeout → ExecuteAsync`.
- **Strategy options** are records — set once, share via `ResiliencePipelineRegistry` if reused.
- **`CancellationToken`** flows naturally; do not swallow it in your strategy callbacks.
- **`ResiliencePipeline<TResult>`** is the typed variant — predicates can inspect the result.

## "To Be Dangerous" Cheatsheet

| Strategy | Builder method |
|---|---|
| Retry | `.AddRetry(new RetryStrategyOptions { ... })` |
| Circuit breaker | `.AddCircuitBreaker(new CircuitBreakerStrategyOptions { ... })` |
| Timeout | `.AddTimeout(TimeSpan.FromSeconds(2))` |
| Hedging | `.AddHedging(new HedgingStrategyOptions { ... })` |
| Rate limiter | `.AddRateLimiter(new SlidingWindowRateLimiter(...))` |
| Fallback | `.AddFallback(new FallbackStrategyOptions { ... })` |

## Quick Reference

```csharp
var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
    .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
    {
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>()
            .HandleResult(r => (int)r.StatusCode is 408 or 429 or >= 500),
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
    })
    .AddTimeout(TimeSpan.FromSeconds(2))
    .Build();
```

## Common Pitfalls

- Retrying non-idempotent ops without an idempotency key.
- Retry inside retry (your code + HttpClientFactory + downstream gateway). N×N×N attempts.
- Circuit breaker thresholds tuned only for dev traffic.
- Rate limiter inside per-instance pipelines for a multi-instance service — limits per pod, not per fleet.

## Examples in this folder

- [`Pipelines.cs`](Pipelines.cs) — typed and untyped pipelines
- [`Registry.cs`](Registry.cs) — share by name via `ResiliencePipelineRegistry`

## See also

- [../../../Observability/Resilience](../../../Observability/Resilience/) — ops view + standard handler
