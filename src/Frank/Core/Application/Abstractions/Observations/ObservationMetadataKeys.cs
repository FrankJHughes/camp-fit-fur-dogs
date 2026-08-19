namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Defines the well‑known metadata keys used within the unified observability
/// envelope (<see cref="IObservationContext"/>).
/// <para>
/// These keys are used to attach structured diagnostic objects to the
/// <c>Metadata</c> dictionary inside <see cref="ObservationContextBase"/>.
/// </para>
/// <para>
/// All keys follow a stable naming convention (<c>api.*</c>, <c>http.*</c>,
/// <c>system.*</c>) to ensure consistency across logs, metrics, trace sinks,
/// and distributed diagnostics.
/// </para>
/// </summary>
public static class ObservationMetadataKeys
{
    /// <summary>
    /// The metadata key under which API‑level request validation diagnostics
    /// (<see cref="ApiValidationDiagnostic"/>) are stored.
    /// <para>
    /// This key is used exclusively by API boundary validation (US‑199) and
    /// must not be reused by handler‑level validation, which uses its own
    /// <c>handler.validation.*</c> namespace.
    /// </para>
    /// </summary>
    public const string ApiValidation = "api.validation";

    /// <summary>
    /// The metadata key used to store HTTP request routing information,
    /// typically populated by the Identity observability middleware.
    /// <para>
    /// This may include path, method, route pattern, or other request‑level
    /// metadata relevant to subsystem observability (US‑183).
    /// </para>
    /// </summary>
    public const string HttpRouting = "http.routing";

    /// <summary>
    /// The metadata key used to store error boundary diagnostics emitted by
    /// <see cref="IErrorBoundaryObserver"/>.
    /// <para>
    /// This key is used when unhandled exceptions occur within the request
    /// pipeline and structured error metadata must be attached to the
    /// observability envelope.
    /// </para>
    /// </summary>
    public const string ErrorBoundary = "system.error";

    /// <summary>
    /// The metadata key used to store correlation propagation metadata,
    /// typically populated by <see cref="ICorrelationContext"/>.
    /// <para>
    /// This key is used to track correlation lineage across distributed
    /// systems, background jobs, and asynchronous workflows.
    /// </para>
    /// </summary>
    public const string Correlation = "system.correlation";
}
