using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;

namespace Frank.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class RecordingStep : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly string _id;
    private readonly List<string> _recorder;

    public RecordingStep(string id, List<string> recorder)
    {
        _id = id;
        _recorder = recorder;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(_id, $"Recording Step {_id}");

    public bool CanExecute(ApplicationAuthCallbackContext ctx) => true;

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        _recorder.Add(_id);
        return Task.FromResult(ctx);
    }
}
