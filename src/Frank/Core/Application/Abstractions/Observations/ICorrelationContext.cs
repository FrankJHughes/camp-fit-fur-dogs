namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Provides functionality for creating and propagating correlation identifiers
/// used to trace operations across the application.
///
/// <para>
/// Correlation identifiers allow logs, diagnostics, distributed traces, and
/// request pipelines to be linked together under a single, stable identifier.
/// Implementations of <see cref="ICorrelationContext"/> ensure that every
/// operation has a correlation ID, either newly generated or propagated from an
/// incoming request.
/// </para>
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Creates a new correlation identifier.
    ///
    /// <para>
    /// Implementations typically generate a unique, trace‑safe value such as a
    /// GUID or ULID. The returned identifier is intended to be used as the root
    /// correlation ID for a new operation or request pipeline.
    /// </para>
    /// </summary>
    /// <returns>
    /// A newly generated correlation identifier.
    /// </returns>
    string Create();

    /// <summary>
    /// Propagates an incoming correlation identifier or creates a new one if
    /// the incoming value is missing or invalid.
    ///
    /// <para>
    /// This method ensures that downstream components always receive a valid
    /// correlation ID. If the caller provides an existing identifier, it is
    /// returned; otherwise, a new identifier is generated via <see cref="Create"/>.
    /// </para>
    /// </summary>
    /// <param name="incoming">
    /// The incoming correlation identifier, if any.
    /// </param>
    /// <returns>
    /// A valid correlation identifier, either propagated or newly created.
    /// </returns>
    string Propagate(string? incoming);
}
