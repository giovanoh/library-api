using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;

using Library.API.Domain.Repositories;
using Library.API.Domain.Services;
using Library.API.Infrastructure.Contexts;
using Library.API.Infrastructure.Factories;
using Library.API.Infrastructure.Repositories;
using Library.API.Infrastructure.Services;
using Library.API.Infrastructure.Middlewares;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var appVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan() // Add traceId/spanId
    .Enrich.WithProperty("Environment", environment)
    .Enrich.WithProperty("AppVersion", appVersion)
    .WriteTo.Console(new JsonFormatter())
    .Filter.ByExcluding(c =>
        c.Properties.Any(p => p.Key == "RequestPath" &&
            p.Value.ToString().Contains("/metrics") ||
            p.Value.ToString().Contains("/swagger")))
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1",
        Description = "Simple API to manage a library",
        Contact = new OpenApiContact
        {
            Name = "Giovano Hendges",
            Url = new Uri("https://github.com/giovanoh")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Library.API"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddConsoleExporter()
            .AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri("http://jaeger:4317");
                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Library.API"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter();
    });
builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(
            new LowercaseParameterTransformer()
        ));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.Create;
    });
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseInMemoryDatabase("library-api-in-memory-database"));

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton(new ActivitySource("Library.API"));

WebApplication app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseMiddleware<CorrelationIdMiddleware>();
app.MapControllers();
app.Run();

// This partial class makes Program explicitly accessible for integration tests
// with WebApplicationFactory<Program> in .NET 6+ minimal API applications
public partial class Program { }
