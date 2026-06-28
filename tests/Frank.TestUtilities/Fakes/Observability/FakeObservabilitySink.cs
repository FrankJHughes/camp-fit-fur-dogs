using System.Collections.Generic;
using Frank.Abstractions.Observations;

namespace Frank.TestUtilities.Fakes.Observability;

public sealed class FakeObservabilitySink : IObservationSink
{
    public sealed record CapturedTrace(
        string EventName,
        string Category,
        string Severity,
        object? Payload,
        IObservationContext Context);

    public List<CapturedTrace> Events { get; } = new();

    public void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservationContext context)
    {
        Events.Add(new CapturedTrace(eventName, category, severity, payload, context));
    }
}

