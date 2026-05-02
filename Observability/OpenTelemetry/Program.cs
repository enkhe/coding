// ASP.NET Core 10 + OpenTelemetry wiring — copy/paste reference.
// Requires:
//   OpenTelemetry.Extensions.Hosting
//   OpenTelemetry.Exporter.OpenTelemetryProtocol
//   OpenTelemetry.Instrumentation.AspNetCore
//   OpenTelemetry.Instrumentation.Http
//   OpenTelemetry.Instrumentation.Runtime

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string ServiceName = "orders-api";
const string ServiceVersion = "1.0.0";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService(ServiceName, serviceVersion: ServiceVersion)
        .AddAttributes(new KeyValuePair<string, object>[]
        {
            new("deployment.environment", builder.Environment.EnvironmentName),
            new("host.name", Environment.MachineName),
        }))
    .WithTracing(t => t
        .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1)))
        .AddAspNetCoreInstrumentation(o =>
        {
            o.RecordException = true;
            o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
        })
        .AddHttpClientInstrumentation()
        .AddSource(ServiceName)
        .AddOtlpExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter(ServiceName)
        .AddOtlpExporter());

builder.Logging.AddOpenTelemetry(l =>
{
    l.IncludeFormattedMessage = true;
    l.IncludeScopes = true;
    l.ParseStateValues = true;
    l.AddOtlpExporter();
});

var app = builder.Build();

app.MapGet("/", () => "OK");

app.Run();
