// .NET 10 Blazor Server (interactive) host registration
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Components + interactive Server render mode
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Scoped DbContext (works with OwningComponentBase<TDbContext>)
builder.Services.AddDbContextFactory<WeatherDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<WeatherDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<WeatherDbContext>>().CreateDbContext());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

public partial class WeatherDbContext : DbContext { /* see WeatherList.razor */ }
public partial class App { } // root component
