# Stored Procedures

> Procs are still the right call when you need: server-side logic, parameterized batches, security boundary, plan reuse, or table-valued parameters.

## Core Concepts

- **Plan reuse**: parameterized procs cache one plan per parameter set. Beware *parameter sniffing* on skewed data.
- **`SET NOCOUNT ON`**: suppresses "X rows affected" messages = fewer round-trips, smaller TDS payload.
- **Error handling**: `BEGIN TRY ... END TRY BEGIN CATCH ... THROW; END CATCH` is the modern pattern. `RAISERROR` is legacy.
- **`OUTPUT` parameters**: pass scalars back to the caller; for sets use a `SELECT` (or `RETURN` for status int).
- **Table-valued parameters (TVPs)**: pass a typed result set in. The fastest way to send a list to SQL Server.
- **Security**: `EXECUTE AS OWNER` lets a proc cross schema/permission boundaries safely.
- **Recompile**: `OPTION (RECOMPILE)` per statement, or `WITH RECOMPILE` per proc, when parameter sniffing hurts.

## "To Be Dangerous" Cheatsheet

- Always start with `SET NOCOUNT ON; SET XACT_ABORT ON;`
- `THROW;` (no args) inside `CATCH` rethrows preserving the original error.
- Parameter sniffing fixes:
  - `OPTION (RECOMPILE)` - recompile each call.
  - `OPTION (OPTIMIZE FOR (@p = 'commonValue'))` - hint the typical value.
  - Local variable trick: copy `@p` to a local var to break sniffing.
- TVP recipe: `CREATE TYPE dbo.IntList AS TABLE (Id bigint NOT NULL PRIMARY KEY);` then `@ids dbo.IntList READONLY`.
- Idempotent upsert: `MERGE` is convenient but has known correctness bugs - prefer explicit `UPDATE ... WHERE` + `IF @@ROWCOUNT = 0 INSERT`.
- Return one result set per proc when possible. Multi-result-set procs complicate ORM mapping.
- Avoid scalar UDFs called from procs in loops - hidden row-by-row execution.

## Quick Reference

```sql
CREATE OR ALTER PROCEDURE app.usp_Example
    @CustomerId bigint,
    @NewTotal   decimal(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        UPDATE app.Customer SET LastSeenUtc = SYSUTCDATETIME() WHERE CustomerId = @CustomerId;

        SELECT @NewTotal = SUM(TotalAmount)
        FROM   app.OrderHeader
        WHERE  CustomerId = @CustomerId AND Status = 2;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;
        THROW;
    END CATCH;
END;
```

## Common Pitfalls

- Forgetting `SET NOCOUNT ON` - extra messages cost round-trips.
- Mixing `IF` branches that return different shapes - clients break.
- Catching errors but not rethrowing - silent failures.
- Long-running procs holding locks while calling out to linked servers.
- `MERGE` correctness issues (see Aaron Bertrand's posts) - use UPDATE+INSERT.
- Large dynamic SQL strings without `sp_executesql` parameterization.

## Examples in this folder

- [usp_GetOrders.sql](./usp_GetOrders.sql) - read proc with paging, optional filters, TVP for status filter.
- [usp_UpsertCustomer.sql](./usp_UpsertCustomer.sql) - idempotent upsert with OUTPUT and audit trail.

## See also

- [../Indexes/README.md](../Indexes/README.md)
- [../Functions/README.md](../Functions/README.md)
