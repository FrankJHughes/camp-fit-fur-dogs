namespace Frank.Core.Api.Routing.Validation;

/// <summary>
/// Defines the structured event names emitted during API‑level request
/// validation. These events form part of the unified observability envelope
/// (US‑199) and are consumed by logs, metrics, trace sinks, and distributed
/// diagnostics.
/// <para>
/// All event names follow the <c>api.validation.*</c> naming convention to
/// clearly distinguish API‑boundary validation from handler‑level validation
/// (<c>handler.validation.*</c>).
/// </para>
/// </summary>
public static class ApiValidationEvents
{
    /// <summary>
    /// Emitted when API request validation begins.
    /// <para>
    /// Includes correlation ID, route, and DTO type. Used to establish the
    /// beginning of the validation window for duration measurement.
    /// </para>
    /// </summary>
    public const string Start = "api.validation.start";

    /// <summary>
    /// Emitted when API request validation completes successfully or fails.
    /// <para>
    /// Includes correlation ID, route, DTO type, and validation duration.
    /// </para>
    /// </summary>
    public const string End = "api.validation.end";

    /// <summary>
    /// Emitted when API request validation fails.
    /// <para>
    /// Includes correlation ID, route, DTO type, and structured validation
    /// error metadata. No PII is ever logged.
    /// </para>
    /// </summary>
    public const string Failed = "api.validation.failed";
}
