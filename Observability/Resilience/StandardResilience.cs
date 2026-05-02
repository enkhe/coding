// Polly v8 via Microsoft.Extensions.Resilience attached to a typed HttpClient.
// Package: Microsoft.Extensions.Http.Resilience
using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IPaymentsClient, PaymentsClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Payments:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30); // pipeline timeout will fire first
})
.AddStandardResilienceHandler(o =>
{
    o.Retry.MaxRetryAttempts = 3;
    o.Retry.UseJitter = true;
    o.Retry.Delay = TimeSpan.FromMilliseconds(250);
    o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;

    o.CircuitBreaker.FailureRatio = 0.5;
    o.CircuitBreaker.MinimumThroughput = 20;
    o.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
    o.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    o.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);
    o.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();
app.Run();

public interface IPaymentsClient
{
    Task<PaymentResult> ChargeAsync(Guid orderId, decimal amount, CancellationToken ct);
}

public sealed class PaymentsClient(HttpClient http) : IPaymentsClient
{
    public async Task<PaymentResult> ChargeAsync(Guid orderId, decimal amount, CancellationToken ct)
    {
        // Pass an idempotency key so retries don't double-charge.
        using var req = new HttpRequestMessage(HttpMethod.Post, "/charge")
        {
            Content = JsonContent.Create(new { orderId, amount })
        };
        req.Headers.Add("Idempotency-Key", orderId.ToString());

        using var res = await http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<PaymentResult>(ct))!;
    }
}

public sealed record PaymentResult(string TransactionId, string Status);
