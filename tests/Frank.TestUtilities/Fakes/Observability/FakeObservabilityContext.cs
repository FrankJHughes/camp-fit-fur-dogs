#nullable enable

using System;
using System.Collections.Generic;
using Frank.Abstractions.Observability;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeObservabilityContext : IObservabilityContext
{
    public string CorrelationId { get; init; } = "test-correlation";
    public string Channel { get; init; } = "test-slice";
    public string Agent { get; init; } = "test-module";
    public string Environment { get; init; } = "Test";
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public IReadOnlyDictionary<string, object?> Metadata { get; init; }
        = new Dictionary<string, object?>();
}
