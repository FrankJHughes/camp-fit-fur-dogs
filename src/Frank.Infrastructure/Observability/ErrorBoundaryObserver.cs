using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability;

public sealed class ErrorBoundaryObserver : IErrorBoundaryObserver
{
    public void OnError(Exception exception, IObservabilityContext context)
    {
        // TODO: Implement structured error observation
        // Placeholder no-op
    }
}
