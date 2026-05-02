// Web app signing in users via Entra and calling a downstream API on their behalf.
// Packages:
//   Microsoft.Identity.Web
//   Microsoft.Identity.Web.UI
//   Microsoft.Identity.Web.DownstreamApi
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(builder.Configuration.GetSection("AzureAd:Scopes").Get<string[]>())
    .AddDownstreamApi("Orders", builder.Configuration.GetSection("DownstreamApis:Orders"))
    .AddDistributedTokenCaches();

builder.Services.AddDistributedMemoryCache(); // replace with Redis in prod
builder.Services.AddAuthorization();
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapGet("/me/orders", async (IDownstreamApi orders) =>
{
    // Acquires a token for the user and calls /orders.
    var data = await orders.CallApiForUserAsync<List<OrderDto>>("Orders", o =>
    {
        o.RelativePath = "orders";
    });
    return Results.Ok(data);
}).RequireAuthorization();

app.Run();

public sealed record OrderDto(Guid Id, decimal Amount);
