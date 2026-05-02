using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Testing.Testcontainers;

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("app")
        .WithUsername("test")
        .WithPassword("test")
        .WithReuse(true)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await RunMigrationsAsync();
    }

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public async Task ResetAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "TRUNCATE TABLE orders RESTART IDENTITY CASCADE;";
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task RunMigrationsAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS orders (
                id UUID PRIMARY KEY,
                sku TEXT NOT NULL,
                quantity INT NOT NULL,
                status TEXT NOT NULL
            );
            """;
        await cmd.ExecuteNonQueryAsync();
    }
}
