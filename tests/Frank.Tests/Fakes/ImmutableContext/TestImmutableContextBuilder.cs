using Frank.Abstractions.ImmutableContext;
using Frank.ImmutableContext;

namespace Frank.Tests.Fakes.ImmutableContext;

public sealed class TestImmutableContextBuilder
    : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
{
    public TestImmutableContextBuilder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
        : base(steps) { }

    // Public wrapper for testing
    public Task<TestImmutableContext> RunAsync(TestImmutableContext ctx, CancellationToken ct)
        => ProcessAsync(ctx, ct);

    protected override void AssertValidTransition(
        IImmutableContextBuildStep<TestImmutableContext> step,
        TestImmutableContext before,
        TestImmutableContext after)
    {
        // No immutability rules for this test context
    }

    protected override void EmitStartEvent(
        IImmutableContextBuildStep<TestImmutableContext> step,
        TestImmutableContext before)
    { }

    protected override void EmitEndEvent(
        IImmutableContextBuildStep<TestImmutableContext> step,
        TestImmutableContext before,
        TestImmutableContext after,
        long durationMs)
    { }
}
