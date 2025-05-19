using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;

using Library.Checkout.Consumers;

var step = Environment.GetEnvironmentVariable("PROCESS_STEP")?.ToLower();
if (string.IsNullOrEmpty(step))
{
    throw new InvalidOperationException("PROCESS_STEP not informed");
}

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
var appVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan() // Add traceId/spanId
    .Enrich.WithProperty("Environment", environment)
    .Enrich.WithProperty("AppVersion", appVersion)
    .WriteTo.Console(new JsonFormatter())
    .Filter.ByExcluding(c =>
        c.Properties.Any(p => p.Key == "RequestPath" &&
            p.Value.ToString().Contains("/metrics")))
    .CreateLogger();

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            if (ConsumerRegistry.TryGetConsumer(step, out var consumerType))
            {
                x.AddConsumer(consumerType);
            }
            else
            {
                throw new InvalidOperationException($"Invalid PROCESS_STEP: '{step}'. Valid values: {string.Join(", ", ConsumerRegistry.ValidSteps)}");
            }
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"Library.Checkout.{step}"))
                    .AddSource("MassTransit")
                    .AddMassTransitInstrumentation()
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri("http://otel-collector:4317");
                        otlp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"Library.Checkout.{step}"))
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri("http://otel-collector:4317");
                        otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    });
            });
    });

IHost host = builder.Build();

await host.RunAsync();
