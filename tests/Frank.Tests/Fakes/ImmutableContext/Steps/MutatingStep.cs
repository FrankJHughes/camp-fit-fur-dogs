using Frank.Abstractions.ImmutableContextBuilder;

namespace Frank.Tests.Fakes.ImmutableContext.Steps;

public sealed class MutatingStep : IImmutableContextBuildStep<TestImmutableContext>
{
    private readonly string? _newCode;
    private readonly DateTimeOffset? _newNow;
    private readonly bool _returnNull;

    public MutatingStep(string? newCode = null, DateTimeOffset? newNow = null, bool returnNull = false)
    {
        _newCode = newCode;
        _newNow = newNow;
        _returnNull = returnNull;
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata("MutatingStep", "Mutating Step");

    public bool CanExecute(TestImmutableContext ctx) => true;

    public Task<TestImmutableContext> ExecuteAsync(TestImmutableContext ctx, CancellationToken ct)
    {
        if (_returnNull)
            return Task.FromResult<TestImmutableContext>(null!);

        return Task.FromResult(
            ctx with
            {
                Code = _newCode ?? ctx.Code,
                Now = _newNow ?? ctx.Now
            }
        );
    }
}
