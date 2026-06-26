using System;
using System.Collections.Generic;
using Frank.Abstractions.Observability;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeObservabilityContext : IObservabilityContext
{
    public string CorrelationId => "test-correlation";
    public string Channel => "TestChannel";
    public string Agent => "TestAgent";
    public string Environment => "Test";
    public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;

    public IReadOnlyDictionary<string, object?> Metadata { get; }
        = new Dictionary<string, object?>();
}
