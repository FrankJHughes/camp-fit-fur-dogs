#nullable enable
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Frank.Abstractions.Observability;
namespace Frank.Infrastructure.Observability.Http;

public sealed class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITraceEvents _trace;
    private readonly IMetrics _metrics;
    private readonly ICorrelationContext _correlation;
    private readonly IErrorBoundaryObserver _errors;
    private readonly IHostEnvironment _environment;

    public ObservabilityMiddleware(
        RequestDelegate next,
        ITraceEvents trace,
        IMetrics metrics,
        ICorrelationContext correlation,
        IErrorBoundaryObserver errors,
        IHostEnvironment environment)
    {
        _next = next;
        _trace = trace;
        _metrics = metrics;
        _correlation = correlation;
        _errors = errors;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var incomingCorrelation =
            httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? httpContext.TraceIdentifier;

        var correlationId = _correlation.Propagate(incomingCorrelation);

        var context = new ObservabilityContext(
            correlationId: correlationId,
            channel: "http",
            agent: "pipeline",
            environment: _environment,
            metadata: new Dictionary<string, object?>
            {
                ["path"] = httpContext.Request.Path.Value,
                ["method"] = httpContext.Request.Method
            });

        httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

        using var timer = _metrics.Timer("http.request.duration", context);
        var sw = Stopwatch.StartNew();

        _trace.Emit(
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
            _metrics.Increment("http.request.count", 1, context);

            _trace.Emit(
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
            _metrics.Increment("http.request.errors", 1, context);

            _errors.OnError(ex, context);

            _trace.Emit(
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
