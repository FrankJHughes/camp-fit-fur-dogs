using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Core.Application.Tests.Fakes.Application.Authentication.Callback.Steps;

public sealed class RecordingStep : IImmutableContextBuildStep<CallbackSaveContext>
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

    public bool CanExecute(CallbackSaveContext ctx) => true;

    public Task<CallbackSaveContext> ExecuteAsync(
        CallbackSaveContext ctx,
        CancellationToken ct)
    {
        _recorder.Add(_id);
        return Task.FromResult(ctx);
    }
}
