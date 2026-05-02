// Share resilience pipelines by name via DI.
// Package: Microsoft.Extensions.Resilience
using Polly;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResiliencePipeline("db-retry", b => b
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 5,
        UseJitter = true,
        BackoffType = DelayBackoffType.Exponential,
    })
    .AddTimeout(TimeSpan.FromSeconds(5)));

builder.Services.AddResiliencePipeline("bus-retry", b => b
    .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 })
    .AddTimeout(TimeSpan.FromSeconds(2)));

var app = builder.Build();

app.MapGet("/orders/{id:guid}", async (
    Guid id,
    [FromServices] ResiliencePipelineProvider<string> pipelines,
    [FromServices] IOrderRepository repo,
    CancellationToken ct) =>
{
    var pipeline = pipelines.GetPipeline("db-retry");
    var order = await pipeline.ExecuteAsync(async token => await repo.GetAsync(id, token), ct);
    return order is null ? Results.NotFound() : Results.Ok(order);
});

app.Run();

public interface IOrderRepository { Task<object?> GetAsync(Guid id, CancellationToken ct); }
