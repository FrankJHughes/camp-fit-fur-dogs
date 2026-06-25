using Frank.Abstractions.ImmutableContextBuilder;

namespace Frank.Tests.Fakes.ImmutableContext.Steps;

public sealed class NoOpStep<TContext> : IImmutableContextBuildStep<TContext>
    where TContext : ImmutableContextBase
{
    private readonly string _id;

    public NoOpStep(string id)
    {
        _id = id;
    }

    public bool CanExecute(TContext ctx) => true;

    public Task<TContext> ExecuteAsync(TContext ctx, CancellationToken ct)
        => Task.FromResult(ctx);

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(_id, $"NoOp {_id}");
}
