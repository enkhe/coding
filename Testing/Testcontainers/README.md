# Testcontainers

> Real Postgres, Redis, Kafka, Azurite - on demand, in Docker - so tests run against the same engine as production.

## Core Concepts

- A **container fixture** owns the lifecycle (start once, dispose at end).
- Implement `IAsyncLifetime` (xUnit) and expose connection strings to the test.
- **Container reuse** across the whole test run cuts wall-clock time dramatically.
- One fixture per dependency type; share via `IClassFixture` or `ICollectionFixture`.
- Reset data between tests (truncate / Respawn / transaction rollback) instead of restarting containers.

## "To Be Dangerous" Cheatsheet

```csharp
public sealed class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithReuse(true)         // share across runs (Testcontainers labels container)
        .Build();

    public string ConnectionString => Container.GetConnectionString();
    public Task InitializeAsync() => Container.StartAsync();
    public Task DisposeAsync() => Container.DisposeAsync().AsTask();
}
```

| Module                                    | Use                                |
|-------------------------------------------|------------------------------------|
| `Testcontainers.PostgreSql`               | Postgres                           |
| `Testcontainers.MsSql`                    | SQL Server                         |
| `Testcontainers.Redis`                    | Redis                              |
| `Testcontainers.Kafka`                    | Apache Kafka                       |
| `Testcontainers.RabbitMq`                 | RabbitMQ                           |
| `Testcontainers.Azurite`                  | Azure Storage emulator             |
| `Testcontainers.LocalStack`               | AWS services emulator              |

## Quick Reference

- Reuse: `.WithReuse(true)` plus a deterministic label - container survives between `dotnet test` runs.
- Wait strategies: most modules ship a sensible default; override with `.WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))`.
- TLS / certs: bind volumes via `.WithBindMount(host, "/etc/...")`.
- CI: requires Docker-in-Docker or a Docker socket. GitHub Actions runners ship with Docker.

## Common Pitfalls

- One container per test - 30x slower than per-class.
- Using `localhost:5432` instead of `Container.GetConnectionString()` - port is randomized.
- Forgetting to dispose - dangling containers fill the host disk.
- Migrations not run after start - the schema is empty.
- Mixed test parallelism with shared container state - either truncate per test or disable parallelism for the collection.

## Examples in this folder

- [PostgresFixture.cs](./PostgresFixture.cs)
- [RedisFixture.cs](./RedisFixture.cs)
- [KafkaFixture.cs](./KafkaFixture.cs)

## See also

- [../Integration/README.md](../Integration/README.md)
- [../ContractTests/README.md](../ContractTests/README.md)
