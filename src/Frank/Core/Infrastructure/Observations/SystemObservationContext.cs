using Frank.Core.Application.Abstractions.Clock;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Represents a system‑scope observation context used for background operations,
/// scheduled jobs, infrastructure workflows, and any activity not initiated by
/// an external request or authenticated user.
/// <para>
/// This context provides correlation metadata, environment information,
/// timestamps, and structured diagnostic metadata, enabling unified observability
/// across non‑request execution paths.
/// </para>
/// </summary>
public sealed class SystemObservationContext : ObservationContextBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SystemObservationContext"/>
    /// class using the provided correlation ID, channel, agent, environment,
    /// timestamp, and metadata.
    /// <para>
    /// Unlike <see cref="RequestObservationContext"/>, this context does not
    /// include user identity because system‑scope operations are not associated
    /// with authenticated users.
    /// </para>
    /// </summary>
    /// <param name="correlationId">The correlation identifier for the operation.</param>
    /// <param name="channel">The logical channel through which the operation was initiated.</param>
    /// <param name="agent">The initiating agent or system component.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <param name="clock">The clock abstraction used to obtain the current timestamp.</param>
    /// <param name="metadata">Structured metadata associated with the operation.</param>
    public SystemObservationContext(
        string correlationId,
        string channel,
        string agent,
        IHostEnvironment environment,
        IClock clock,
        IReadOnlyDictionary<string, object?> metadata)
        : base(
            correlationId,
            channel,
            agent,
            environment.EnvironmentName,
            clock.UtcNow,
            metadata)
    {
    }

    /// <summary>
    /// Creates a new <see cref="SystemObservationContext"/> instance using a
    /// freshly generated correlation ID and optional metadata.
    /// <para>
    /// This factory method is intended for background tasks, scheduled jobs,
    /// and infrastructure‑level operations that require consistent observability
    /// envelopes but do not originate from user requests.
    /// </para>
    /// </summary>
    /// <param name="channel">The logical channel through which the operation was initiated.</param>
    /// <param name="agent">The initiating agent or system component.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <param name="clock">The clock abstraction used to obtain the current timestamp.</param>
    /// <param name="metadata">Optional structured metadata associated with the operation.</param>
    /// <returns>A fully constructed <see cref="SystemObservationContext"/>.</returns>
    public static SystemObservationContext Create(
        string channel,
        string agent,
        IHostEnvironment environment,
        IClock clock,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        return new SystemObservationContext(
            correlationId: Guid.NewGuid().ToString("N"),
            channel: channel,
            agent: agent,
            environment: environment,
            clock: clock,
            metadata: metadata ?? new Dictionary<string, object?>());
    }
}
