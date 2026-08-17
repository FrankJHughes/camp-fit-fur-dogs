namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Emits structured trace events for observability.
///
/// <para>
/// An <see cref="IObservationSink"/> acts as the final output channel for
/// observability signals such as trace events, structured logs, or telemetry
/// envelopes. Implementations are provided by the infrastructure layer and
/// integrate with the chosen backend (e.g., OpenTelemetry, Application
/// Insights, Seq, or custom pipelines).
/// </para>
///
/// <para>
/// Sinks are responsible only for emission, not for constructing observation
/// context or correlation identifiers. They receive fully enriched events and
/// forward them to the underlying telemetry system.
/// </para>
/// </summary>
public interface IObservationSink
{
    /// <summary>
    /// Emits a structured event with the given metadata and observation context.
    ///
    /// <para>
    /// Events typically represent meaningful occurrences such as state changes,
    /// boundary transitions, failures, or domain‑level signals. The
    /// <paramref name="context"/> provides correlation identifiers, environment
    /// metadata, and other structured information that enriches the event.
    /// </para>
    ///
    /// <para>
    /// Implementations should treat this method as fire‑and‑forget: the event
    /// must be emitted reliably, but the call should not block the request
    /// pipeline unless required by the underlying telemetry system.
    /// </para>
    /// </summary>
    /// <param name="eventName">
    /// The name of the event being emitted (e.g., "OrderCreated", "PipelineError").
    /// </param>
    /// <param name="category">
    /// A category or grouping used to classify the event (e.g., "Domain",
    /// "Infrastructure", "Security").
    /// </param>
    /// <param name="severity">
    /// The severity level of the event (e.g., "Info", "Warning", "Error",
    /// "Critical").
    /// </param>
    /// <param name="payload">
    /// Optional structured data associated with the event. May be any object
    /// that the sink can serialize or transform.
    /// </param>
    /// <param name="context">
    /// The observation context providing correlation identifiers, environment
    /// metadata, and other structured observability information.
    /// </param>
    void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservationContext context);
}
