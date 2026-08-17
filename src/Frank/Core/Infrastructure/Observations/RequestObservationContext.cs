using Frank.Core.Application.Abstractions.Clock;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Represents a request‑scope observation context enriched with user identity,
/// correlation metadata, environment information, and structured diagnostic
/// metadata.
/// <para>
/// This context is used for authenticated requests and provides a unified
/// observability envelope consumed by trace emitters, metrics, logs, and
/// distributed diagnostics.
/// </para>
/// </summary>
public sealed class RequestObservationContext : ObservationContextBase, IRequestObservationContext
{
    /// <summary>
    /// Gets the authenticated user identifier associated with the request.
    /// <para>
    /// If the request is unauthenticated, this value may be <c>null</c>.
    /// </para>
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestObservationContext"/>
    /// class using the provided correlation ID, channel, agent, environment,
    /// timestamp, and metadata.
    /// <para>
    /// User identity is included both as a top‑level property and, when present,
    /// injected into the metadata dictionary under the key <c>"user.id"</c>.
    /// </para>
    /// </summary>
    /// <param name="userId">The authenticated user identifier, if any.</param>
    /// <param name="correlationId">The correlation identifier for the request.</param>
    /// <param name="channel">The logical channel through which the request was initiated.</param>
    /// <param name="agent">The initiating agent or client identifier.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <param name="clock">The clock abstraction used to obtain the current timestamp.</param>
    /// <param name="metadata">Structured metadata associated with the request.</param>
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

    /// <summary>
    /// Creates a new <see cref="RequestObservationContext"/> instance using the
    /// provided values, supplying a default metadata dictionary when none is
    /// provided.
    /// </summary>
    /// <param name="userId">The authenticated user identifier, if any.</param>
    /// <param name="correlationId">The correlation identifier for the request.</param>
    /// <param name="channel">The logical channel through which the request was initiated.</param>
    /// <param name="agent">The initiating agent or client identifier.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <param name="clock">The clock abstraction used to obtain the current timestamp.</param>
    /// <param name="metadata">Optional structured metadata associated with the request.</param>
    /// <returns>A fully constructed <see cref="RequestObservationContext"/>.</returns>
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

    /// <summary>
    /// Enriches the metadata dictionary with the user identifier when present.
    /// <para>
    /// If <paramref name="userId"/> is null or whitespace, the original metadata
    /// dictionary is returned unchanged.
    /// </para>
    /// </summary>
    /// <param name="userId">The authenticated user identifier, if any.</param>
    /// <param name="metadata">The existing metadata dictionary.</param>
    /// <returns>
    /// A metadata dictionary containing the original entries plus a
    /// <c>"user.id"</c> entry when applicable.
    /// </returns>
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
