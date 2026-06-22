// src/Frank/Abstractions/Observability/IMetrics.cs
namespace Frank.Abstractions.Observability;

/// <summary>
/// Provides metric emission capabilities such as counters, gauges, and timers.
/// </summary>
public interface IMetrics
{
    /// <summary>
    /// Increments a counter metric.
    /// </summary>
    void Increment(string name, long value = 1, IObservabilityContext? context = null);

    /// <summary>
    /// Records a gauge metric.
    /// </summary>
    void Gauge(string name, double value, IObservabilityContext? context = null);

    /// <summary>
    /// Creates a timer metric that records duration when disposed.
    /// </summary>
    IDisposable Timer(string name, IObservabilityContext? context = null);
}
