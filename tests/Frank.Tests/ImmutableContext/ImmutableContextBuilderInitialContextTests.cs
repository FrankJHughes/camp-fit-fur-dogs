using Frank.Abstractions.ImmutableContextBuilder;
using Frank.ImmutableContextBuilder;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderInitialContextTests
{
    private sealed class InitialContextBuilder
        : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
    {
        public List<TestImmutableContext> Seen { get; } = new();

        public InitialContextBuilder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
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
            Seen.Add(before);
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
    public async Task UsesInitialContext_AsFirstStepInput()
    {
        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new NoOpStep<TestImmutableContext>("noop")
        };

        var builder = new InitialContextBuilder(steps);

        var ctx = new TestImmutableContext { Code = "initial", Now = DateTimeOffset.UtcNow };

        await builder.RunAsync(ctx, CancellationToken.None);

        builder.Seen.First().Should().Be(ctx);
    }
}
