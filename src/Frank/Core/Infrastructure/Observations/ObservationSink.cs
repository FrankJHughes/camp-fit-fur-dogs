using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Provides the infrastructure‑level implementation of <see cref="IObservationSink"/>,
/// responsible for emitting structured observability events into the underlying
/// tracing, logging, or telemetry backend.
/// <para>
/// This implementation currently acts as a no‑op placeholder. It defines the
/// contract surface for event emission but does not yet forward data to a
/// vendor‑specific observability system.
/// </para>
/// <para>
/// Future implementations may integrate with OpenTelemetry, Application Insights,
/// Prometheus, Elastic, or other trace/metric/log aggregation platforms.
/// </para>
/// </summary>
public sealed class ObservationSink : IObservationSink
{
    /// <summary>
    /// Emits a structured observability event containing an event name,
    /// category, severity, payload, and associated observation context.
    /// <para>
    /// The payload is expected to be a serializable object representing
    /// structured diagnostic data.
    /// This method is currently a no‑op placeholder.
    /// </para>
    /// </summary>
    /// <param name="eventName">The logical name of the event.</param>
    /// <param name="category">The category of the event (e.g., <c>request</c>, <c>error</c>, <c>metric</c>).</param>
    /// <param name="severity">The severity level (e.g., <c>info</c>, <c>warn</c>, <c>error</c>).</param>
    /// <param name="payload">Structured diagnostic data associated with the event.</param>
    /// <param name="context">The observation context providing correlation and metadata.</param>
    public void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservationContext context)
    {
        // TODO: Implement vendor-specific trace emission
        // Placeholder no-op implementation
    }
}
