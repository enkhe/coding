// HTTP-triggered isolated-worker Function for .NET 10.
// Demonstrates DI, structured logging, and OpenAPI-friendly response shape.
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MyApi.Functions;

public sealed class HttpTriggerFunction(ILogger<HttpTriggerFunction> logger)
{
    [Function(nameof(GetWidget))]
    public async Task<HttpResponseData> GetWidget(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "widgets/{id:guid}")]
        HttpRequestData req,
        Guid id,
        CancellationToken ct)
    {
        logger.LogInformation("Fetching widget {WidgetId}", id);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            id,
            name = "Widget",
            createdUtc = DateTime.UtcNow
        }, ct);

        return response;
    }

    [Function(nameof(ProcessQueueMessage))]
    public Task ProcessQueueMessage(
        [ServiceBusTrigger("orders", Connection = "ServiceBus")]
        string body,
        FunctionContext ctx,
        CancellationToken ct)
    {
        logger.LogInformation("Processing message {MessageId}", ctx.BindingContext.BindingData["MessageId"]);
        // Idempotency: check a dedup store before doing side effects.
        return Task.CompletedTask;
    }
}
