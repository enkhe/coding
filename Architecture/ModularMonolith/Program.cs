// Composition root — wires up all modules.
// Each module is a self-contained capability; the host just composes them.

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOrdersModule(builder.Configuration)
    .AddBillingModule(builder.Configuration)
    .AddNotificationsModule(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app
    .MapOrdersEndpoints()
    .MapBillingEndpoints()
    .MapNotificationsEndpoints();

app.Run();

// Stubs for the other modules in this example.
namespace Modules.Billing
{
    public static class BillingModule
    {
        public static IServiceCollection AddBillingModule(this IServiceCollection s, IConfiguration _) => s;
        public static IEndpointRouteBuilder MapBillingEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/billing/invoices", () => Results.Ok());
            return app;
        }
    }
}

namespace Modules.Notifications
{
    public static class NotificationsModule
    {
        public static IServiceCollection AddNotificationsModule(this IServiceCollection s, IConfiguration _) => s;
        public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/notifications", () => Results.Ok());
            return app;
        }
    }
}
