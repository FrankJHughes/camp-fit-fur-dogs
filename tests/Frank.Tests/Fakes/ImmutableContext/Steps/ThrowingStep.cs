using Frank.Abstractions.ImmutableContext;

namespace Frank.Tests.Fakes.ImmutableContext.Steps;

public sealed class ThrowingStep<TContext> : IImmutableContextBuildStep<TContext>
    where TContext : ImmutableContextBase
{
    private readonly string _id;
    private readonly Exception _exception;

    public ThrowingStep(string id, Exception? exception = null)
    {
        _id = id;
        _exception = exception ?? new InvalidOperationException("boom");
    }

    public bool CanExecute(TContext ctx) => true;

    public Task<TContext> ExecuteAsync(TContext ctx, CancellationToken ct)
        => throw _exception;

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(_id, $"Throwing {_id}");
}
