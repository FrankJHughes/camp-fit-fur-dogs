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
            new GatedRecordingStep<TestImmutableContext>("1", recorder, canExecute: true),
            new GatedRecordingStep<TestImmutableContext>("2", recorder, canExecute: false),
            new GatedRecordingStep<TestImmutableContext>("3", recorder, canExecute: true)
        };

        var builder = new TestImmutableContextBuilder(steps);

        var ctx = new TestImmutableContext { Code = "initial" };

        await builder.RunAsync(ctx, CancellationToken.None);

        recorder.Should().Equal(new[] { "1", "3" });
    }

    [Fact]
    public async Task ExecutesOnlyStepsWithTrueCanExecute_InOrder()
    {
        var recorder = new List<string>();

        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new GatedRecordingStep<TestImmutableContext>("A", recorder, true),
            new GatedRecordingStep<TestImmutableContext>("B", recorder, false),
            new GatedRecordingStep<TestImmutableContext>("C", recorder, false),
            new GatedRecordingStep<TestImmutableContext>("D", recorder, true)
        };

        var builder = new TestImmutableContextBuilder(steps);

        var ctx = new TestImmutableContext { Code = "initial" };

        await builder.RunAsync(ctx, CancellationToken.None);

        recorder.Should().Equal(new[] { "A", "D" });
    }
}
