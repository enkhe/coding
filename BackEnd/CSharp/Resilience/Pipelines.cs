// Polly v8 pipelines — typed and untyped variants.
// Package: Polly.Core
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Resilience;

public static class Pipelines
{
    // Typed pipeline that inspects HttpResponseMessage to decide retries.
    public static ResiliencePipeline<HttpResponseMessage> HttpRetry { get; } =
        new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(r => (int)r.StatusCode is 408 or 429 or >= 500),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromMilliseconds(250),
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                FailureRatio = 0.5,
                MinimumThroughput = 20,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(30),
            })
            .AddTimeout(TimeSpan.FromSeconds(2))
            .Build();

    // Untyped pipeline — for non-HTTP work.
    public static ResiliencePipeline DbRetry { get; } =
        new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromMilliseconds(100),
                ShouldHandle = new PredicateBuilder()
                    .Handle<TimeoutRejectedException>()
                    // SQL Server transient errors
                    .Handle<Microsoft.Data.SqlClient.SqlException>(ex =>
                        ex.Number is 4060 or 40197 or 40501 or 40613 or 49918 or 49919 or 49920 or 11001),
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();
}
