/*
    IndexUsageQueries.sql
    DMV queries to find missing, unused, and duplicate indexes.
    Run on a server with representative workload uptime (>1 week).
*/

------------------------------------------------------------
-- 1. Missing index suggestions, ranked by impact
--    Impact = (avg_total_user_cost * avg_user_impact * (user_seeks + user_scans))
------------------------------------------------------------
SELECT TOP 25
    DatabaseName  = DB_NAME(mid.database_id),
    [Schema.Table] = OBJECT_SCHEMA_NAME(mid.object_id, mid.database_id) + '.' +
                     OBJECT_NAME(mid.object_id, mid.database_id),
    Impact         = migs.avg_total_user_cost * migs.avg_user_impact *
                     (migs.user_seeks + migs.user_scans),
    migs.user_seeks,
    migs.user_scans,
    EqualityCols   = mid.equality_columns,
    InequalityCols = mid.inequality_columns,
    IncludedCols   = mid.included_columns,
    SuggestedIndex =
        N'CREATE NONCLUSTERED INDEX IX_'
        + REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns,'') + '_' + ISNULL(mid.inequality_columns,''),', ','_'),'[',''),']','')
        + N' ON ' + mid.statement
        + N' (' + ISNULL(mid.equality_columns,'')
        + CASE WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN N',' ELSE N'' END
        + ISNULL(mid.inequality_columns,'') + N')'
        + CASE WHEN mid.included_columns IS NOT NULL THEN N' INCLUDE (' + mid.included_columns + N')' ELSE N'' END
FROM   sys.dm_db_missing_index_details mid
JOIN   sys.dm_db_missing_index_groups   mig  ON mid.index_handle = mig.index_handle
JOIN   sys.dm_db_missing_index_group_stats migs ON mig.index_group_handle = migs.group_handle
WHERE  mid.database_id = DB_ID()
ORDER  BY Impact DESC;

------------------------------------------------------------
-- 2. Unused / write-only indexes (lots of writes, no reads)
--    Reset on SQL service restart - interpret accordingly
------------------------------------------------------------
SELECT
    [Schema.Table] = OBJECT_SCHEMA_NAME(i.object_id) + '.' + OBJECT_NAME(i.object_id),
    IndexName      = i.name,
    i.type_desc,
    Reads          = ISNULL(s.user_seeks,0) + ISNULL(s.user_scans,0) + ISNULL(s.user_lookups,0),
    Writes         = ISNULL(s.user_updates,0)
FROM   sys.indexes i
LEFT   JOIN sys.dm_db_index_usage_stats s
       ON  s.object_id = i.object_id
       AND s.index_id  = i.index_id
       AND s.database_id = DB_ID()
WHERE  i.is_primary_key = 0
  AND  i.is_unique_constraint = 0
  AND  i.type_desc IN ('NONCLUSTERED','CLUSTERED COLUMNSTORE','NONCLUSTERED COLUMNSTORE')
  AND  OBJECTPROPERTY(i.object_id,'IsUserTable') = 1
  AND  ISNULL(s.user_updates,0) > 0
  AND  ISNULL(s.user_seeks,0) + ISNULL(s.user_scans,0) + ISNULL(s.user_lookups,0) = 0
ORDER  BY Writes DESC;

------------------------------------------------------------
-- 3. Duplicate / overlapping indexes (same key, narrower include)
------------------------------------------------------------
WITH IndexCols AS (
    SELECT
        i.object_id, i.index_id, i.name AS index_name,
        STRING_AGG(CAST(ic.column_id AS varchar(10)), ',')
            WITHIN GROUP (ORDER BY ic.key_ordinal) AS key_cols
    FROM sys.indexes i
    JOIN sys.index_columns ic
        ON ic.object_id = i.object_id AND ic.index_id = i.index_id
    WHERE ic.is_included_column = 0
      AND i.type_desc = 'NONCLUSTERED'
    GROUP BY i.object_id, i.index_id, i.name
)
SELECT
    [Schema.Table] = OBJECT_SCHEMA_NAME(a.object_id) + '.' + OBJECT_NAME(a.object_id),
    a.index_name AS index_a, b.index_name AS index_b, a.key_cols
FROM   IndexCols a
JOIN   IndexCols b
       ON  a.object_id = b.object_id
       AND a.key_cols  = b.key_cols
       AND a.index_id  < b.index_id;

------------------------------------------------------------
-- 4. Fragmentation (target hot indexes)
------------------------------------------------------------
SELECT
    [Schema.Table] = OBJECT_SCHEMA_NAME(ips.object_id) + '.' + OBJECT_NAME(ips.object_id),
    IndexName      = i.name,
    ips.avg_fragmentation_in_percent,
    ips.page_count,
    Recommendation = CASE
        WHEN ips.avg_fragmentation_in_percent < 5  THEN 'leave'
        WHEN ips.avg_fragmentation_in_percent < 30 THEN 'REORGANIZE'
        ELSE 'REBUILD'
    END
FROM   sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
JOIN   sys.indexes i ON i.object_id = ips.object_id AND i.index_id = ips.index_id
WHERE  ips.page_count > 1000
ORDER  BY ips.avg_fragmentation_in_percent DESC;
