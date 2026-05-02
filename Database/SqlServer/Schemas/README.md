# Schemas

> SQL Server schemas as namespace + security boundary. Use them to separate concerns (`app`, `audit`, `staging`) and grant permissions at the schema level.

## Core Concepts

- A **schema** is a container of database objects, owned by a principal (user/role).
- Default schema for new objects is `dbo` unless overridden per-user.
- Permissions can be granted at server, database, schema, or object level - schema-level is the sweet spot.
- Two-part names (`schema.table`) are required for explicit resolution; one-part names hit the user's default schema first, then `dbo`.
- Synonyms can alias `linked.db.schema.object` into a local name.

## "To Be Dangerous" Cheatsheet

- `CREATE SCHEMA app AUTHORIZATION db_owner;` - schema owned by a role, not a person, so it survives staff changes.
- Grant `SELECT` on a schema once: `GRANT SELECT ON SCHEMA::app TO ReadOnlyRole;`. New tables in the schema inherit the grant.
- Move objects between schemas with `ALTER SCHEMA target TRANSFER source.Object;`.
- Use `audit` schema with append-only triggers; deny `DELETE`/`UPDATE` to everyone except a service principal.
- `staging` schema for ETL landing zones; truncate without affecting `app` data.
- Avoid `dbo` for new code. It signals "no separation".

## Quick Reference

```sql
-- Create
CREATE SCHEMA app    AUTHORIZATION db_owner;
CREATE SCHEMA audit  AUTHORIZATION db_owner;
CREATE SCHEMA staging AUTHORIZATION db_owner;

-- Grant
GRANT SELECT, INSERT, UPDATE ON SCHEMA::app TO AppRole;
GRANT SELECT ON SCHEMA::audit TO ReportingRole;
DENY  DELETE, UPDATE ON SCHEMA::audit TO PUBLIC;

-- Discover
SELECT s.name AS schema_name, p.name AS owner
FROM sys.schemas s
JOIN sys.database_principals p ON s.principal_id = p.principal_id;
```

## Common Pitfalls

- Owning schemas with personal users - object orphans when the user is dropped. Always `AUTHORIZATION` to a role.
- Mixing `dbo` and `app` for the same domain - confusing and dangerous when granting.
- Forgetting that schema permissions are not inherited by certified DDL (e.g., `CREATE PROCEDURE` still needs `CREATE PROCEDURE` permission).

## Examples in this folder

- [Schema.sql](./Schema.sql) - multi-schema design with permissions and a sample table per schema.

## See also

- [../README.md](../README.md)
- [../Indexes/README.md](../Indexes/README.md)
