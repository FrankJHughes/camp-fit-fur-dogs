#nullable enable
using System.Text.RegularExpressions;
using Frank.Abstractions.Identity;
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

    public async Task InvokeAsync(
        HttpContext httpContext,
        IHostEnvironment env,
        ICurrentUser currentUser,
        ICorrelationContext correlation)
    {
        // -----------------------------
        // 1. Extract correlation ID
        // -----------------------------
        var incomingCorrelationId = ExtractCorrelationId(httpContext);
        var correlationId = correlation.Propagate(incomingCorrelationId);

        // -----------------------------
        // 2. Safely extract user ID
        // -----------------------------
        string? userId = null;
        try
        {
            userId = currentUser.Id.ToString();
        }
        catch
        {
            // User is not authenticated — leave userId = null
        }

        // -----------------------------
        // 3. Build RequestObservabilityContext
        // -----------------------------
        var context = new RequestObservabilityContext(
            userId: userId,
            correlationId: correlationId,
            channel: "http",
            agent: "pipeline",
            environment: env,
            metadata: new Dictionary<string, object?>
            {
                ["path"] = httpContext.Request.Path.Value,
                ["method"] = httpContext.Request.Method
            });

        // -----------------------------
        // 4. Store for downstream
        // -----------------------------
        httpContext.Items[nameof(IRequestObservabilityContext)] = context;

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
        var parts = traceparent.Split('-');
        if (parts.Length != 4)
            return null;

        var traceId = parts[1];

        if (traceId.Length != 32)
            return null;

        if (!Regex.IsMatch(traceId, "^[0-9a-fA-F]{32}$"))
            return null;

        return traceId;
    }
}
