/*
    Indexes.sql
    Annotated examples: clustered, nonclustered, covering, filtered, unique, columnstore.
*/

------------------------------------------------------------
-- Sample table
------------------------------------------------------------
IF OBJECT_ID('app.OrderHeader','U') IS NOT NULL DROP TABLE app.OrderHeader;
GO
CREATE TABLE app.OrderHeader
(
    OrderId        bigint        IDENTITY(1,1) NOT NULL,
    CustomerId     bigint        NOT NULL,
    Status         tinyint       NOT NULL,                 -- 1=Open, 2=Paid, 3=Shipped, 9=Cancelled
    TotalAmount    decimal(18,2) NOT NULL,
    CurrencyCode   char(3)       NOT NULL,
    PlacedUtc      datetime2(3)  NOT NULL,
    ClosedUtc      datetime2(3)  NULL,
    Notes          nvarchar(500) NULL,
    -- Clustered: narrow, increasing, unique, immutable
    CONSTRAINT PK_OrderHeader PRIMARY KEY CLUSTERED (OrderId)
);
GO

------------------------------------------------------------
-- 1. Plain nonclustered (single-column lookup)
------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_OrderHeader_CustomerId
    ON app.OrderHeader (CustomerId);
GO

------------------------------------------------------------
-- 2. Covering index for the most common query
--    SELECT OrderId, Status, TotalAmount FROM app.OrderHeader
--    WHERE  CustomerId = @c AND PlacedUtc >= @from
--    ORDER BY PlacedUtc DESC
------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_OrderHeader_Customer_Placed
    ON app.OrderHeader (CustomerId, PlacedUtc DESC)
    INCLUDE (Status, TotalAmount);
GO

------------------------------------------------------------
-- 3. Filtered index: only open orders (sparse predicate)
--    Smaller than a full nonclustered, used when the predicate matches
------------------------------------------------------------
CREATE NONCLUSTERED INDEX IX_OrderHeader_OpenOnly
    ON app.OrderHeader (CustomerId, PlacedUtc)
    INCLUDE (TotalAmount)
    WHERE Status = 1;
GO

------------------------------------------------------------
-- 4. Unique index (business key) - enforces + speeds up
------------------------------------------------------------
ALTER TABLE app.OrderHeader ADD ExternalRef varchar(40) NULL;
GO
CREATE UNIQUE NONCLUSTERED INDEX UX_OrderHeader_ExternalRef
    ON app.OrderHeader (ExternalRef)
    WHERE ExternalRef IS NOT NULL; -- partial unique
GO

------------------------------------------------------------
-- 5. Columnstore for analytics on the same table
--    Nonclustered columnstore keeps rowstore for OLTP + columnar for reporting
------------------------------------------------------------
CREATE NONCLUSTERED COLUMNSTORE INDEX NCCI_OrderHeader_Analytics
    ON app.OrderHeader (CustomerId, Status, TotalAmount, PlacedUtc, ClosedUtc);
GO

------------------------------------------------------------
-- 6. Maintenance (online rebuild, resumable)
------------------------------------------------------------
ALTER INDEX IX_OrderHeader_Customer_Placed
    ON app.OrderHeader
    REBUILD WITH (ONLINE = ON, RESUMABLE = ON, MAXDOP = 4, FILLFACTOR = 90);
GO
