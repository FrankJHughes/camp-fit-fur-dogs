using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Base class for all observability contexts, including both request‑scope and
/// system‑scope contexts.
/// <para>
/// This type provides the unified metadata model consumed by <see cref="ITraceEvent"/>
/// emitters and observability sinks.
/// It encapsulates correlation identifiers, environment metadata, timestamps,
/// and arbitrary key/value metadata used for structured diagnostics.
/// </para>
/// </summary>
public abstract class ObservationContextBase : IObservationContext
{
    /// <summary>
    /// Gets the correlation identifier associated with the operation.
    /// <para>
    /// Correlation IDs allow logs, traces, and metrics to be linked across
    /// distributed systems and asynchronous workflows.
    /// </para>
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Gets the logical channel through which the request or operation was
    /// initiated (e.g., <c>http</c>, <c>cli</c>, <c>system</c>).
    /// </summary>
    public string Channel { get; }

    /// <summary>
    /// Gets the agent responsible for initiating the operation, such as a
    /// client identifier, service name, or automation agent.
    /// </summary>
    public string Agent { get; }

    /// <summary>
    /// Gets the name of the hosting environment (e.g., <c>Development</c>,
    /// <c>Staging</c>, <c>Production</c>).
    /// </summary>
    public string Environment { get; }

    /// <summary>
    /// Gets the timestamp at which the observation context was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the structured metadata associated with the observation context.
    /// <para>
    /// Metadata may include request identifiers, feature flags, routing
    /// information, or slice‑specific diagnostic values.
    /// </para>
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservationContextBase"/> class.
    /// </summary>
    /// <param name="correlationId">The correlation identifier for the operation.</param>
    /// <param name="channel">The logical channel through which the operation occurred.</param>
    /// <param name="agent">The initiating agent or client identifier.</param>
    /// <param name="environmentName">The hosting environment name.</param>
    /// <param name="timestamp">The timestamp of the observation context.</param>
    /// <param name="metadata">Structured metadata associated with the context.</param>
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
