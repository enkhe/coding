# Dapper

> Micro-ORM. Use when EF Core's abstractions get in the way (hot paths, complex SQL, perf-critical reads).

## When to pick Dapper over EF

- Read-heavy hot paths (no change tracking overhead)
- Complex SQL you'd write by hand anyway
- Stored proc / raw query workflows
- Reporting queries that don't fit the entity graph

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Single result | `QuerySingleAsync<T>(sql, params)` |
| Multiple | `QueryAsync<T>(sql, params)` |
| Multi-mapping (joins) | `QueryAsync<A,B,Result>(sql, (a,b) => ..., splitOn: "Id")` |
| Multiple resultsets | `QueryMultipleAsync(sql, params)` then `ReadAsync<T>()` |
| Execute | `ExecuteAsync(sql, params)` |
| Scalar | `ExecuteScalarAsync<T>(sql)` |
| Stored proc | `commandType: CommandType.StoredProcedure` |
| Bulk insert | Loop + transaction, or `Dapper.Plus` (paid) |

## Quick Reference

```csharp
public sealed class OrderRepository(IDbConnectionFactory factory)
{
    public async Task<Order?> GetAsync(Guid id, CancellationToken ct)
    {
        await using var conn = factory.Open();
        return await conn.QuerySingleOrDefaultAsync<Order>(
            "SELECT id, user_id AS UserId, amount FROM orders WHERE id = @id",
            new { id });
    }

    public async Task<IReadOnlyList<OrderListItem>> GetForUserAsync(Guid userId, int limit, CancellationToken ct)
    {
        await using var conn = factory.Open();
        return (await conn.QueryAsync<OrderListItem>(
            """
            SELECT id, amount, placed_at AS PlacedAt
            FROM orders
            WHERE user_id = @userId
            ORDER BY placed_at DESC
            LIMIT @limit
            """,
            new { userId, limit })).AsList();
    }
}
```

## Common Pitfalls

- Concatenating user input into SQL → SQL injection. **Always parameterize.**
- Not disposing connections → pool starvation
- Forgetting `commandTimeout` for long queries
- N+1 queries because Dapper doesn't auto-load relations — write the JOIN

## See also

- [../EntityFramework](../EntityFramework/) · [../SqlServer](../SqlServer/) · [../PostgreSQL](../PostgreSQL/)
