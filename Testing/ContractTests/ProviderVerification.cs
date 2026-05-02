using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using PactNet.Verifier;
using Xunit;

namespace Testing.ContractTests;

// Replays the consumer's pact against the real orders-api implementation.
public sealed class OrdersApiProviderVerification : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrdersApiProviderVerification(WebApplicationFactory<Program> factory) =>
        _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Testing"));

    [Fact]
    public void Verify_Pacts_From_Broker()
    {
        var client = _factory.CreateClient();
        var providerUri = client.BaseAddress!;

        new PactVerifier("orders-api", new PactVerifierConfig())
            .WithHttpEndpoint(providerUri)
            // Consumer-side state setup - the broker pact's "Given" string maps here.
            .WithProviderStateUrl(new Uri(providerUri, "/_provider-states"))
            .WithPactBrokerSource(new Uri("https://broker.example.com"), opts =>
                opts.PublishResults(System.Environment.GetEnvironmentVariable("GIT_SHA") ?? "local",
                    new[] { "main" }))
            .Verify();
    }
}
