using Frank.Abstractions.Observability;

namespace CampFitFurDogs.TestUtilities.Fakes.Observability;

public sealed class FakeMetrics : IMetrics
{
    public sealed record IncrementCall(string Name, long Value, IObservabilityContext? Context);
    public sealed record GaugeCall(string Name, double Value, IObservabilityContext? Context);
    public sealed record TimerCall(string Name, TimeSpan Duration, IObservabilityContext? Context);

    public List<IncrementCall> Increments { get; } = [];
    public List<GaugeCall> Gauges { get; } = [];
    public List<TimerCall> Timers { get; } = [];

    public void Increment(string name, long value = 1, IObservabilityContext? context = null)
    {
        Increments.Add(new IncrementCall(name, value, context));
    }

    public void Gauge(string name, double value, IObservabilityContext? context = null)
    {
        Gauges.Add(new GaugeCall(name, value, context));
    }

    public IDisposable Timer(string name, IObservabilityContext? context = null)
        => new TimerScope(this, name, context);

    private sealed class TimerScope : IDisposable
    {
        private readonly FakeMetrics _owner;
        private readonly string _name;
        private readonly IObservabilityContext? _context;
        private readonly DateTimeOffset _start;
        private bool _disposed;

        public TimerScope(FakeMetrics owner, string name, IObservabilityContext? context)
        {
            _owner = owner;
            _name = name;
            _context = context;
            _start = DateTimeOffset.UtcNow;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            var duration = DateTimeOffset.UtcNow - _start;
            _owner.Timers.Add(new TimerCall(_name, duration, _context));
        }
    }
}
