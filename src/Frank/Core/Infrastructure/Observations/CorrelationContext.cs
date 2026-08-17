using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Provides the infrastructure‑level implementation of <see cref="ICorrelationContext"/>,
/// responsible for generating and propagating correlation identifiers used for
/// request tracing, logging, and distributed diagnostics.
/// <para>
/// A correlation ID is a stable identifier that flows through the lifetime of a
/// request or operation, enabling unified observability across services,
/// pipelines, and logs.
/// </para>
/// </summary>
public sealed class CorrelationContext : ICorrelationContext
{
    /// <summary>
    /// Creates a new correlation identifier using a GUID formatted as a
    /// 32‑character, lowercase, hyphen‑free string (<c>"N"</c> format).
    /// </summary>
    /// <returns>
    /// A newly generated correlation ID.
    /// </returns>
    public string Create()
        => Guid.NewGuid().ToString("N");

    /// <summary>
    /// Propagates an incoming correlation identifier if present; otherwise,
    /// generates a new one.
    /// <para>
    /// This ensures that every request has a valid correlation ID, while
    /// preserving upstream identifiers when available.
    /// </para>
    /// </summary>
    /// <param name="incoming">
    /// The incoming correlation ID, if any.
    /// </param>
    /// <returns>
    /// The propagated correlation ID, or a newly generated one if the incoming
    /// value is null, empty, or whitespace.
    /// </returns>
    public string Propagate(string? incoming)
        => string.IsNullOrWhiteSpace(incoming)
            ? Create()
            : incoming;
}
