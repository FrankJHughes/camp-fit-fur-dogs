namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Observes unhandled exceptions that occur within defined error boundaries,
/// enabling instrumentation, logging, and diagnostic workflows.
///
/// <para>
/// Error boundary observers provide a hook for capturing failures at the point
/// where they cross a boundary in the application (e.g., middleware, pipeline
/// segments, dispatch layers). Implementations may record telemetry, enrich
/// logs, emit structured events, or trigger alerting systems.
/// </para>
///
/// <para>
/// This interface does not handle or suppress exceptions; it strictly observes
/// them, ensuring that error‑handling logic remains separate from
/// observability concerns.
/// </para>
/// </summary>
public interface IErrorBoundaryObserver
{
    /// <summary>
    /// Called when an unhandled exception occurs within an error boundary.
    ///
    /// <para>
    /// Implementations may use the provided <paramref name="context"/> to
    /// access correlation identifiers, request metadata, or other observation
    /// details that help contextualize the failure. This method is invoked
    /// before the exception is rethrown or passed to the next handler.
    /// </para>
    /// </summary>
    /// <param name="exception">
    /// The unhandled exception that occurred.
    /// </param>
    /// <param name="context">
    /// The observation context associated with the request or operation in
    /// which the exception occurred.
    /// </param>
    void OnError(Exception exception, IRequestObservationContext context);
}
