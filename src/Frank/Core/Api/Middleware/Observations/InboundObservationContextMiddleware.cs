#nullable enable
using System.Text.RegularExpressions;
using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Infrastructure.Observations;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Api.Middleware.Observations;

/// <summary>
/// Middleware that constructs and attaches an <see cref="IRequestObservationContext"/>
/// to the current HTTP request.
/// <para>
/// The observation context captures correlation identifiers, user identity,
/// environment metadata, clock information, and request details.
/// This enables consistent observability across vertical slices, logging,
/// diagnostics, and distributed tracing.
/// </para>
/// <para>
/// Correlation IDs are extracted from W3C <c>traceparent</c> headers when present,
/// falling back to <c>X-Correlation-ID</c> or the ASP.NET Core <c>TraceIdentifier</c>.
/// </para>
/// </summary>
public sealed class InboundObservationContextMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="InboundObservationContextMiddleware"/>
    /// class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public InboundObservationContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Creates an <see cref="IRequestObservationContext"/> for the incoming request,
    /// attaches it to <see cref="HttpContext.Items"/>, and propagates correlation
    /// identifiers for downstream components.
    /// <para>
    /// The context includes:
    /// <list type="bullet">
    /// <item><description>User identity (if authenticated)</description></item>
    /// <item><description>Correlation ID (extracted or generated)</description></item>
    /// <item><description>Hosting environment</description></item>
    /// <item><description>Clock abstraction for timestamping</description></item>
    /// <item><description>Request metadata (path, method)</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="env">The hosting environment.</param>
    /// <param name="correlation">The correlation context used to propagate IDs.</param>
    /// <param name="clock">The clock abstraction for timestamp generation.</param>
    public async Task InvokeAsync(
        HttpContext httpContext,
        IHostEnvironment env,
        ICorrelationContext correlation,
        IClock clock)
    {
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
            clock: clock,
            metadata: new Dictionary<string, object?>
            {
                ["path"] = httpContext.Request.Path.Value,
                ["method"] = httpContext.Request.Method
            });

        httpContext.Items[nameof(IRequestObservationContext)] = context;

        await _next(httpContext);
    }

    /// <summary>
    /// Extracts a correlation identifier from the incoming request.
    /// <para>
    /// Priority order:
    /// <list type="number">
    /// <item><description>W3C <c>traceparent</c> header</description></item>
    /// <item><description><c>X-Correlation-ID</c> header</description></item>
    /// <item><description>ASP.NET Core <c>TraceIdentifier</c></description></item>
    /// </list>
    /// </para>
    /// </summary>
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

    /// <summary>
    /// Parses a W3C <c>traceparent</c> header and extracts the trace ID component.
    /// <para>
    /// Valid trace IDs must be 32 hexadecimal characters.
    /// If the header is malformed or does not match the expected format,
    /// <c>null</c> is returned.
    /// </para>
    /// </summary>
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
