# Integration Tests

> End-to-end tests of the composed system inside the test process - real DB, real HTTP pipeline, real DI.

## Core Concepts

- Boot the actual `Program.cs` via `WebApplicationFactory<TProgram>`; override only what you must (auth, time, external HTTP).
- Use **real** databases via Testcontainers - never EF InMemory in 2026. Different SQL dialect, different concurrency, different bugs.
- Treat each test as a black-box HTTP call: send a request, assert on response and persisted state.
- Reset DB state per test (transaction rollback or `Respawn`/truncate) instead of restarting containers.

## "To Be Dangerous" Cheatsheet

```csharp
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine").Build();

    public string ConnectionString => _pg.GetConnectionString();

    public Task InitializeAsync() => _pg.StartAsync();
    public new Task DisposeAsync() => _pg.DisposeAsync().AsTask();

    protected override void ConfigureWebHost(IWebHostBuilder b) =>
        b.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = ConnectionString
        }));
}
```

| Need                                  | How                                                    |
|---------------------------------------|--------------------------------------------------------|
| Override services                     | `builder.ConfigureTestServices(s => s.Replace(...))`   |
| Bypass auth                           | Custom `AuthenticationHandler` returning a fake user   |
| Stub outbound HTTP                    | `IHttpClientFactory` + custom `DelegatingHandler`      |
| Reset state                           | `Respawn` or `BEGIN TRANSACTION` per test              |
| Run migrations                        | `db.Database.MigrateAsync()` after container starts    |

## Quick Reference

- `factory.CreateClient()` returns an `HttpClient` rooted at the in-process server.
- `IClassFixture<ApiFactory>` reuses the container across tests in a class.
- `ICollectionFixture<ApiFactory>` reuses across multiple test classes.
- Mark heavy collections `[CollectionDefinition("integration", DisableParallelization = true)]` if shared mutable DB.

## Common Pitfalls

- Using `WebApplicationFactory.Server` for direct calls - prefer `CreateClient()` so middleware runs.
- Forgetting to `WaitUntil` containers are healthy.
- Not awaiting outbox/background workers - assertions race the side-effect.
- Hard-coded ports, leading to CI flakes - let Testcontainers pick.
- Recreating the container per test - 30x slower than per-class.

## Examples in this folder

- [OrdersApiTests.cs](./OrdersApiTests.cs) - `WebApplicationFactory` + Testcontainers Postgres

## See also

- [../Testcontainers/README.md](../Testcontainers/README.md)
- [../ContractTests/README.md](../ContractTests/README.md)
