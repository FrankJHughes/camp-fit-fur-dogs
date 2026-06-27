using Frank.Abstractions.ImmutableContext;
using Frank.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderBaseImmutabilityTests
{
    private sealed class TestBuilder
        : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
    {
        public TestBuilder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
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
                throw new InvalidOperationException("Returned null context.");

            if (after.Code != before.Code)
                throw new InvalidOperationException("Immutable field 'Code' was modified.");

            if (after.Now != before.Now)
                throw new InvalidOperationException("Immutable field 'Now' was modified.");
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

    private static TestImmutableContext NewContext => new()
    {
        Code = "abc123",
        Now = DateTimeOffset.UtcNow
    };

    [Fact]
    public async Task Throws_WhenStepChanges_Code()
    {
        var builder = new TestBuilder(
        [
            new MutatingStep(newCode: "DIFFERENT")
        ]);

        var act = async () => await builder.RunAsync(NewContext, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Code*");
    }

    [Fact]
    public async Task Throws_WhenStepChanges_Now()
    {
        var builder = new TestBuilder(
        [
            new MutatingStep(newNow: DateTimeOffset.UtcNow.AddMinutes(1))
        ]);

        var act = async () => await builder.RunAsync(NewContext, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Now*");
    }

    [Fact]
    public async Task Throws_WhenStepReturnsNull()
    {
        var builder = new TestBuilder(new[]
        {
            new MutatingStep(returnNull: true)
        });

        var act = async () => await builder.RunAsync(NewContext, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*null*");
    }
}
