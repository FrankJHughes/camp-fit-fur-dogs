using Frank.Abstractions.Observability;

namespace CampFitFurDogs.TestUtilities.Fakes.Observability;

public sealed class FakeErrorBoundaryObserver : IErrorBoundaryObserver
{
    public sealed record CapturedError(Exception Exception, IRequestObservabilityContext Context);

    public List<CapturedError> Errors { get; } = [];

    public void OnError(Exception exception, IRequestObservabilityContext context)
    {
        Errors.Add(new CapturedError(exception, context));
    }
}
