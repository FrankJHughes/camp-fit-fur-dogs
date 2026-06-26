using Frank.Abstractions.Observability;
using Microsoft.Extensions.Hosting;

namespace Frank.Infrastructure.Observability;

/// <summary>
/// Observability context for request-scope operations.
/// Includes authenticated user identity when available.
/// </summary>
public sealed class RequestObservabilityContext : ObservabilityContextBase, IRequestObservabilityContext
{
    public string? UserId { get; }

    /// <summary>
    /// Canonical constructor.
    /// Mirrors UserId into Metadata["user.id"] when provided.
    /// </summary>
    public RequestObservabilityContext(
        string? userId,
        string correlationId,
        string channel,
        string agent,
        string environmentName,
        DateTimeOffset timestamp,
        IReadOnlyDictionary<string, object?> metadata)
        : base(
            correlationId,
            channel,
            agent,
            environmentName,
            timestamp,
            EnrichMetadata(userId, metadata))
    {
        UserId = userId;
    }

    /// <summary>
    /// Convenience constructor using IHostEnvironment. Timestamp defaults to UtcNow.
    /// </summary>
    public RequestObservabilityContext(
        string? userId,
        string correlationId,
        string channel,
        string agent,
        IHostEnvironment environment,
        IReadOnlyDictionary<string, object?> metadata)
        : this(
            userId,
            correlationId,
            channel,
            agent,
            environment.EnvironmentName,
            DateTimeOffset.UtcNow,
            metadata)
    {
    }

    /// <summary>
    /// Factory helper for typical request-scope usage.
    /// </summary>
    public static RequestObservabilityContext Create(
        string? userId,
        string correlationId,
        string channel,
        string agent,
        IHostEnvironment environment,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        return new RequestObservabilityContext(
            userId: userId,
            correlationId: correlationId,
            channel: channel,
            agent: agent,
            environmentName: environment.EnvironmentName,
            timestamp: DateTimeOffset.UtcNow,
            metadata: metadata ?? new Dictionary<string, object?>());
    }

    private static IReadOnlyDictionary<string, object?> EnrichMetadata(
        string? userId,
        IReadOnlyDictionary<string, object?> metadata)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return metadata;

        // Copy to a mutable dictionary to avoid assuming the input is mutable.
        var dict = new Dictionary<string, object?>(metadata)
        {
            ["user.id"] = userId
        };

        return dict;
    }
}
