using Frank.Abstractions.ImmutableContext;
using Frank.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderBaseImmutabilityTests
{
    private sealed class TestBuilder
        : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
    {
        public TestBuilder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
            : base(steps) { }

        public Task<TestImmutableContext> RunAsync(TestImmutableContext ctx, CancellationToken ct)
            => ProcessAsync(ctx, ct);

        protected override void AssertValidTransition(
            IImmutableContextBuildStep<TestImmutableContext> step,
            TestImmutableContext before,
            TestImmutableContext after)
        {
            if (after is null)
                throw new InvalidOperationException($"Step '{step.Metadata.Id}' returned null context.");

            if (after.Code != before.Code)
                throw new InvalidOperationException($"Step '{step.Metadata.Id}' modified immutable field 'Code'.");

            if (after.Now != before.Now)
                throw new InvalidOperationException($"Step '{step.Metadata.Id}' modified immutable field 'Now'.");
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
        var builder = new TestBuilder(new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new MutatingStep(newCode: "DIFFERENT")
        });

        var act = async () => await builder.RunAsync(NewContext, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*immutable field 'Code'*");
    }

    [Fact]
    public async Task Throws_WhenStepChanges_Now()
    {
        var builder = new TestBuilder(new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new MutatingStep(newNow: DateTimeOffset.UtcNow.AddMinutes(5))
        });

        var act = async () => await builder.RunAsync(NewContext, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*immutable field 'Now'*");
    }

    [Fact]
    public async Task Throws_WhenStepReturnsNullContext()
    {
        var builder = new TestBuilder(new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new MutatingStep(returnNull: true)
        });

        var act = async () => await builder.RunAsync(NewContext, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*returned null context*");
    }
}
