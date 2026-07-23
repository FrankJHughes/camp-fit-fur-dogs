using Frank.Core.Application.Abstractions.Clock;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Observations;

public sealed class SystemObservationContext : ObservationContextBase
{
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
