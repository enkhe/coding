// Primary (driving) adapter: HTTP -> IUserPort. The web does not know about the repository.

using Architecture.Hexagonal.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Architecture.Hexagonal.WebAdapter;

public sealed record RegisterUserRequest(string Email, string Name);

public static class WebUserAdapter
{
    public static IEndpointRouteBuilder MapUsers(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (RegisterUserRequest req, IUserPort users, CancellationToken ct) =>
        {
            var dto = await users.RegisterAsync(req.Email, req.Name, ct);
            return Results.Created($"/users/{dto.Id}", dto);
        });

        app.MapGet("/users/{id:guid}", async (Guid id, IUserPort users, CancellationToken ct) =>
            await users.GetByIdAsync(id, ct) is { } u ? Results.Ok(u) : Results.NotFound());

        return app;
    }
}
