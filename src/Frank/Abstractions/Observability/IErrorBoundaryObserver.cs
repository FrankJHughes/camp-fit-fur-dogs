// src/Frank/Abstractions/Observability/IErrorBoundaryObserver.cs
namespace Frank.Abstractions.Observations;

/// <summary>
/// Observes unhandled exceptions at error boundaries.
/// </summary>
public interface IErrorBoundaryObserver
{
    /// <summary>
    /// Called when an unhandled exception occurs within an error boundary.
    /// </summary>
    void OnError(Exception exception, IRequestObservationContext context);
}
