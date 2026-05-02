// ASP.NET Core 10 - OpenID Connect wiring with cookie session + PKCE.
// dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "__Host-app";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    })
    .AddOpenIdConnect(options =>
    {
        options.Authority = builder.Configuration["Oidc:Authority"];     // e.g. https://login.microsoftonline.com/{tenant}/v2.0
        options.ClientId = builder.Configuration["Oidc:ClientId"];
        options.ClientSecret = builder.Configuration["Oidc:ClientSecret"]; // confidential client
        options.ResponseType = OpenIdConnectResponseType.Code;             // Authorization Code
        options.UsePkce = true;                                            // mandatory
        options.SaveTokens = true;                                         // persist id/access/refresh in auth props
        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;                                  // keep claim types as-issued

        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("offline_access");                               // refresh tokens

        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "roles";
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;

        // Use proper unique user id; never use email.
        options.ClaimActions.MapJsonKey("sub", "sub");

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = ctx =>
            {
                ctx.ProtocolMessage.SetParameter("prompt", "select_account");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                // hand-off to a transformer; see ClaimsTransformer.cs
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddTransient<IClaimsTransformation, AppClaimsTransformer>();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (HttpContext ctx) =>
    ctx.User.Identity?.IsAuthenticated == true
        ? Results.Ok(new { user = ctx.User.Identity!.Name })
        : Results.Challenge());

app.MapGet("/me", (HttpContext ctx) =>
        Results.Ok(ctx.User.Claims.Select(c => new { c.Type, c.Value })))
    .RequireAuthorization();

app.MapGet("/signout", () =>
    Results.SignOut(
        new AuthenticationProperties { RedirectUri = "/" },
        [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

app.Run();
