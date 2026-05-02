# SQLite

> The most-deployed database on Earth. Single file, embedded, surprisingly capable.

## When to use

- Embedded (mobile, desktop, IoT)
- Local cache / offline-first
- Tests + integration fixtures
- Small server apps with low write concurrency

## When NOT to use

- High-write concurrency from multiple processes
- Network access (not designed as a server)

## "To Be Dangerous" Cheatsheet

| Need | Pattern |
|---|---|
| WAL mode | `PRAGMA journal_mode = WAL;` (better concurrency) |
| Tune | `PRAGMA synchronous = NORMAL; PRAGMA cache_size = -64000;` |
| Full text | `CREATE VIRTUAL TABLE docs USING fts5(content);` |
| JSON | `JSON1` extension built-in: `json_extract(data, '$.x')` |
| Foreign keys | `PRAGMA foreign_keys = ON;` (off by default!) |
| Backup | `VACUUM INTO 'backup.db';` (atomic) |
| Vacuum | `PRAGMA auto_vacuum = INCREMENTAL;` |

## EF Core 10 with SQLite

```csharp
builder.Services.AddDbContext<AppDb>(o =>
    o.UseSqlite("Data Source=app.db", b => b.MigrationsAssembly("App")));
```

## Common Pitfalls

- Default journal mode (DELETE) limits concurrency — use WAL
- Foreign keys off by default
- `text` is dynamically typed (you can put a number in) — use CHECK constraints
- One writer at a time; many readers — design accordingly

## See also

- [../EntityFramework](../EntityFramework/) · [../../BackEnd/CSharp/ConsoleApps](../../BackEnd/CSharp/ConsoleApps/)
