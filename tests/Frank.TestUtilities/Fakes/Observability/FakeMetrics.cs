#nullable enable

using System;
using System.Collections.Generic;
using Frank.Abstractions.Observability;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeMetrics : IMetrics
{
    public Dictionary<string, long> Increments { get; } = [];
    public Dictionary<string, double> Gauges { get; } = [];
    public HashSet<string> Timers { get; } = [];

    public int Count => Increments.Count + Gauges.Count + Timers.Count;

    public void Increment(string name, long value, IRequestObservabilityContext? context = null)
    {
        if (!Increments.ContainsKey(name))
            Increments[name] = 0;

        Increments[name] += value;
    }

    public void Gauge(string name, double value, IRequestObservabilityContext? context = null)
    {
        Gauges[name] = value;
    }

    public IDisposable Timer(string name, IRequestObservabilityContext? context = null)
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
