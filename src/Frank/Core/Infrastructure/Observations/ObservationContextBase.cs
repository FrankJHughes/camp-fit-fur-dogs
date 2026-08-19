using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Base class for all observability contexts, including both request‑scope and
/// system‑scope contexts.
/// </summary>
public abstract class ObservationContextBase : IObservationContext
{
    private readonly Dictionary<string, object?> _metadata;

    public string CorrelationId { get; }
    public string Channel { get; }
    public string Agent { get; }
    public string Environment { get; }
    public DateTimeOffset Timestamp { get; }

    public IReadOnlyDictionary<string, object?> Metadata => _metadata;

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

        _metadata = new Dictionary<string, object?>(metadata);
    }

    /// <summary>
    /// Adds or updates a metadata entry in the observation context.
    /// </summary>
    public void AddMetadata(string key, object? value)
    {
        _metadata[key] = value;
    }
}
