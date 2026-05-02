# ASP.NET Core 10

> The .NET HTTP framework. Hosts Minimal APIs, MVC controllers, gRPC, Blazor, and SignalR.

## Core Concepts

- **`WebApplication`** — host + DI + middleware pipeline
- **DI** — `IServiceCollection`, lifetimes (Singleton / Scoped / Transient)
- **Options pattern** — `IOptions<T>` / `IOptionsMonitor<T>` / `IOptionsSnapshot<T>`
- **Configuration** — chained sources (json, env, KV, CLI); strongly typed via Options
- **Middleware** — pipeline order matters; order in `UseX()` calls
- **`IHttpClientFactory`** — never `new HttpClient()`; named/typed clients
- **OpenAPI 3.1** — `AddOpenApi()` is built-in (no Swashbuckle needed)
- **Output cache** — `AddOutputCache()` + `[OutputCache]`
- **Rate limiting** — `AddRateLimiter()` + per-policy

## "To Be Dangerous" Cheatsheet

| Need | API |
|---|---|
| Minimal host | `WebApplication.CreateBuilder(args)` then `.Build().Run()` |
| Add OpenAPI | `services.AddOpenApi()` + `app.MapOpenApi()` |
| Typed config | `services.Configure<MyOptions>(cfg.GetSection("My"))` |
| Typed HttpClient | `services.AddHttpClient<IClient, Client>()` |
| Auth | `services.AddAuthentication("Bearer").AddJwtBearer()` |
| Authz | `services.AddAuthorization(o => o.AddPolicy(...))` |
| Cache | `services.AddOutputCache()` + `app.UseOutputCache()` |
| Rate limit | `services.AddRateLimiter(o => ...)` |
| Health | `services.AddHealthChecks()` + `app.MapHealthChecks(...)` |
| OTel | `services.AddOpenTelemetry()...` |

## Quick Reference

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDb>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Db")));
builder.Services.AddHttpClient<IPaymentsClient, PaymentsClient>().AddStandardResilienceHandler();
builder.Services.Configure<OrderOptions>(builder.Configuration.GetSection("Orders"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapHealthChecks("/health/live");

app.MapGet("/", () => "OK");
app.Run();

public sealed class OrderOptions { public int MaxItems { get; set; } = 100; }
```

## Common Pitfalls

- Middleware order — `UseAuthentication` BEFORE `UseAuthorization` BEFORE endpoints
- Singleton consuming Scoped — runtime explosion. Use scope factory.
- `new HttpClient()` everywhere — DNS exhaustion / sockets. Use `IHttpClientFactory`.
- Sync I/O in async handlers — defeats the purpose
- Missing `await` — async void handlers + lost exceptions

## Examples in this folder

- `Program.cs` — full skeleton (cross-link to other CSharp folders)

## See also

- [../MinimalApi](../MinimalApi/) · [../WebApi](../WebApi/) · [../Resilience](../Resilience/) · [../../../Observability](../../../Observability/)
