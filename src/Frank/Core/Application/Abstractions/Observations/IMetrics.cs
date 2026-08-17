namespace Frank.Core.Application.Abstractions.Observations;

/// <summary>
/// Provides an abstraction for emitting metrics such as counters, gauges,
/// and timers, enabling structured observability across the application.
///
/// <para>
/// Implementations of <see cref="IMetrics"/> integrate with the application's
/// telemetry backend (e.g., OpenTelemetry, Prometheus, Application Insights)
/// to record quantitative measurements that help monitor performance,
/// throughput, resource usage, and operational behavior.
/// </para>
///
/// <para>
/// All metric operations may optionally include an
/// <see cref="IRequestObservationContext"/> to enrich emitted metrics with
/// correlation identifiers, request metadata, or other contextual information.
/// </para>
/// </summary>
public interface IMetrics
{
    /// <summary>
    /// Increments a counter metric by the specified value.
    ///
    /// <para>
    /// Counters represent monotonically increasing values such as request
    /// counts, error counts, or processed items. The default increment is 1,
    /// but callers may specify a larger value when appropriate.
    /// </para>
    /// </summary>
    /// <param name="name">
    /// The name of the counter metric to increment.
    /// </param>
    /// <param name="value">
    /// The amount to increment the counter by. Defaults to <c>1</c>.
    /// </param>
    /// <param name="context">
    /// Optional observation context used to enrich the metric with correlation
    /// or request metadata.
    /// </param>
    void Increment(string name, long value = 1, IRequestObservationContext? context = null);

    /// <summary>
    /// Records a gauge metric with the specified value.
    ///
    /// <para>
    /// Gauges represent values that may increase or decrease over time, such as
    /// memory usage, queue depth, or active connection counts.
    /// </para>
    /// </summary>
    /// <param name="name">
    /// The name of the gauge metric to record.
    /// </param>
    /// <param name="value">
    /// The current value of the gauge.
    /// </param>
    /// <param name="context">
    /// Optional observation context used to enrich the metric with correlation
    /// or request metadata.
    /// </param>
    void Gauge(string name, double value, IRequestObservationContext? context = null);

    /// <summary>
    /// Creates a timer metric that records its duration when disposed.
    ///
    /// <para>
    /// Timers measure how long an operation takes. The returned
    /// <see cref="IDisposable"/> should be wrapped in a <c>using</c> block so
    /// that the elapsed time is automatically recorded when the timer is
    /// disposed.
    /// </para>
    /// </summary>
    /// <param name="name">
    /// The name of the timer metric.
    /// </param>
    /// <param name="context">
    /// Optional observation context used to enrich the metric with correlation
    /// or request metadata.
    /// </param>
    /// <returns>
    /// An <see cref="IDisposable"/> that records the elapsed duration when
    /// disposed.
    /// </returns>
    IDisposable Timer(string name, IRequestObservationContext? context = null);
}
