using Frank.Abstractions.ImmutableContextBuilder;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderBaseCanExecuteTests
{
    [Fact]
    public async Task SkipsSteps_WhereCanExecuteIsFalse()
    {
        var recorder = new List<string>();

        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new GatedRecordingStep<TestImmutableContext>("1", recorder, true),
            new GatedRecordingStep<TestImmutableContext>("2", recorder, false),
            new GatedRecordingStep<TestImmutableContext>("3", recorder, true)
        };

        var builder = new TestImmutableContextBuilder(steps);

        var ctx = new TestImmutableContext { Code = "initial", Now = DateTimeOffset.UtcNow };

        await builder.RunAsync(ctx, CancellationToken.None);

        recorder.Should().Equal(new[] { "1", "3" });
    }

    [Fact]
    public async Task ExecutesStepsInDeterministicOrder()
    {
        var recorder = new List<string>();

        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new RecordingStep<TestImmutableContext>("A", recorder),
            new RecordingStep<TestImmutableContext>("B", recorder),
            new RecordingStep<TestImmutableContext>("C", recorder)
        };

        var builder = new TestImmutableContextBuilder(steps);

        var ctx = new TestImmutableContext { Code = "initial", Now = DateTimeOffset.UtcNow };

        await builder.RunAsync(ctx, CancellationToken.None);

        recorder.Should().Equal(new[] { "A", "B", "C" });
    }
}
