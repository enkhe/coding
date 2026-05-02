/*
    usp_UpsertCustomer.sql
    Idempotent upsert with OUTPUT param, TRY/CATCH, and audit row.
    Avoids MERGE due to known correctness issues (see Aaron Bertrand).
*/

CREATE OR ALTER PROCEDURE app.usp_UpsertCustomer
    @Email        nvarchar(254),
    @DisplayName  nvarchar(100),
    @CustomerId   bigint OUTPUT,
    @WasInserted  bit    OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @CustomerId  = NULL;
    SET @WasInserted = 0;

    BEGIN TRY
        BEGIN TRAN;

        -- Lookup first, then update-or-insert (avoids MERGE pitfalls)
        SELECT @CustomerId = CustomerId
        FROM   app.Customer WITH (UPDLOCK, HOLDLOCK)
        WHERE  Email = @Email;

        IF @CustomerId IS NOT NULL
        BEGIN
            UPDATE app.Customer
            SET    DisplayName = @DisplayName
            WHERE  CustomerId = @CustomerId;

            INSERT audit.CustomerHistory (CustomerId, Action, NewValues)
            VALUES (@CustomerId, 'U',
                    (SELECT @DisplayName AS DisplayName FOR JSON PATH, WITHOUT_ARRAY_WRAPPER));
        END
        ELSE
        BEGIN
            INSERT app.Customer (Email, DisplayName)
            VALUES (@Email, @DisplayName);

            SET @CustomerId  = SCOPE_IDENTITY();
            SET @WasInserted = 1;

            INSERT audit.CustomerHistory (CustomerId, Action, NewValues)
            VALUES (@CustomerId, 'I',
                    (SELECT @Email AS Email, @DisplayName AS DisplayName
                     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER));
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;
        THROW; -- preserve original error
    END CATCH;
END;
GO

------------------------------------------------------------
-- Sample call
------------------------------------------------------------
/*
DECLARE @id bigint, @ins bit;
EXEC app.usp_UpsertCustomer
        @Email       = N'a@b.com',
        @DisplayName = N'Ada Lovelace',
        @CustomerId  = @id OUTPUT,
        @WasInserted = @ins OUTPUT;
SELECT CustomerId = @id, WasInserted = @ins;
*/
