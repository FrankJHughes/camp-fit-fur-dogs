using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Observes unhandled or boundary‑level errors and emits structured error
/// events into the unified observability sink.
/// <para>
/// This observer is typically invoked by exception boundaries, middleware,
/// or pipeline components responsible for capturing unexpected failures.
/// It transforms exceptions into structured telemetry that can be consumed
/// by logs, metrics, tracing systems, or external observability platforms.
/// </para>
/// </summary>
public sealed class ErrorBoundaryObserver : IErrorBoundaryObserver
{
    private readonly IObservationSink _sink;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorBoundaryObserver"/> class.
    /// </summary>
    /// <param name="sink">
    /// The sink responsible for emitting structured observability events.
    /// </param>
    public ErrorBoundaryObserver(IObservationSink sink)
    {
        _sink = sink;
    }

    /// <summary>
    /// Emits a structured error event into the observability sink when an
    /// exception occurs within a request boundary.
    /// <para>
    /// The emitted event includes exception metadata such as message,
    /// stack trace, source, and exception type, along with the associated
    /// request observation context.
    /// </para>
    /// </summary>
    /// <param name="exception">
    /// The exception that occurred.
    /// </param>
    /// <param name="context">
    /// The request observation context associated with the error.
    /// </param>
    public void OnError(Exception exception, IRequestObservationContext context)
    {
        // Emit a structured error event into the unified observability sink
        _sink.Emit(
            eventName: "request.error",
            category: "error",
            severity: "error",
            payload: new
            {
                exception.Message,
                exception.StackTrace,
                exception.Source,
                ExceptionType = exception.GetType().FullName
            },
            context: context
        );
    }
}
