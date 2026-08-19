namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Represents the structured, immutable context that flows through all
/// observable operations in the application.
///
/// <para>
/// Observation contexts provide the foundational metadata required for
/// correlation, tracing, logging, and metric enrichment. They are created and
/// supplied by the infrastructure layer and remain immutable throughout the
/// lifetime of the operation, ensuring deterministic and consistent
/// observability behavior.
/// </para>
///
/// <para>
/// This context is intended to be passed through pipelines, middleware,
/// dispatch layers, and instrumentation components so that every observable
/// event carries the same correlated metadata.
/// </para>
/// </summary>
public interface IObservationContext
{
    /// <summary>
    /// Gets the unique identifier used to correlate logs, traces, and metrics
    /// across boundaries and subsystems.
    ///
    /// <para>
    /// Correlation identifiers allow distributed operations to be linked
    /// together, enabling end‑to‑end tracing and unified diagnostic workflows.
    /// </para>
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the vertical slice, capability, or functional channel emitting the
    /// event.
    ///
    /// <para>
    /// Channels help categorize observability data by feature area, enabling
    /// more granular filtering and analysis.
    /// </para>
    /// </summary>
    string Channel { get; }

    /// <summary>
    /// Gets the module or subsystem within the slice that produced the event.
    ///
    /// <para>
    /// Agents provide fine‑grained attribution for observability signals,
    /// supporting diagnostics at the component level.
    /// </para>
    /// </summary>
    string Agent { get; }

    /// <summary>
    /// Gets the environment in which the event occurred (e.g., Development,
    /// Staging, Production).
    ///
    /// <para>
    /// Environment metadata enables environment‑specific filtering,
    /// troubleshooting, and operational analysis.
    /// </para>
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Gets the timestamp associated with the event or operation.
    ///
    /// <para>
    /// Timestamps ensure temporal accuracy for logs, traces, and metrics,
    /// supporting chronological reconstruction and performance analysis.
    /// </para>
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets additional metadata relevant to the operation.
    ///
    /// <para>
    /// Metadata may include request identifiers, feature flags, user context,
    /// or any other structured information that enriches observability signals.
    /// </para>
    /// </summary>
    IReadOnlyDictionary<string, object?> Metadata { get; }

    /// <summary>
    /// Adds or updates a metadata entry in the observation context.
    ///
    /// <para>
    /// This method is used by instrumentation components (e.g., API validation,
    /// error boundaries, routing diagnostics) to enrich the unified observability
    /// envelope with structured, PII‑safe diagnostic information.
    /// </para>
    ///
    /// <para>
    /// Although observation contexts are conceptually immutable, controlled
    /// metadata enrichment is permitted and required for subsystem observability
    /// (US‑183, US‑199). Implementations must ensure thread‑safety and preserve
    /// deterministic behavior.
    /// </para>
    /// </summary>
    /// <param name="key">The metadata key to add or update.</param>
    /// <param name="value">The metadata value to associate with the key.</param>
    void AddMetadata(string key, object? value);
}
