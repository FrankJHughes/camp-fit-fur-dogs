namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Represents structured, PII‑safe diagnostic metadata emitted during
/// API‑level request validation (US‑199).
/// <para>
/// This diagnostic object is attached to the unified request‑scope
/// <see cref="IRequestObservationContext"/> via the
/// <see cref="ObservationMetadataKeys.ApiValidation"/> metadata key.
/// </para>
/// <para>
/// The diagnostic payload is intentionally minimal and contains no sensitive
/// information. It is safe for logs, metrics, trace sinks, distributed
/// diagnostics, and external observability systems.
/// </para>
/// </summary>
public sealed class ApiValidationDiagnostic
{
    /// <summary>
    /// Gets the route or endpoint display name associated with the request
    /// being validated.
    /// <para>
    /// This value is typically derived from <see cref="Microsoft.AspNetCore.Http.Endpoint.DisplayName"/>
    /// and helps correlate validation events with specific API endpoints.
    /// </para>
    /// </summary>
    public string Route { get; init; } = default!;

    /// <summary>
    /// Gets the fully qualified type name of the request DTO being validated.
    /// <para>
    /// This value enables slice‑level diagnostics and helps developers identify
    /// which request type triggered validation failures.
    /// </para>
    /// </summary>
    public string DtoType { get; init; } = default!;

    /// <summary>
    /// Gets the number of validation errors produced by FluentValidation.
    /// <para>
    /// This value is used for metrics, dashboards, and structured logs.
    /// </para>
    /// </summary>
    public int ErrorCount { get; init; }

    /// <summary>
    /// Gets the list of validation error codes or property names associated
    /// with the validation failure.
    /// <para>
    /// No PII or raw input values are ever included. Only error codes or
    /// property identifiers are emitted to preserve privacy and purity rules.
    /// </para>
    /// </summary>
    public IReadOnlyList<string> ErrorCodes { get; init; } = new List<string>();

    /// <summary>
    /// Gets the total duration of the validation operation in milliseconds.
    /// <para>
    /// This value is used for performance diagnostics, latency analysis, and
    /// validation‑specific timing metrics.
    /// </para>
    /// </summary>
    public long DurationMs { get; init; }
}
