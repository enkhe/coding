# Resilience

> Polly v8 + `Microsoft.Extensions.Resilience` — pipeline-based, DI-native.

## Core Concepts

- **Strategies** compose into a **pipeline** (outer to inner): retry → circuit breaker → timeout → fallback.
- **`ResiliencePipeline`** is the runtime executor; **`ResiliencePipelineBuilder`** constructs it.
- **`Microsoft.Extensions.Resilience`** integrates with `IHttpClientFactory` so resilience travels with the typed client.
- **Hedging** — fire parallel attempts to reduce tail latency (good for read-mostly idempotent calls).
- **Rate limiter** — token bucket / fixed window in-process to protect downstreams.
- **Always idempotent** before retrying — POST without idempotency keys leaks duplicates.

## "To Be Dangerous" Cheatsheet

| Strategy | When |
|---|---|
| Retry (exp backoff + jitter) | Transient deps (HTTP 5xx, transient SQL) |
| Circuit breaker | Failing fast under sustained downstream outage |
| Timeout (per attempt) | Bound tail latency |
| Hedging | Read-mostly idempotent, latency-sensitive |
| Rate limiter | Protect downstream from spike |
| Fallback | Graceful degradation (cached / canned response) |

## Quick Reference

```csharp
builder.Services.AddHttpClient<IPaymentsClient, PaymentsClient>(c =>
    c.BaseAddress = new Uri("https://payments.internal"))
    .AddStandardResilienceHandler(o =>
    {
        o.Retry.MaxRetryAttempts = 3;
        o.Retry.Delay = TimeSpan.FromMilliseconds(250);
        o.CircuitBreaker.FailureRatio = 0.5;
        o.CircuitBreaker.MinimumThroughput = 20;
        o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);
        o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
    });
```

For non-HTTP code paths use `ResiliencePipelineBuilder` directly:

```csharp
var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 20,
        BreakDuration = TimeSpan.FromSeconds(30),
    })
    .AddTimeout(TimeSpan.FromSeconds(2))
    .Build();

await pipeline.ExecuteAsync(async ct => await SomeOperationAsync(ct), ct);
```

## Common Pitfalls

- Retrying non-idempotent operations (POST without idempotency key) — duplicates.
- Combining client-side retry with downstream retry — N×N amplification.
- Circuit breaker thresholds tuned for dev traffic — prod patterns differ.
- Timeouts longer than the upstream client's timeout — wasted retries.
- No metrics on retries / breaker state — invisible failure modes.

## Examples in this folder

- [`StandardResilience.cs`](StandardResilience.cs) — typed HTTP client with `AddStandardResilienceHandler`
- [`CustomPipeline.cs`](CustomPipeline.cs) — manual `ResiliencePipelineBuilder`

## See also

- [../../BackEnd/CSharp/Resilience](../../BackEnd/CSharp/Resilience/) — language-level patterns
- [../OpenTelemetry](../OpenTelemetry/) — Polly v8 emits OTel telemetry automatically
