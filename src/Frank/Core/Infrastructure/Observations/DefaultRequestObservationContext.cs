using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Default request-scope observation context used when no user identity is available.
/// </summary>
public sealed class DefaultRequestObservationContext : ObservationContextBase, IRequestObservationContext
{
    public string? UserId => null;

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
