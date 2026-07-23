using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Observations;

public sealed class RequestObservationContext : ObservationContextBase, IRequestObservationContext
{
    public string? UserId { get; }

    public RequestObservationContext(
        string? userId,
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
            EnrichMetadata(userId, metadata))
    {
        UserId = userId;
    }

    public static RequestObservationContext Create(
        string? userId,
        string correlationId,
        string channel,
        string agent,
        IHostEnvironment environment,
        IClock clock,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        return new RequestObservationContext(
            userId,
            correlationId,
            channel,
            agent,
            environment,
            clock,
            metadata ?? new Dictionary<string, object?>());
    }

    private static IReadOnlyDictionary<string, object?> EnrichMetadata(
        string? userId,
        IReadOnlyDictionary<string, object?> metadata)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return metadata;

        var dict = new Dictionary<string, object?>(metadata)
        {
            ["user.id"] = userId
        };

        return dict;
    }
}
