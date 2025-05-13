using System.Diagnostics;

using Serilog.Context;

namespace Library.API.Infrastructure.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

        // Adiciona ao contexto de log
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            // Adiciona ao header de resposta
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            await _next(context);
        }
    }
}