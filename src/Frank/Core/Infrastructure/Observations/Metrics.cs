using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Infrastructure.Observations;

/// <summary>
/// Provides the infrastructure‑level implementation of <see cref="IMetrics"/>,
/// offering counter, gauge, and timer metric primitives.
/// <para>
/// This implementation currently acts as a no‑op placeholder. It defines the
/// contract surface for metric emission but does not yet forward data to an
/// underlying metrics backend.
/// </para>
/// <para>
/// Future implementations may integrate with systems such as Prometheus,
/// OpenTelemetry Metrics, Application Insights, or custom metric sinks.
/// </para>
/// </summary>
public sealed class Metrics : IMetrics
{
    /// <summary>
    /// Increments a counter metric by the specified value.
    /// <para>
    /// Counters represent monotonically increasing values such as request
    /// counts, error counts, or processed items.
    /// This method is currently a no‑op placeholder.
    /// </para>
    /// </summary>
    /// <param name="name">The name of the counter metric.</param>
    /// <param name="value">The amount to increment by. Defaults to <c>1</c>.</param>
    /// <param name="context">
    /// Optional request observation context associated with the metric.
    /// </param>
    public void Increment(string name, long value = 1, IRequestObservationContext? context = null)
    {
        // TODO: Implement counter metric
    }

    /// <summary>
    /// Records a gauge metric with the specified value.
    /// <para>
    /// Gauges represent instantaneous measurements such as memory usage,
    /// queue depth, or active connections.
    /// This method is currently a no‑op placeholder.
    /// </para>
    /// </summary>
    /// <param name="name">The name of the gauge metric.</param>
    /// <param name="value">The numeric value to record.</param>
    /// <param name="context">
    /// Optional request observation context associated with the metric.
    /// </param>
    public void Gauge(string name, double value, IRequestObservationContext? context = null)
    {
        // TODO: Implement gauge metric
    }

    /// <summary>
    /// Creates a timer metric used to measure the duration of an operation.
    /// <para>
    /// The returned <see cref="IDisposable"/> should be disposed when the
    /// operation completes.
    /// This implementation currently returns a no‑op timer.
    /// </para>
    /// </summary>
    /// <param name="name">The name of the timer metric.</param>
    /// <param name="context">
    /// Optional request observation context associated with the metric.
    /// </param>
    /// <returns>
    /// An <see cref="IDisposable"/> representing the timer instance.
    /// </returns>
    public IDisposable Timer(string name, IRequestObservationContext? context = null)
    {
        // TODO: Implement timer metric
        return new NoOpTimer();
    }

    /// <summary>
    /// A no‑operation timer used as a placeholder until a real timer
    /// implementation is provided.
    /// </summary>
    private sealed class NoOpTimer : IDisposable
    {
        /// <summary>
        /// Completes the timer. This implementation performs no action.
        /// </summary>
        public void Dispose()
        {
            // No-op
        }
    }
}
