using Frank.Abstractions.ImmutableContextBuilder;
using Frank.ImmutableContextBuilder;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderExceptionFlowTests
{
    private sealed class Builder
        : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
    {
        public Builder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
            : base(steps, new FakeObservabilitySink(), new FakeObservabilityContext())
        {
        }

        public Task<TestImmutableContext> RunAsync(TestImmutableContext ctx, CancellationToken ct)
            => ProcessAsync(ctx, ct);

        protected override void AssertValidTransition(
            IImmutableContextBuildStep<TestImmutableContext> step,
            TestImmutableContext before,
            TestImmutableContext after)
        { }

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

    [Fact]
    public async Task StopsPipeline_WhenStepThrows()
    {
        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new ThrowingStep<TestImmutableContext>("boom")
        };

        var builder = new Builder(steps);

        var ctx = new TestImmutableContext { Code = "x", Now = DateTimeOffset.UtcNow };

        var act = async () => await builder.RunAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("boom");
    }
}
