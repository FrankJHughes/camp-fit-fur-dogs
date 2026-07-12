#nullable enable
using System.Text.RegularExpressions;
using Frank.Abstractions.Identity;
using Frank.Abstractions.Observations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observations.Http;

public sealed class InboundObservationContextMiddleware
{
    private readonly RequestDelegate _next;

    public InboundObservationContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        IHostEnvironment env,
        ICorrelationContext correlation)
    {
        // Resolve scoped service INSIDE the request scope
        var currentUser = httpContext.RequestServices.GetRequiredService<ICurrentUser>();

        var incomingCorrelationId = ExtractCorrelationId(httpContext);
        var correlationId = correlation.Propagate(incomingCorrelationId);

        string? userId = null;
        try
        {
            userId = currentUser.Id.ToString();
        }
        catch
        {
            // User not authenticated
        }

        var context = new RequestObservationContext(
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

        httpContext.Items[nameof(IRequestObservationContext)] = context;

        await _next(httpContext);
    }

    private static string ExtractCorrelationId(HttpContext http)
    {
        if (http.Request.Headers.TryGetValue("traceparent", out var traceparent))
        {
            var traceId = ParseTraceId(traceparent!);
            if (traceId is not null)
                return traceId;
        }

        if (http.Request.Headers.TryGetValue("X-Correlation-ID", out var correlation))
            return correlation!;

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
