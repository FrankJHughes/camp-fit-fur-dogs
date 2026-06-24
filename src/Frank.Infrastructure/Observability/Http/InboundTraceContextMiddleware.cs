using System.Text.RegularExpressions;
using Frank.Abstractions.Observability;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observability.Http;

public sealed class InboundTraceContextMiddleware
{
    private readonly RequestDelegate _next;

    public InboundTraceContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IHostEnvironment env)
    {
        // -----------------------------
        // 1. Extract correlation ID
        // -----------------------------
        var correlationId = ExtractCorrelationId(httpContext);

        // -----------------------------
        // 2. Build ObservabilityContext
        // -----------------------------
        var context = new ObservabilityContext(
            correlationId: correlationId,
            channel: "http",
            agent: "pipeline",
            environment: env,
            metadata: new Dictionary<string, object?>
            {
                ["path"] = httpContext.Request.Path.Value,
                ["method"] = httpContext.Request.Method
            });

        // Store for downstream
        httpContext.Items[nameof(IObservabilityContext)] = context;

        await _next(httpContext);
    }

    private static string ExtractCorrelationId(HttpContext http)
    {
        // 1. Prefer W3C traceparent
        if (http.Request.Headers.TryGetValue("traceparent", out var traceparent))
        {
            var traceId = ParseTraceId(traceparent!);
            if (traceId is not null)
                return traceId;
        }

        // 2. Fallback to X-Correlation-ID
        if (http.Request.Headers.TryGetValue("X-Correlation-ID", out var correlation))
            return correlation!;

        // 3. Fallback to HttpContext.TraceIdentifier
        return http.TraceIdentifier;
    }

    private static string? ParseTraceId(string traceparent)
    {
        // W3C format: version-traceid-spanid-flags
        // Example: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01

        var parts = traceparent.Split('-');
        if (parts.Length != 4)
            return null;

        var traceId = parts[1];

        // Must be 32 hex chars
        if (traceId.Length != 32)
            return null;

        // Must be valid hex
        if (!Regex.IsMatch(traceId, "^[0-9a-fA-F]{32}$"))
            return null;

        return traceId;
    }
}
