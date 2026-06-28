using Frank.Abstractions.Observations;

namespace Frank.Infrastructure.Observations;

public sealed class Metrics : IMetrics
{
    public void Increment(string name, long value = 1, IRequestObservationContext? context = null)
    {
        // TODO: Implement counter metric
    }

    public void Gauge(string name, double value, IRequestObservationContext? context = null)
    {
        // TODO: Implement gauge metric
    }

    public IDisposable Timer(string name, IRequestObservationContext? context = null)
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
