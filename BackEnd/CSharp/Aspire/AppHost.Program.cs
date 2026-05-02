// AppHost/Program.cs
// Describes a distributed app: web frontend + API + Postgres + Redis.
// Run with `dotnet run` (Docker required for Postgres/Redis containers).

var builder = DistributedApplication.CreateBuilder(args);

// Infra
var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("pg")
    .WithDataVolume("orders-pg-data")
    .WithPgAdmin();

var ordersDb = postgres.AddDatabase("orders");

// Services — `Projects.X` symbols are generated from <ProjectReference> in csproj.
var ordersApi = builder.AddProject<Projects.OrdersApi>("orders-api")
    .WithReference(cache)
    .WithReference(ordersDb);

builder.AddProject<Projects.Web>("web")
    .WithReference(ordersApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
