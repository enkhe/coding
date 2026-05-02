// Web layer: Minimal API surface. Translates HTTP <-> use cases. Composition root lives here too.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MyApp.Application.Users.CreateUser;

namespace MyApp.Web.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (
            CreateUserCommand cmd,
            CreateUserHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(cmd, ct);
            return Results.Created($"/users/{result.UserId}", result);
        });

        return app;
    }
}

// Program.cs (composition root):
// builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connStr));
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddSingleton<IClock, SystemClock>();
// builder.Services.AddScoped<CreateUserHandler>();
// app.MapUsers();
