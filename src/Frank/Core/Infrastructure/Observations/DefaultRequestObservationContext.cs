using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Represents the default request‑scope observation context used when no
/// authenticated user identity is available.
/// <para>
/// This context provides a correlation ID, environment metadata, timestamp,
/// and default values for channel and agent. It is typically used for
/// unauthenticated requests, background operations, or system‑initiated
/// workflows.
/// </para>
/// </summary>
public sealed class DefaultRequestObservationContext : ObservationContextBase, IRequestObservationContext
{
    /// <summary>
    /// Gets the user identifier associated with the request.
    /// <para>
    /// Because this context represents unauthenticated or system‑level
    /// operations, the value is always <c>null</c>.
    /// </para>
    /// </summary>
    public string? UserId => null;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="DefaultRequestObservationContext"/> class.
    /// <para>
    /// A new correlation ID is generated for each request, and environment
    /// metadata is populated using the provided <see cref="IHostEnvironment"/>
    /// and <see cref="IClock"/> abstractions.
    /// </para>
    /// </summary>
    /// <param name="environment">
    /// The hosting environment providing the current environment name.
    /// </param>
    /// <param name="clock">
    /// The clock abstraction used to obtain the current UTC timestamp.
    /// </param>
    public DefaultRequestObservationContext(
        IHostEnvironment environment,
        IClock clock)
        : base(
            correlationId: Guid.NewGuid().ToString("N"),
            channel: "none",
            agent: "none",
            environmentName: environment.EnvironmentName,
            timestamp: clock.UtcNow,
            metadata: new Dictionary<string, object?>())
    {
    }
}
