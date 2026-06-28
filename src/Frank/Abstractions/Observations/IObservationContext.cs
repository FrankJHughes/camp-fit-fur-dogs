// src/Frank/Abstractions/Observability/IObservabilityContext.cs
namespace Frank.Abstractions.Observations;

/// <summary>
/// Represents the structured context that flows through all observable operations.
/// This context is immutable and must be provided by the infrastructure layer.
/// </summary>
public interface IObservationContext
{
    /// <summary>
    /// A unique identifier used to correlate logs, traces, and metrics across boundaries.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// The vertical slice or capability emitting the event.
    /// </summary>
    string Channel { get; }

    /// <summary>
    /// The module or subsystem within the slice.
    /// </summary>
    string Agent { get; }

    /// <summary>
    /// The environment (e.g., Development, Staging, Production).
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Timestamp of the event or operation.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Additional metadata relevant to the operation.
    /// </summary>
    IReadOnlyDictionary<string, object?> Metadata { get; }
}
