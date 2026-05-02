# Azure SQL Database

> Managed SQL Server (without you running boxes). Pricing models matter — pick on workload.

## Tier decisions

| Tier | When |
|---|---|
| **Serverless (vCore)** | Bursty; auto-pause; lowest cost when idle |
| **General Purpose vCore** | Steady workload; predictable cost |
| **Business Critical vCore** | Lowest latency, in-memory OLTP, AlwaysOn |
| **Hyperscale** | TBs to PBs; rapid backup/restore; read replicas |
| **Elastic pool** | Many small DBs sharing capacity |
| **Managed Instance** | Lift-and-shift workloads needing SQL Agent, CLR, cross-DB queries |

## Auth

- **Microsoft Entra-only** authentication (preferred; disable SQL auth where possible)
- Managed identity from app → DB

```csharp
// EF Core 10 with managed identity
var conn = "Server=my-srv.database.windows.net;Database=orders;Authentication=Active Directory Default;";
builder.Services.AddDbContext<AppDb>(o => o.UseSqlServer(conn));
```

## Operations cheatsheet

| Need | How |
|---|---|
| Geo-replication | Active geo-replica or Failover group |
| PITR | Auto, up to 35 days |
| Long-term retention | Backup vault policy |
| Auditing | Send to Log Analytics |
| Threat detection | Microsoft Defender for SQL |
| Private network | Private Endpoint + disable public access |

## Bicep snippet

```bicep
resource sql 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: 'sql-orders-prod'
  location: location
  properties: {
    administrators: { administratorType: 'ActiveDirectory', login: 'sql-admins' }
    publicNetworkAccess: 'Disabled'
    minimalTlsVersion: '1.2'
  }
}

resource db 'Microsoft.Sql/servers/databases@2024-05-01-preview' = {
  parent: sql
  name: 'orders'
  location: location
  sku: { name: 'GP_S_Gen5_2', tier: 'GeneralPurpose' }   // serverless 2 vCore
  properties: { autoPauseDelay: 60, minCapacity: json('0.5') }
}
```

## Common Pitfalls

- Pricing tier surprises (DTU vs vCore migrations)
- No firewall rule → can't connect; private endpoint then bastion
- Letting `tempdb` or transaction log grow unbounded under heavy workloads
- Forgetting to set `Auto-pause` on serverless dev DBs → unnecessary cost

## See also

- [../../../Database/SqlServer](../../../Database/SqlServer/) · [../../../Database/EntityFramework](../../../Database/EntityFramework/) · [../KeyVault](../KeyVault/)
