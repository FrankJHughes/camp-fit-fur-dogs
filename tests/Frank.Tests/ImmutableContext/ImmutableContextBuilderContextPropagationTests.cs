using Frank.Abstractions.ImmutableContextBuilder;
using Frank.ImmutableContextBuilder;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderContextPropagationTests
{
    private sealed class PropagatingBuilder
        : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
    {
        public PropagatingBuilder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
            : base(steps, new FakeObservabilitySink(), new FakeObservabilityContext())
        {
        }

        public Task<TestImmutableContext> RunAsync(TestImmutableContext ctx, CancellationToken ct)
            => ProcessAsync(ctx, ct);

        protected override void AssertValidTransition(
            IImmutableContextBuildStep<TestImmutableContext> step,
            TestImmutableContext before,
            TestImmutableContext after)
        {
            if (after is null)
                throw new InvalidOperationException("Null context not allowed.");
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

    [Fact]
    public async Task PassesUpdatedContextBetweenSteps_LastStepWins()
    {
        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new MutatingStep(newCode: "first"),
            new MutatingStep(newCode: "second")
        };

        var builder = new PropagatingBuilder(steps);

        var ctx = new TestImmutableContext
        {
            Code = "initial",
            Now = DateTimeOffset.UtcNow
        };

        var result = await builder.RunAsync(ctx, CancellationToken.None);

        result.Code.Should().Be("second");
    }
}
