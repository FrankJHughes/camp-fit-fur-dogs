using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability;

public sealed class Metrics : IMetrics
{
    public void Increment(string name, long value = 1, IObservabilityContext? context = null)
    {
        // TODO: Implement counter metric
    }

    public void Gauge(string name, double value, IObservabilityContext? context = null)
    {
        // TODO: Implement gauge metric
    }

    public IDisposable Timer(string name, IObservabilityContext? context = null)
    {
        // TODO: Implement timer metric
        return new NoOpTimer();
    }

    private sealed class NoOpTimer : IDisposable
    {
        public void Dispose()
        {
            // No-op
        }
    }
}
