# Entity Framework Core 10

> The default .NET ORM. Use it well or it'll bite (N+1, change tracking surprises).

## Core Concepts

- **`DbContext`** — unit of work + identity map; scoped lifetime
- **`DbContextPool`** — reuse instances on hot paths (`AddDbContextPool`)
- **Change tracking** — by default; turn off for read-only with `AsNoTracking()`
- **Query splitting** — `AsSplitQuery()` to avoid cartesian explosion
- **Compiled queries** — `EF.CompileAsyncQuery` for true hot paths
- **Migrations** — `dotnet ef migrations add` / `database update`; never edit applied migrations

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Add | `services.AddDbContext<AppDb>(o => o.UseSqlServer(conn))` |
| Pool | `services.AddDbContextPool<AppDb>(...)` |
| No tracking | `.AsNoTracking()` for queries you won't update |
| Eager load | `.Include(x => x.Items).ThenInclude(...)` |
| Split query | `.AsSplitQuery()` to avoid join explosion |
| Bulk update | `.ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false))` |
| Bulk delete | `.ExecuteDeleteAsync()` |
| Raw SQL | `.FromSqlInterpolated($"...")` (typed) or `.ExecuteSqlInterpolated(...)` |
| Migrations | `dotnet ef migrations add Name` / `dotnet ef database update` |
| Concurrency | `[Timestamp] byte[] RowVersion { get; set; }` + `DbUpdateConcurrencyException` |
| Vector | `vector(1536)` column type with pgvector / SQL Server 2025 |

## Quick Reference

```csharp
public sealed class AppDb(DbContextOptions<AppDb> opts) : DbContext(opts)
{
    public DbSet<Order> Orders => Set<Order>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDb).Assembly);
    }
}

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("orders");
        b.HasKey(x => x.Id);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.UserId);
    }
}
```

```csharp
// Read (no tracking)
var dto = await db.Orders
    .AsNoTracking()
    .Where(o => o.UserId == userId)
    .OrderByDescending(o => o.PlacedAt)
    .Select(o => new OrderDto(o.Id, o.Amount))
    .FirstOrDefaultAsync(ct);

// Bulk update
await db.Orders.Where(o => o.IsArchived)
              .ExecuteUpdateAsync(s => s.SetProperty(o => o.IsActive, false), ct);

// Compiled query (hot path)
private static readonly Func<AppDb, Guid, Task<Order?>> _byId =
    EF.CompileAsyncQuery((AppDb db, Guid id) =>
        db.Orders.AsNoTracking().FirstOrDefault(o => o.Id == id));
```

## Common Pitfalls

- **N+1**: forgetting `Include`. Profile with `EF Core.Diagnostics.LogTo` in dev.
- Change tracking on huge result sets → memory blowup. `AsNoTracking()` by default for reads.
- Cartesian explosion on multi-`Include` → `AsSplitQuery()`.
- Editing an already-applied migration → use a new migration to fix.
- `.ToList()` mid-query → forces evaluation; lose composability.
- Long DbContext lifetime in workers → pool or new-per-message.

## Examples in this folder

- `AppDb.cs`, `Order.cs`, `OrderConfiguration.cs`, `Program.cs`

## See also

- [../Dapper](../Dapper/) — when EF gets in the way
- [../SqlServer/Migrations](../SqlServer/Migrations/) — expand-contract pattern
- [../VectorDb](../VectorDb/) — vector type
