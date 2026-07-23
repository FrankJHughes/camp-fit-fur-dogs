using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Base class for all observability contexts (request-scope and system-scope).
/// Provides the unified metadata model used by ITraceEvents.
/// </summary>
public abstract class ObservationContextBase : IObservationContext
{
    public string CorrelationId { get; }
    public string Channel { get; }
    public string Agent { get; }
    public string Environment { get; }
    public DateTimeOffset Timestamp { get; }
    public IReadOnlyDictionary<string, object?> Metadata { get; }

    protected ObservationContextBase(
        string correlationId,
        string channel,
        string agent,
        string environmentName,
        DateTimeOffset timestamp,
        IReadOnlyDictionary<string, object?> metadata)
    {
        CorrelationId = correlationId;
        Channel = channel;
        Agent = agent;
        Environment = environmentName;
        Timestamp = timestamp;
        Metadata = metadata;
    }
}
