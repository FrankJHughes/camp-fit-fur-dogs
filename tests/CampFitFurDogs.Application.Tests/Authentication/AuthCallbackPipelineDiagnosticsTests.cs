using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Pipeline;
using Xunit;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackPipelineDiagnosticsTests
{
    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private sealed class NoOpStep : IAuthCallbackStep
    {
        private readonly string _id;
        public StepMetadata Metadata => new(_id, $"Step {_id}");

        public NoOpStep(string id)
        {
            _id = id;
        }

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx);
        }
    }

    private sealed class MutatingStep : IAuthCallbackStep
    {
        public StepMetadata Metadata => new("Mutating", "Mutating Step");

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            return Task.FromResult(ctx with { RedirectUrl = "/changed" });
        }
    }

    // ------------------------------------------------------------
    // 1. Emits START and END events for each step
    // ------------------------------------------------------------
    [Fact]
    public async Task Emits_start_and_end_events_for_each_step()
    {
        var events = new List<PipelineDiagnosticEvent>();

        var steps = new IAuthCallbackStep[]
        {
            new NoOpStep("A"),
            new NoOpStep("B")
        };

        var pipeline = new AuthCallbackPipeline(steps, events.Add);

        var ctx = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        events.Count.Should().Be(4); // 2 steps → 4 events
        events.Select(e => e.Phase).Should().Equal("Start", "End", "Start", "End");
    }

    // ------------------------------------------------------------
    // 2. Metadata is correct for each event
    // ------------------------------------------------------------
    [Fact]
    public async Task Emits_correct_metadata()
    {
        var events = new List<PipelineDiagnosticEvent>();

        var steps = new IAuthCallbackStep[]
        {
            new NoOpStep("A"),
            new NoOpStep("B")
        };

        var pipeline = new AuthCallbackPipeline(steps, events.Add);

        var ctx = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        events[0].StepId.Should().Be("A");
        events[0].StepName.Should().Be("Step A");

        events[2].StepId.Should().Be("B");
        events[2].StepName.Should().Be("Step B");
    }

    // ------------------------------------------------------------
    // 3. Duration semantics are correct
    // ------------------------------------------------------------
    [Fact]
    public async Task Duration_semantics_are_correct()
    {
        var events = new List<PipelineDiagnosticEvent>();

        var steps = new IAuthCallbackStep[]
        {
            new NoOpStep("A")
        };

        var pipeline = new AuthCallbackPipeline(steps, events.Add);

        var ctx = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        var start = events[0];
        var end = events[1];

        start.DurationMs.Should().BeNull();
        end.DurationMs.Should().NotBeNull();
        end.DurationMs!.Value.Should().BeGreaterThanOrEqualTo(0);
    }

    // ------------------------------------------------------------
    // 4. Before/After snapshots are correct
    // ------------------------------------------------------------
    [Fact]
    public async Task Before_and_after_snapshots_are_correct()
    {
        var events = new List<PipelineDiagnosticEvent>();

        var steps = new IAuthCallbackStep[]
        {
            new MutatingStep()
        };

        var pipeline = new AuthCallbackPipeline(steps, events.Add);

        var initial = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(initial, CancellationToken.None);

        var start = events[0];
        var end = events[1];

        // Start event: Before == After
        start.Before.Should().Be(start.After);

        // End event: After contains mutation
        end.Before.RedirectUrl.Should().BeNull();
        end.After.RedirectUrl.Should().Be("/changed");
    }

    // ------------------------------------------------------------
    // 5. Pipeline runs without a trace delegate
    // ------------------------------------------------------------
    [Fact]
    public async Task Pipeline_runs_without_trace_delegate()
    {
        var steps = new IAuthCallbackStep[]
        {
            new NoOpStep("A")
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        var result = await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        result.Should().NotBeNull();
    }
}
