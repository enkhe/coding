// Program.cs - AOT-ready Minimal API.
// Uses CreateSlimBuilder (no MVC, no controllers, no JSON reflection).
// All DTOs serialized through a JsonSerializerContext (source-gen).

using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

// Wire the source-gen JSON context first so AOT trimmer keeps it.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

var app = builder.Build();

var users = app.MapGroup("/users");
users.MapGet("/{id:int}", (int id) => new User(id, $"u{id}@example.com"));
users.MapPost("/", (User u) => Results.Created($"/users/{u.Id}", u));

app.Run();

public sealed record User(int Id, string Email);

[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(User[]))]
internal partial class AppJsonContext : JsonSerializerContext;
