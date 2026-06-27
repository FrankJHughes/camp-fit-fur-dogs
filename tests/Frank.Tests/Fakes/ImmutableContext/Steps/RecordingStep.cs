using Frank.Abstractions.ImmutableContext;

namespace Frank.Tests.Fakes.ImmutableContext.Steps;

public sealed class RecordingStep<TContext> : IImmutableContextBuildStep<TContext>
    where TContext : ImmutableContextBase
{
    private readonly string _id;
    private readonly List<string> _recorder;

    public RecordingStep(string id, List<string> recorder)
    {
        _id = id;
        _recorder = recorder;
    }

    public bool CanExecute(TContext ctx) => true;

    public Task<TContext> ExecuteAsync(TContext ctx, CancellationToken ct)
    {
        _recorder.Add(_id);
        return Task.FromResult(ctx);
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(_id, $"Recording {_id}");
}
