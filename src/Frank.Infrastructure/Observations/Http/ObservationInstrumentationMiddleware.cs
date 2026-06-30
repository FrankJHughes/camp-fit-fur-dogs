#nullable enable
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Frank.Abstractions.Observations;
using Frank.Abstractions.Identity;

namespace Frank.Infrastructure.Observations.Http;

public sealed class ObservationInstrumentationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IObservationSink _trace;
    private readonly IMetrics _metrics;
    private readonly ICorrelationContext _correlation;
    private readonly IErrorBoundaryObserver _errors;
    private readonly IHostEnvironment _environment;
    private readonly ICurrentUser _currentUser;

    public ObservationInstrumentationMiddleware(
        RequestDelegate next,
        IObservationSink trace,
        IMetrics metrics,
        ICorrelationContext correlation,
        IErrorBoundaryObserver errors,
        IHostEnvironment environment,
        ICurrentUser currentUser)
    {
        _next = next;
        _trace = trace;
        _metrics = metrics;
        _correlation = correlation;
        _errors = errors;
        _environment = environment;
        _currentUser = currentUser;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var incomingCorrelation =
            httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? httpContext.TraceIdentifier;

        var correlationId = _correlation.Propagate(incomingCorrelation);

        string? userId = null;
        try
        {
            Guid? userGuid = _currentUser.Id;
            userId = userGuid.ToString();
        }
        catch
        {
            // User is not authenticated — leave userId = null
        }

        var context = new RequestObservationContext(
            userId: userId,
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
