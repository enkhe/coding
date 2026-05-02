// Interface Segregation Principle
// Clients should not be forced to depend on members they do not use.

namespace Architecture.SOLID.Isp;

public sealed record Product(Guid Id, string Name);

// BAD: every consumer drags read + write + bulk + audit responsibilities.
public interface IRepository_Bad<T>
{
    Task<T?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task UpdateAsync(T entity, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task BulkImportAsync(IEnumerable<T> entities, CancellationToken ct);
    Task<IReadOnlyList<AuditEntry>> AuditAsync(Guid id, CancellationToken ct);
}

public sealed record AuditEntry(DateTimeOffset At, string Action);

// GOOD: small, role-shaped interfaces. Compose where needed.
public interface IReader<T>          { Task<T?> GetAsync(Guid id, CancellationToken ct); }
public interface IWriter<T>          { Task AddAsync(T entity, CancellationToken ct); Task UpdateAsync(T entity, CancellationToken ct); }
public interface IBulkImporter<T>    { Task BulkImportAsync(IEnumerable<T> entities, CancellationToken ct); }
public interface IAuditable          { Task<IReadOnlyList<AuditEntry>> AuditAsync(Guid id, CancellationToken ct); }

public sealed class ProductLookup(IReader<Product> reader)
{
    public Task<Product?> ByIdAsync(Guid id, CancellationToken ct) => reader.GetAsync(id, ct);
}
