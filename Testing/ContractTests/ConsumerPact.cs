using System.Net.Http.Json;
using FluentAssertions;
using PactNet;
using PactNet.Matchers;
using Xunit;

namespace Testing.ContractTests;

// The consumer's expectation of the producer (orders-api).
public sealed class OrdersApiConsumerPact : IDisposable
{
    private readonly IPactBuilderV4 _pact;

    public OrdersApiConsumerPact()
    {
        var pact = Pact.V4("checkout-web", "orders-api", new PactConfig
        {
            PactDir = "../../../pacts",
            DefaultJsonSettings = new() { },
        });
        _pact = pact.WithHttpInteractions();
    }

    [Fact]
    public async Task Get_Order_Returns_Expected_Shape()
    {
        var orderId = Guid.Parse("00000000-0000-0000-0000-0000000000a1");

        _pact
            .UponReceiving("a request for an existing order")
                .Given("an order with id 00000000-0000-0000-0000-0000000000a1 exists")
                .WithRequest(HttpMethod.Get, $"/orders/{orderId}")
                .WithHeader("Accept", "application/json")
            .WillRespond()
                .WithStatus(200)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new
                {
                    id = Match.Equality(orderId),
                    sku = Match.Type("SKU-1"),
                    quantity = Match.Integer(1),
                    status = Match.Regex("Pending", "Pending|Paid|Cancelled")
                });

        await _pact.VerifyAsync(async ctx =>
        {
            using var http = new HttpClient { BaseAddress = ctx.MockServerUri };
            var response = await http.GetAsync($"/orders/{orderId}");

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadFromJsonAsync<OrderDto>();
            body!.Id.Should().Be(orderId);
        });
    }

    public void Dispose() { /* PactBuilder writes the pact on test completion */ }

    private sealed record OrderDto(Guid Id, string Sku, int Quantity, string Status);
}
