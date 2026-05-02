// Decorator: add behavior to a port without changing its contract or the core.
// Useful for cross-cutting concerns: caching, logging, retry, metrics.

namespace Architecture.DesignPatterns.Decorator;

public sealed record Customer(Guid Id, string Name);

public interface ICustomerRepository
{
    Task<Customer?> GetAsync(Guid id, CancellationToken ct);
}

public sealed class SqlCustomerRepository : ICustomerRepository
{
    public Task<Customer?> GetAsync(Guid id, CancellationToken ct) =>
        Task.FromResult<Customer?>(new Customer(id, "Ada"));
}

public sealed class CachingCustomerRepository(ICustomerRepository inner, IMemoryCache cache) : ICustomerRepository
{
    public async Task<Customer?> GetAsync(Guid id, CancellationToken ct)
    {
        if (cache.TryGetValue(id, out Customer? cached)) return cached;
        var fresh = await inner.GetAsync(id, ct);
        if (fresh is not null) cache.Set(id, fresh, TimeSpan.FromMinutes(5));
        return fresh;
    }
}

public sealed class LoggingCustomerRepository(ICustomerRepository inner, ILogger<LoggingCustomerRepository> log) : ICustomerRepository
{
    public async Task<Customer?> GetAsync(Guid id, CancellationToken ct)
    {
        log.LogDebug("GetCustomer {Id}", id);
        return await inner.GetAsync(id, ct);
    }
}

// Wiring (Scrutor style):
// services.AddScoped<ICustomerRepository, SqlCustomerRepository>();
// services.Decorate<ICustomerRepository, CachingCustomerRepository>();
// services.Decorate<ICustomerRepository, LoggingCustomerRepository>();

// Stand-ins so the file is self-contained:
public interface IMemoryCache
{
    bool TryGetValue<T>(object key, out T? value);
    void Set<T>(object key, T value, TimeSpan ttl);
}
public interface ILogger<T> { void LogDebug(string template, params object[] args); }
