/*
    usp_GetOrders.sql
    Read proc: paged orders with optional filters and a TVP for status list.
*/

------------------------------------------------------------
-- Table-valued parameter type for status filter
------------------------------------------------------------
IF TYPE_ID('app.TinyIntList') IS NULL
    CREATE TYPE app.TinyIntList AS TABLE (Value tinyint NOT NULL PRIMARY KEY);
GO

------------------------------------------------------------
-- Proc
------------------------------------------------------------
CREATE OR ALTER PROCEDURE app.usp_GetOrders
    @CustomerId  bigint        = NULL,
    @FromUtc     datetime2(3)  = NULL,
    @ToUtc       datetime2(3)  = NULL,
    @Statuses    app.TinyIntList READONLY,
    @PageNumber  int           = 1,
    @PageSize    int           = 50,
    @TotalCount  int           = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @PageSize > 500 SET @PageSize = 500; -- guardrail

    DECLARE @HasStatusFilter bit = CASE WHEN EXISTS (SELECT 1 FROM @Statuses) THEN 1 ELSE 0 END;

    ;WITH Filtered AS
    (
        SELECT  o.OrderId, o.CustomerId, o.Status, o.TotalAmount,
                o.CurrencyCode, o.PlacedUtc
        FROM    app.OrderHeader o
        WHERE   (@CustomerId IS NULL OR o.CustomerId = @CustomerId)
          AND   (@FromUtc    IS NULL OR o.PlacedUtc  >= @FromUtc)
          AND   (@ToUtc      IS NULL OR o.PlacedUtc  <  @ToUtc)
          AND   (@HasStatusFilter = 0 OR o.Status IN (SELECT Value FROM @Statuses))
    )
    SELECT @TotalCount = COUNT(*) FROM Filtered;

    SELECT  o.OrderId, o.CustomerId, o.Status, o.TotalAmount,
            o.CurrencyCode, o.PlacedUtc
    FROM    app.OrderHeader o
    WHERE   (@CustomerId IS NULL OR o.CustomerId = @CustomerId)
      AND   (@FromUtc    IS NULL OR o.PlacedUtc  >= @FromUtc)
      AND   (@ToUtc      IS NULL OR o.PlacedUtc  <  @ToUtc)
      AND   (@HasStatusFilter = 0 OR o.Status IN (SELECT Value FROM @Statuses))
    ORDER BY o.PlacedUtc DESC, o.OrderId DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH  NEXT @PageSize ROWS ONLY
    OPTION (RECOMPILE); -- avoid bad sniffing on optional filter combinations
END;
GO

------------------------------------------------------------
-- Sample call
------------------------------------------------------------
/*
DECLARE @s app.TinyIntList;
INSERT  @s VALUES (1),(2);
DECLARE @t int;
EXEC app.usp_GetOrders
        @CustomerId = 42,
        @Statuses   = @s,
        @PageNumber = 1,
        @PageSize   = 50,
        @TotalCount = @t OUTPUT;
SELECT TotalCount = @t;
*/
