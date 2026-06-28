// src/Frank/Abstractions/Observability/ITraceEvents.cs
namespace Frank.Abstractions.Observations;

/// <summary>
/// Emits structured trace events for observability.
/// Implementations must be provided by the infrastructure layer.
/// </summary>
public interface IObservationSink
{
    /// <summary>
    /// Emits a structured event with the given metadata and context.
    /// </summary>
    void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservationContext context);
}
