using Frank.Abstractions.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext;
using Frank.Tests.Fakes.ImmutableContext.Steps;

namespace Frank.Tests.ImmutableContext;

public sealed class ImmutableContextBuilderObservabilityEventsTests
{
    [Fact]
    public async Task EmitsStartAndEndEvents_ForEachStep()
    {
        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new NoOpStep<TestImmutableContext>("A"),
            new NoOpStep<TestImmutableContext>("B")
        };

        var builder = new TestableObservabilityBuilder(steps);

        var ctx = new TestImmutableContext
        {
            Code = "initial",
            Now = DateTimeOffset.UtcNow
        };

        await builder.RunAsync(ctx, CancellationToken.None);

        var events = builder.TestSink.Events;

        bool hasStartA = events.Any(e =>
        {
            dynamic p = e.Payload!;
            return e.EventName == "Test.StepStart" && p.step == "A";
        });

        bool hasEndA = events.Any(e =>
        {
            dynamic p = e.Payload!;
            return e.EventName == "Test.StepEnd" && p.step == "A";
        });

        bool hasStartB = events.Any(e =>
        {
            dynamic p = e.Payload!;
            return e.EventName == "Test.StepStart" && p.step == "B";
        });

        bool hasEndB = events.Any(e =>
        {
            dynamic p = e.Payload!;
            return e.EventName == "Test.StepEnd" && p.step == "B";
        });

        hasStartA.Should().BeTrue();
        hasEndA.Should().BeTrue();
        hasStartB.Should().BeTrue();
        hasEndB.Should().BeTrue();
    }

    [Fact]
    public async Task EmitsEvents_InCorrectOrder()
    {
        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new NoOpStep<TestImmutableContext>("A"),
            new NoOpStep<TestImmutableContext>("B")
        };

        var builder = new TestableObservabilityBuilder(steps);

        var ctx = new TestImmutableContext
        {
            Code = "initial",
            Now = DateTimeOffset.UtcNow
        };

        await builder.RunAsync(ctx, CancellationToken.None);

        var sequence = builder.TestSink.Events
            .Select(e =>
            {
                dynamic p = e.Payload!;
                return $"{e.EventName}:{p.step}";
            })
            .ToList();

        sequence.Should().Equal(new[]
        {
            "Test.StepStart:A",
            "Test.StepEnd:A",
            "Test.StepStart:B",
            "Test.StepEnd:B"
        });
    }

    [Fact]
    public async Task EmitsDuration_OnEndEvent()
    {
        var steps = new IImmutableContextBuildStep<TestImmutableContext>[]
        {
            new NoOpStep<TestImmutableContext>("A")
        };

        var builder = new TestableObservabilityBuilder(steps);

        var ctx = new TestImmutableContext
        {
            Code = "initial",
            Now = DateTimeOffset.UtcNow
        };

        await builder.RunAsync(ctx, CancellationToken.None);

        var endEvent = builder.TestSink.Events
            .Single(e => e.EventName == "Test.StepEnd");

        dynamic payload = endEvent.Payload!;
        long duration = payload.durationMs;

        duration.Should().BeGreaterThanOrEqualTo(0);
    }
}
