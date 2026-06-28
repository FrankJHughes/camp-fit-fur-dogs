using Frank.Abstractions.Observations;

namespace Frank.Infrastructure.Observations;

public sealed class ErrorBoundaryObserver : IErrorBoundaryObserver
{
    private readonly IObservationSink _sink;

    public ErrorBoundaryObserver(IObservationSink sink)
    {
        _sink = sink;
    }

    public void OnError(Exception exception, IRequestObservationContext context)
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
