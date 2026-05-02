/*
    Functions.sql
    Scalar UDF, inline TVF, multi-statement TVF - and the rewrite to prefer.
*/

------------------------------------------------------------
-- 1. Scalar UDF (avoid in hot paths; SQL 2019+ may inline it)
------------------------------------------------------------
CREATE OR ALTER FUNCTION app.fn_OrderAgeDays(@PlacedUtc datetime2(3))
RETURNS int
WITH SCHEMABINDING
AS
BEGIN
    RETURN DATEDIFF(DAY, @PlacedUtc, SYSUTCDATETIME());
END;
GO

-- Check inlining
SELECT name, is_inlineable
FROM   sys.sql_modules m
JOIN   sys.objects o ON m.object_id = o.object_id
WHERE  o.name = 'fn_OrderAgeDays';

------------------------------------------------------------
-- 2. Inline TVF (PREFERRED) - acts like a parameterized view
------------------------------------------------------------
CREATE OR ALTER FUNCTION app.fn_OrdersForCustomer
(
    @CustomerId bigint,
    @SinceUtc   datetime2(3)
)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
(
    SELECT OrderId, Status, TotalAmount, PlacedUtc
    FROM   app.OrderHeader
    WHERE  CustomerId = @CustomerId
      AND  PlacedUtc >= @SinceUtc
);
GO

------------------------------------------------------------
-- 3. Multi-statement TVF (AVOID for large data)
--    Demonstrated only so the difference is clear.
------------------------------------------------------------
CREATE OR ALTER FUNCTION app.fn_OrderSummary_BAD(@CustomerId bigint)
RETURNS @t TABLE
(
    StatusCode    tinyint NOT NULL,
    OrderCount    int     NOT NULL,
    TotalAmount   decimal(18,2) NOT NULL
)
AS
BEGIN
    INSERT @t (StatusCode, OrderCount, TotalAmount)
    SELECT Status, COUNT(*), SUM(TotalAmount)
    FROM   app.OrderHeader
    WHERE  CustomerId = @CustomerId
    GROUP  BY Status;
    RETURN;
END;
GO

------------------------------------------------------------
-- 3b. Same logic as iTVF (do this instead)
------------------------------------------------------------
CREATE OR ALTER FUNCTION app.fn_OrderSummary
(
    @CustomerId bigint
)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
(
    SELECT Status      AS StatusCode,
           COUNT_BIG(*) AS OrderCount,
           SUM(TotalAmount) AS TotalAmount
    FROM   app.OrderHeader
    WHERE  CustomerId = @CustomerId
    GROUP  BY Status
);
GO

------------------------------------------------------------
-- Usage
------------------------------------------------------------
SELECT c.CustomerId, s.StatusCode, s.OrderCount, s.TotalAmount
FROM   app.Customer c
CROSS APPLY app.fn_OrderSummary(c.CustomerId) s;
