# .NET 10 / C# 14 / ASP.NET Core 10 Cheatsheet

## C# 14 features (the new stuff)

```csharp
// Field-backed properties — no manual backing field
public string Name { get => field; set => field = value?.Trim() ?? ""; }

// Extension members — instance + static, properties + methods on extension types
public static class StringExtensions
{
    extension(string s)
    {
        public bool IsHexColor => System.Text.RegularExpressions.Regex.IsMatch(s, "^#[0-9A-Fa-f]{6}$");
        public string Reverse() { var arr = s.ToCharArray(); System.Array.Reverse(arr); return new(arr); }
    }
}

// Lambda parameter modifiers — match the underlying delegate exactly
delegate bool TryParse<T>(string s, out T result);
TryParse<int> p = (string s, out int r) => int.TryParse(s, out r);

// params for collections
void Take(params ReadOnlySpan<int> nums) { /* ... */ }
Take(1, 2, 3, 4);

// Unbound nameof
string field = nameof(List<>.Count);
```

## ASP.NET Core 10 — Minimal API skeleton

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDb>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Db")));
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddOtlpExporter());

var app = builder.Build();
app.MapOpenApi();
app.UseAuthentication();
app.UseAuthorization();

var orders = app.MapGroup("/orders").RequireAuthorization();
orders.MapGet("/{id:guid}", async (Guid id, AppDb db) =>
    await db.Orders.FindAsync(id) is { } o ? Results.Ok(o) : Results.NotFound());
orders.MapPost("/", async (CreateOrder cmd, AppDb db) =>
{
    var o = new Order(Guid.NewGuid(), cmd.UserId, cmd.Amount);
    db.Orders.Add(o); await db.SaveChangesAsync();
    return Results.Created($"/orders/{o.Id}", o);
});

app.Run();

public sealed record CreateOrder(Guid UserId, decimal Amount);
```

## EF Core 10 quick wins

```csharp
// Bulk update without loading
await db.Orders.Where(o => o.IsArchived)
              .ExecuteUpdateAsync(s => s.SetProperty(o => o.IsActive, false));

// Bulk delete
await db.Orders.Where(o => o.PlacedAt < cutoff).ExecuteDeleteAsync();

// Compiled query (hot path)
private static readonly Func<AppDb, Guid, Task<Order?>> _byId =
    EF.CompileAsyncQuery((AppDb db, Guid id) => db.Orders.AsNoTracking().FirstOrDefault(o => o.Id == id));
```

## Polly v8 (resilience)

```csharp
builder.Services.AddHttpClient<IPaymentsClient, PaymentsClient>()
    .AddStandardResilienceHandler();
```

## OpenTelemetry essentials

```csharp
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("orders-api"))
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter());

builder.Logging.AddOpenTelemetry(l => l.AddOtlpExporter());
```

## .NET 10 release facts (2026)

- LTS, supported through November 10, 2028
- C# 14, F# 10
- .NET 8 and .NET 9 reach EoS November 10, 2026
- Native AOT improvements; Blazor Auto rendering matured
