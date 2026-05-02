// Serilog → OTel logs bridge — modern .NET 10 logging baseline.
// Packages:
//   Serilog.AspNetCore
//   Serilog.Sinks.OpenTelemetry
//   Serilog.Enrichers.Environment
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.OpenTelemetry(o =>
    {
        o.Endpoint = ctx.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";
        o.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "orders-api",
            ["service.version"] = "1.0.0",
            ["deployment.environment"] = ctx.HostingEnvironment.EnvironmentName,
        };
    }));

var app = builder.Build();

app.UseSerilogRequestLogging(o =>
{
    o.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    o.GetLevel = (httpCtx, elapsed, ex) => ex != null
        ? LogEventLevel.Error
        : httpCtx.Response.StatusCode >= 500 ? LogEventLevel.Error
        : httpCtx.Response.StatusCode >= 400 ? LogEventLevel.Warning
        : LogEventLevel.Information;
});

app.MapGet("/", () => "OK");
app.Run();
