using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Xunit;

namespace Testing.Integration;

// Assumes the SUT exposes a public partial class Program (default in .NET 10 minimal APIs).
public sealed class OrdersApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("orders")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = ConnectionString
            });
        });
    }
}

public sealed record CreateOrderRequest(string Sku, int Quantity);
public sealed record OrderResponse(Guid Id, string Sku, int Quantity, string Status);

public sealed class OrdersApiTests(OrdersApiFactory factory) : IClassFixture<OrdersApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostOrders_Should_Create_And_PersistOrder()
    {
        var response = await _client.PostAsJsonAsync(
            "/orders",
            new CreateOrderRequest("SKU-1", 3));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<OrderResponse>();
        created.Should().NotBeNull();
        created!.Sku.Should().Be("SKU-1");
        created.Status.Should().Be("Pending");

        var fetched = await _client.GetFromJsonAsync<OrderResponse>($"/orders/{created.Id}");
        fetched.Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task GetOrder_Should_Return404_When_NotFound()
    {
        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
