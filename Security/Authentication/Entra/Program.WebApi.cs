// Protected Web API — validates JWT bearer tokens issued by Entra ID / External ID.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("orders:read", p => p.RequireScope("Orders.Read"));
    o.AddPolicy("orders:write", p => p.RequireScope("Orders.Write"));
    o.AddPolicy("orders:admin", p => p.RequireRole("OrdersAdmin"));
});

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();

app.MapGet("/orders", () => Results.Ok(new[] { new { id = Guid.NewGuid(), amount = 99.99m } }))
   .RequireAuthorization("orders:read");

app.MapPost("/orders", () => Results.Created())
   .RequireAuthorization("orders:write");

app.MapDelete("/orders/{id:guid}", (Guid id) => Results.NoContent())
   .RequireAuthorization("orders:admin");

app.Run();
