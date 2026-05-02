// Aspire AppHost — declarative distributed app model.
// Package: Aspire.Hosting.Azure (and per-resource packages: Aspire.Hosting.Azure.PostgreSQL, etc.)

var builder = DistributedApplication.CreateBuilder(args);

// Azure Postgres Flexible Server, with a database "orders".
var pg = builder.AddAzurePostgresFlexibleServer("orders-pg")
    .WithPasswordAuthentication()
    .AddDatabase("orders");

// Azure Cache for Redis.
var redis = builder.AddAzureRedis("orders-cache");

// Azure Service Bus with a queue.
var bus = builder.AddAzureServiceBus("orders-bus")
    .WithQueue("orders.created");

// Optional: parameterize the connection-string source for Application Insights.
var appInsights = builder.AddAzureApplicationInsights("orders-ai");

// API project — references all infra; Aspire injects connection strings as env vars.
var api = builder.AddProject<Projects.Orders_Api>("api")
    .WithReference(pg)
    .WithReference(redis)
    .WithReference(bus)
    .WithReference(appInsights);

// Worker — same DB, same bus.
builder.AddProject<Projects.Orders_Worker>("worker")
    .WithReference(pg)
    .WithReference(bus)
    .WithReference(appInsights);

// Front-end calls API.
builder.AddProject<Projects.Orders_Web>("web")
    .WithReference(api);

builder.Build().Run();
