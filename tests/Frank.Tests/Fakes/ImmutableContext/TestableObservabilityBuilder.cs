using Frank.Abstractions.ImmutableContextBuilder;
using Frank.ImmutableContextBuilder;
using Frank.TestUtilities.Fakes.Observability;

namespace Frank.Tests.Fakes.ImmutableContext;

public sealed class TestableObservabilityBuilder
    : ImmutableContextBuilderBase<TestImmutableContext, IImmutableContextBuildStep<TestImmutableContext>>
{
    // Expose the sink without hiding the base property
    public FakeObservabilitySink TestSink { get; }

    public TestableObservabilityBuilder(IEnumerable<IImmutableContextBuildStep<TestImmutableContext>> steps)
        : base(steps, new FakeObservabilitySink(), new FakeObservabilityContext())
    {
        TestSink = (FakeObservabilitySink)base.Sink;
    }

    public Task<TestImmutableContext> RunAsync(TestImmutableContext ctx, CancellationToken ct)
        => ProcessAsync(ctx, ct);

    // Required abstract method
    protected override void AssertValidTransition(
        IImmutableContextBuildStep<TestImmutableContext> step,
        TestImmutableContext before,
        TestImmutableContext after)
    {
        // No immutability rules for this test builder
    }

    // Override observability hooks to emit simple test events
    protected override void EmitStartEvent(
        IImmutableContextBuildStep<TestImmutableContext> step,
        TestImmutableContext before)
    {
        TestSink.Emit(
            eventName: "Test.StepStart",
            category: "Test",
            severity: "Info",
            payload: new { step = step.Metadata.Id },
            context: new FakeObservabilityContext());
    }

    protected override void EmitEndEvent(
        IImmutableContextBuildStep<TestImmutableContext> step,
        TestImmutableContext before,
        TestImmutableContext after,
        long durationMs)
    {
        TestSink.Emit(
            eventName: "Test.StepEnd",
            category: "Test",
            severity: "Info",
            payload: new { step = step.Metadata.Id, durationMs },
            context: new FakeObservabilityContext());
    }
}
