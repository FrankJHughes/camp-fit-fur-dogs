#nullable enable

using System.Collections.Generic;
using Frank.Abstractions.Observability;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeTraceEvents : ITraceEvents
{
    public List<(string Name, string Category, string Message, object? Data)> Events { get; } = new();

    public void Emit(string name, string category, string message, object? data, IObservabilityContext context)
    {
        Events.Add((name, category, message, data));
    }
}
