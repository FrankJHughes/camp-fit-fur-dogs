namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Represents the structured, immutable context associated with a specific
/// request or operation, extending <see cref="IObservationContext"/> with
/// request‑level metadata.
///
/// <para>
/// A <see cref="IRequestObservationContext"/> flows through all observable
/// operations within a request pipeline. It enriches logs, traces, metrics,
/// and diagnostic events with correlation identifiers, environment metadata,
/// slice and subsystem attribution, and—when applicable—user identity.
/// </para>
///
/// <para>
/// This context is immutable and must be created and supplied by the
/// infrastructure layer, ensuring consistent and deterministic observability
/// behavior across modules, as required by the platform’s observability
/// guarantees (e.g., structured logging, correlation IDs, lifecycle events).
/// </para>
/// </summary>
public interface IRequestObservationContext : IObservationContext
{
    /// <summary>
    /// Gets the identifier of the authenticated user associated with the
    /// request, if any.
    ///
    /// <para>
    /// This value may be <c>null</c> for unauthenticated or system‑initiated
    /// operations. When present, it enables user‑scoped diagnostics, security
    /// monitoring, and attribution of observable events without exposing
    /// sensitive information.
    /// </para>
    /// </summary>
    string? UserId { get; }
}
