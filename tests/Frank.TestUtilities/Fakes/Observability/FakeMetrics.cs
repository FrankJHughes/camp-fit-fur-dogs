#nullable enable

using System;
using System.Collections.Generic;
using Frank.Abstractions.Observations;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeMetrics : IMetrics
{
    public Dictionary<string, long> Increments { get; } = [];
    public Dictionary<string, double> Gauges { get; } = [];
    public HashSet<string> Timers { get; } = [];

    public int Count => Increments.Count + Gauges.Count + Timers.Count;

    public void Increment(string name, long value, IRequestObservationContext? context = null)
    {
        if (!Increments.ContainsKey(name))
            Increments[name] = 0;

        Increments[name] += value;
    }

    public void Gauge(string name, double value, IRequestObservationContext? context = null)
    {
        Gauges[name] = value;
    }

    public IDisposable Timer(string name, IRequestObservationContext? context = null)
    {
        Timers.Add(name);

        // Return a no-op disposable to satisfy the contract
        return new NoOpDisposable();
    }

    private sealed class NoOpDisposable : IDisposable
    {
        public void Dispose() { }
    }
}
