/*
    Schema.sql
    Multi-schema design: app (transactional), audit (append-only), staging (ETL).
    Demonstrates CREATE SCHEMA, ownership via roles, and schema-scoped permissions.
*/

------------------------------------------------------------
-- 0. Roles (idempotent)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'AppRole'       AND type = 'R')
    CREATE ROLE AppRole       AUTHORIZATION db_owner;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'ReportingRole' AND type = 'R')
    CREATE ROLE ReportingRole AUTHORIZATION db_owner;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'EtlRole'       AND type = 'R')
    CREATE ROLE EtlRole       AUTHORIZATION db_owner;
GO

------------------------------------------------------------
-- 1. Schemas (owned by db_owner role, not a personal user)
------------------------------------------------------------
IF SCHEMA_ID('app')     IS NULL EXEC('CREATE SCHEMA app     AUTHORIZATION db_owner;');
IF SCHEMA_ID('audit')   IS NULL EXEC('CREATE SCHEMA audit   AUTHORIZATION db_owner;');
IF SCHEMA_ID('staging') IS NULL EXEC('CREATE SCHEMA staging AUTHORIZATION db_owner;');
GO

------------------------------------------------------------
-- 2. Sample tables, one per schema
------------------------------------------------------------
CREATE TABLE app.Customer
(
    CustomerId     bigint        IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customer PRIMARY KEY,
    Email          nvarchar(254) NOT NULL,
    DisplayName    nvarchar(100) NOT NULL,
    CreatedUtc     datetime2(3)  NOT NULL CONSTRAINT DF_Customer_Created DEFAULT SYSUTCDATETIME(),
    RowVersion     rowversion    NOT NULL,
    CONSTRAINT UQ_Customer_Email UNIQUE (Email)
);
GO

CREATE TABLE audit.CustomerHistory
(
    AuditId        bigint        IDENTITY(1,1) NOT NULL CONSTRAINT PK_CustomerHistory PRIMARY KEY,
    CustomerId     bigint        NOT NULL,
    Action         char(1)       NOT NULL,                 -- I=Insert, U=Update, D=Delete
    OldValues      nvarchar(max) NULL,
    NewValues      nvarchar(max) NULL,
    ChangedBy      sysname       NOT NULL CONSTRAINT DF_CustomerHistory_User DEFAULT SUSER_SNAME(),
    ChangedUtc     datetime2(3)  NOT NULL CONSTRAINT DF_CustomerHistory_When DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE staging.CustomerImport
(
    BatchId        uniqueidentifier NOT NULL,
    Email          nvarchar(254)    NULL,
    DisplayName    nvarchar(100)    NULL,
    SourceRow      nvarchar(max)    NULL,
    LoadedUtc      datetime2(3)     NOT NULL CONSTRAINT DF_CustomerImport_When DEFAULT SYSUTCDATETIME()
);
GO

------------------------------------------------------------
-- 3. Permissions (granted once at schema scope)
------------------------------------------------------------
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::app     TO AppRole;
GRANT SELECT                          ON SCHEMA::audit  TO ReportingRole;
GRANT INSERT                          ON SCHEMA::audit  TO AppRole;       -- write-only for app
DENY  DELETE, UPDATE                  ON SCHEMA::audit  TO PUBLIC;        -- audit is append-only
GRANT SELECT, INSERT, DELETE          ON SCHEMA::staging TO EtlRole;
GO

------------------------------------------------------------
-- 4. Verify
------------------------------------------------------------
SELECT s.name AS schema_name, p.name AS owner_principal
FROM   sys.schemas s
JOIN   sys.database_principals p ON s.principal_id = p.principal_id
WHERE  s.name IN ('app','audit','staging');

SELECT class_desc, permission_name, state_desc,
       grantee = USER_NAME(grantee_principal_id),
       schema_name = SCHEMA_NAME(major_id)
FROM   sys.database_permissions
WHERE  class = 3 -- schema
  AND  SCHEMA_NAME(major_id) IN ('app','audit','staging')
ORDER  BY schema_name, grantee, permission_name;
GO
