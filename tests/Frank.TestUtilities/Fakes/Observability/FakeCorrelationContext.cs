#nullable enable

using System.Collections.Generic;
using Frank.Abstractions.Observations;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeCorrelationContext : ICorrelationContext
{
    public List<string?> Propagated { get; } = [];

    public string Create() => "fake-correlation-id";

    public string Propagate(string? incoming)
    {
        Propagated.Add(incoming);
        return incoming ?? "fake-correlation-id";
    }
}
