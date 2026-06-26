using System.Collections.Generic;
using Frank.Abstractions.Observability;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeObservabilitySink : IObservabilitySink
{
    public sealed record CapturedTrace(
        string EventName,
        string Category,
        string Severity,
        object? Payload,
        IObservabilityContext Context);

    public List<CapturedTrace> Events { get; } = new();

    public void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservabilityContext context)
    {
        Events.Add(new CapturedTrace(eventName, category, severity, payload, context));
    }
}

