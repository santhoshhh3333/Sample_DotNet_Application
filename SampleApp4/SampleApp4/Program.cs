using Serilog;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Set up Serilog with HTTP Sink
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.OpenTelemetry(endpoint: "http://loki:3100/loki/api/v1/push",
                           protocol: OtlpProtocol.HttpProtobuf)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

//builder.Host.UseSerilog();

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OpenTelemetryWithSerilog"))
        .AddConsoleExporter();
}).AddSerilog();

// OpenTelemetry Tracing and Metrics
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("OpenTelemetryWithSerilog"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri("http://tempo:4317"); // Use OTLP exporter for Tempo
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddPrometheusExporter()); // Prometheus endpoint

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use OpenTelemetry's middleware for Prometheus scraping
app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
