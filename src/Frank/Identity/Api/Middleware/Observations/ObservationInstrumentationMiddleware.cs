#nullable enable
using System.Diagnostics;
using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Frank.Core.Infrastructure.Observations;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Frank.Identity.Api.Middleware.Observations;

/// <summary>
/// Middleware responsible for emitting structured observations, metrics, correlation
/// identifiers, and error telemetry for every HTTP request flowing through the
/// Identity API pipeline.
/// <para>
/// This middleware forms the foundation of subsystem observability (US‑183),
/// ensuring that all requests are consistently traced, timed, correlated, and
/// monitored.
/// It does not perform any domain logic; instead, it enriches the request with
/// contextual metadata and emits standardized observation events.
/// </para>
/// </summary>
/// <remarks>
/// This middleware aligns with the Identity subsystem’s observability guarantees:
/// <list type="bullet">
/// <item><description>Every request receives a correlation ID (incoming or generated).</description></item>
/// <item><description>Request duration is measured and emitted as a metric.</description></item>
/// <item><description>Begin, complete, and error events are emitted to the observation sink.</description></item>
/// <item><description>User identity is included when available, without breaking purity rules.</description></item>
/// <item><description>Errors are captured and forwarded to <see cref="IErrorBoundaryObserver"/>.</description></item>
/// </list>
/// </remarks>
public sealed class ObservationInstrumentationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the middleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ObservationInstrumentationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Executes the observability instrumentation for the current HTTP request.
    /// <para>
    /// The flow is:
    /// <list type="number">
    /// <item><description>Resolve the current user (if authenticated).</description></item>
    /// <item><description>Propagate or generate a correlation ID.</description></item>
    /// <item><description>Create a <see cref="RequestObservationContext"/> enriched with metadata.</description></item>
    /// <item><description>Emit a “request begin” trace event.</description></item>
    /// <item><description>Measure request duration using metrics.</description></item>
    /// <item><description>Emit “request complete” or “request error” events.</description></item>
    /// <item><description>Forward errors to <see cref="IErrorBoundaryObserver"/>.</description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="trace">The observation sink for trace events.</param>
    /// <param name="metrics">The metrics collector for counters and timers.</param>
    /// <param name="correlation">The correlation context manager.</param>
    /// <param name="errors">The error observer for structured error reporting.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <param name="clock">The system clock abstraction.</param>
    public async Task InvokeAsync(
        HttpContext httpContext,
        IObservationSink trace,
        IMetrics metrics,
        ICorrelationContext correlation,
        IErrorBoundaryObserver errors,
        IHostEnvironment environment,
        IClock clock)
    {
        var currentUser = httpContext.RequestServices.GetRequiredService<ICurrentUser>();

        // Determine correlation ID (incoming or generated)
        var incomingCorrelation =
            httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? httpContext.TraceIdentifier;

        var correlationId = correlation.Propagate(incomingCorrelation);

        // Resolve user ID if authenticated
        string? userId = null;
        try
        {
            Guid? userGuid = currentUser.Id;
            userId = userGuid.ToString();
        }
        catch
        {
            // User not authenticated — ignore
        }

        // Build observation context
        var context = new RequestObservationContext(
            userId: userId,
            correlationId: correlationId,
            channel: "http",
            agent: "pipeline",
            environment: environment,
            clock: clock,
            metadata: new Dictionary<string, object?>
            {
                ["path"] = httpContext.Request.Path.Value,
                ["method"] = httpContext.Request.Method
            });

        // Propagate correlation ID to response
        httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

        using var timer = metrics.Timer("http.request.duration", context);
        var sw = Stopwatch.StartNew();

        // Emit begin event
        trace.Emit(
            "http.request.begin",
            "http",
            "info",
            new
            {
                Path = httpContext.Request.Path.Value,
                Method = httpContext.Request.Method
            },
            context);

        try
        {
            await _next(httpContext);

            sw.Stop();
            metrics.Increment("http.request.count", 1, context);

            // Emit completion event
            trace.Emit(
                "http.request.complete",
                "http",
                "info",
                new
                {
                    Path = httpContext.Request.Path.Value,
                    Method = httpContext.Request.Method,
                    StatusCode = httpContext.Response.StatusCode,
                    ElapsedMilliseconds = sw.ElapsedMilliseconds
                },
                context);
        }
        catch (Exception ex)
        {
            sw.Stop();
            metrics.Increment("http.request.errors", 1, context);

            errors.OnError(ex, context);

            // Emit error event
            trace.Emit(
                "http.request.error",
                "http",
                "error",
                new
                {
                    Path = httpContext.Request.Path.Value,
                    Method = httpContext.Request.Method,
                    ex.Message,
                    ex.StackTrace,
                    ElapsedMilliseconds = sw.ElapsedMilliseconds
                },
                context);

            throw;
        }
    }
}
