using Frank.Abstractions.Observability;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observability;

public sealed class ObservabilityContext : IObservabilityContext
{
    public string CorrelationId { get; }
    public string Channel { get; }
    public string Agent { get; }
    public string Environment { get; }
    public DateTimeOffset Timestamp { get; }
    public IReadOnlyDictionary<string, object?> Metadata { get; }

    // Primary constructor — explicit, canonical
    public ObservabilityContext(
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

    // Secondary convenience constructor — uses IHostEnvironment
    public ObservabilityContext(
        string correlationId,
        string channel,
        string agent,
        IHostEnvironment environment,
        IReadOnlyDictionary<string, object?> metadata)
        : this(
            correlationId,
            channel,
            agent,
            environment.EnvironmentName,
            DateTimeOffset.UtcNow,
            metadata)
    {
    }
}
