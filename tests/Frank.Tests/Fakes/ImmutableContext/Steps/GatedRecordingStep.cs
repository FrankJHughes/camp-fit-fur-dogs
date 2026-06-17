using Frank.Abstractions.ImmutableContext;

namespace Frank.Tests.Fakes.ImmutableContext.Steps;

public sealed class GatedRecordingStep<TContext>
    : IImmutableContextBuildStep<TContext>
    where TContext : ImmutableContextBase
{
    private readonly string _id;
    private readonly List<string> _recorder;
    private readonly bool _canExecute;

    public GatedRecordingStep(string id, List<string> recorder, bool canExecute)
    {
        _id = id;
        _recorder = recorder;
        _canExecute = canExecute;
    }

    public bool CanExecute(TContext ctx) => _canExecute;

    public Task<TContext> ExecuteAsync(TContext ctx, CancellationToken ct)
    {
        _recorder.Add(_id);
        return Task.FromResult(ctx);
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(_id, $"Step {_id}");
}
