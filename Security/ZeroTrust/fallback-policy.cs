// Default-deny: every endpoint requires an authenticated user unless explicitly opted out.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["Identity:Authority"];
        o.Audience = builder.Configuration["Identity:Audience"];
    });

builder.Services.AddAuthorization(o =>
{
    // Fallback policy applies to endpoints with no explicit policy.
    o.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    o.AddPolicy("orders:admin", p => p.RequireRole("OrdersAdmin"));
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Authenticated by default
app.MapGet("/orders", () => Results.Ok());

// Admin-only
app.MapDelete("/orders/{id:guid}", (Guid id) => Results.NoContent())
   .RequireAuthorization("orders:admin");

// Explicit anonymous (e.g., health check)
app.MapGet("/health/live", () => Results.Ok()).AllowAnonymous();

app.Run();
