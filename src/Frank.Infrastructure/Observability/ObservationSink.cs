using Frank.Abstractions.Observations;

namespace Frank.Infrastructure.Observations;

public sealed class ObservationSink : IObservationSink
{
    public void Emit(
        string eventName,
        string category,
        string severity,
        object? payload,
        IObservationContext context)
    {
        // TODO: Implement vendor-specific trace emission
        // Placeholder no-op implementation
    }
}
