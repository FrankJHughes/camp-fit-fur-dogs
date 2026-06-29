// src/Frank/Abstractions/Observability/IMetrics.cs
namespace Frank.Abstractions.Observations;

/// <summary>
/// Provides metric emission capabilities such as counters, gauges, and timers.
/// </summary>
public interface IMetrics
{
    /// <summary>
    /// Increments a counter metric.
    /// </summary>
    void Increment(string name, long value = 1, IRequestObservationContext? context = null);

    /// <summary>
    /// Records a gauge metric.
    /// </summary>
    void Gauge(string name, double value, IRequestObservationContext? context = null);

    /// <summary>
    /// Creates a timer metric that records duration when disposed.
    /// </summary>
    IDisposable Timer(string name, IRequestObservationContext? context = null);
}
