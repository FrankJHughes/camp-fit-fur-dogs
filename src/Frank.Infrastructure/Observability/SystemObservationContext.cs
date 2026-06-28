namespace Frank.Infrastructure.Observations;

using Microsoft.Extensions.Hosting;

/// <summary>
/// Observability context for system-scope operations (startup, discovery,
/// background jobs, domain pipelines, context builders, etc.).
/// </summary>
public sealed class SystemObservationContext : ObservationContextBase
{
    /// <summary>
    /// Canonical constructor.
    /// </summary>
    public SystemObservationContext(
        string correlationId,
        string channel,
        string agent,
        string environmentName,
        DateTimeOffset timestamp,
        IReadOnlyDictionary<string, object?> metadata)
        : base(correlationId, channel, agent, environmentName, timestamp, metadata)
    {
    }

    /// <summary>
    /// Convenience constructor using IHostEnvironment. Timestamp defaults to UtcNow.
    /// </summary>
    public SystemObservationContext(
        string correlationId,
        string channel,
        string agent,
        IHostEnvironment environment,
        IReadOnlyDictionary<string, object?> metadata)
        : base(
            correlationId,
            channel,
            agent,
            environment.EnvironmentName,
            DateTimeOffset.UtcNow,
            metadata)
    {
    }

    /// <summary>
    /// Factory helper for typical system-scope usage.
    /// </summary>
    public static SystemObservationContext Create(
        string channel,
        string agent,
        IHostEnvironment environment,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        return new SystemObservationContext(
            correlationId: Guid.NewGuid().ToString("N"),
            channel: channel,
            agent: agent,
            environmentName: environment.EnvironmentName,
            timestamp: DateTimeOffset.UtcNow,
            metadata: metadata ?? new Dictionary<string, object?>());
    }
}
