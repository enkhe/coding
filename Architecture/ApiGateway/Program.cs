// YARP gateway with auth, rate limiting, and config-driven routes.
// Package: Yarp.ReverseProxy
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.Authority = builder.Configuration["Jwt:Authority"];
        o.Audience = builder.Configuration["Jwt:Audience"];
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("default", p => p.RequireAuthenticatedUser());
    o.AddPolicy("admin", p => p.RequireRole("admin"));
});

builder.Services.AddRateLimiter(o =>
{
    o.AddFixedWindowLimiter("default", c =>
    {
        c.PermitLimit = 100;
        c.Window = TimeSpan.FromSeconds(10);
        c.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        c.QueueLimit = 0;
    });
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapReverseProxy();
app.Run();
