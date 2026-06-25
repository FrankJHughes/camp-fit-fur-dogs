using Frank.Abstractions.Observability;

namespace Frank.Infrastructure.Observability;

public sealed class ErrorBoundaryObserver : IErrorBoundaryObserver
{
    private readonly IObservabilitySink _sink;

    public ErrorBoundaryObserver(IObservabilitySink sink)
    {
        _sink = sink;
    }

    public void OnError(Exception exception, IRequestObservabilityContext context)
    {
        // Emit a structured error event into the unified observability sink
        _sink.Emit(
            eventName: "request.error",
            category: "error",
            severity: "error",
            payload: new
            {
                exception.Message,
                exception.StackTrace,
                exception.Source,
                ExceptionType = exception.GetType().FullName
            },
            context: context
        );
    }
}
