using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability;

public sealed class TraceEvents : ITraceEvents
{
    public void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservabilityContext context)
    {
        // TODO: Implement vendor-specific trace emission
        // Placeholder no-op implementation
    }
}
