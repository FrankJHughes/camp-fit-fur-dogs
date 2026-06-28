using System;
using System.Collections.Generic;
using Frank.Abstractions.Observations;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeObservabilityContext : IObservationContext
{
    public string CorrelationId => "test-correlation";
    public string Channel => "TestChannel";
    public string Agent => "TestAgent";
    public string Environment => "Test";
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;

    public IReadOnlyDictionary<string, object?> Metadata { get; }
        = new Dictionary<string, object?>();
}
