// Manual ResiliencePipeline for non-HTTP scenarios (DB calls, queue ops, etc).
// Package: Polly.Core
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Orders.Api.Resilience;

public sealed class OutboxPublisher(IOutboxStore store, IBus bus, ILogger<OutboxPublisher> log)
{
    private readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 5,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.FromMilliseconds(200),
            ShouldHandle = new PredicateBuilder()
                .Handle<TimeoutRejectedException>()
                .Handle<HttpRequestException>(),
            OnRetry = args =>
            {
                log.LogWarning(args.Outcome.Exception,
                    "Retry {Attempt} after {Delay}ms", args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                return ValueTask.CompletedTask;
            }
        })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(10),
            MinimumThroughput = 20,
            BreakDuration = TimeSpan.FromSeconds(30),
        })
        .AddTimeout(TimeSpan.FromSeconds(5))
        .Build();

    public async Task PublishPendingAsync(CancellationToken ct)
    {
        await foreach (var msg in store.ReadPendingAsync(ct))
        {
            await _pipeline.ExecuteAsync(async token =>
            {
                await bus.PublishAsync(msg, token);
                await store.MarkPublishedAsync(msg.Id, token);
            }, ct);
        }
    }
}

public interface IOutboxStore
{
    IAsyncEnumerable<OutboxMessage> ReadPendingAsync(CancellationToken ct);
    Task MarkPublishedAsync(Guid id, CancellationToken ct);
}
public interface IBus { Task PublishAsync(OutboxMessage msg, CancellationToken ct); }
public sealed record OutboxMessage(Guid Id, string Type, string Payload);
