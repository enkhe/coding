using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;

namespace Testing.Testcontainers;

public sealed class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _container = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .WithReuse(true)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public ConnectionMultiplexer Multiplexer { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        Multiplexer = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await Multiplexer.DisposeAsync();
        await _container.DisposeAsync();
    }

    public Task FlushAsync()
    {
        var server = Multiplexer.GetServer(Multiplexer.GetEndPoints()[0]);
        return server.FlushAllDatabasesAsync();
    }
}
