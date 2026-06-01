using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackPipelineTests
{
    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------

    private sealed class FakeClock : ISystemClock
    {
        public DateTimeOffset UtcNow { get; set; } =
            new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
    }

    private sealed class RecordingStep : IAuthCallbackStep
    {
        private readonly List<string> _log;
        private readonly string _id;

        public RecordingStep(List<string> log, string id)
        {
            _log = log;
            _id = id;
        }

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            _log.Add(_id);
            return Task.FromResult(ctx);
        }
    }

    private sealed class WriteContextStep : IAuthCallbackStep
    {
        private readonly string _cookieValue;

        public WriteContextStep(string cookieValue)
        {
            _cookieValue = cookieValue;
        }

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            ctx.SessionCookie = SessionCookie.FromPlaintextToken(_cookieValue);
            return Task.FromResult(ctx);
        }
    }

    private sealed class ReadContextStep : IAuthCallbackStep
    {
        private readonly Action<AuthCallbackContext> _assert;

        public ReadContextStep(Action<AuthCallbackContext> assert)
        {
            _assert = assert;
        }

        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            _assert(ctx);
            return Task.FromResult(ctx);
        }
    }

    private sealed class ThrowingStep : IAuthCallbackStep
    {
        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            throw new InvalidOperationException("boom");
        }
    }

    private sealed class FinalizeResultStep : IAuthCallbackStep
    {
        public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            // Ensure a valid customer ID
            ctx.CustomerId ??= Guid.NewGuid();

            // Ensure a valid session
            ctx.Session ??= Session.Create(
                SessionTokenHash.From("cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc"),
                CustomerId.From(ctx.CustomerId.Value),
                DateTimeOffset.UtcNow
            );

            // Ensure a valid cookie
            ctx.SessionCookie ??= SessionCookie.FromPlaintextToken("final-cookie");

            // Ensure a redirect URL
            ctx.RedirectUrl ??= "/done";

            return Task.FromResult(ctx);
        }
    }

    // ------------------------------------------------------------
    // 1. EXECUTES STEPS IN ORDER (pipeline only — NOT service)
    // ------------------------------------------------------------
    [Fact]
    public async Task Executes_steps_in_order()
    {
        var log = new List<string>();

        var steps = new IAuthCallbackStep[]
        {
            new RecordingStep(log, "A"),
            new RecordingStep(log, "B"),
            new RecordingStep(log, "C")
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        log.Should().Equal("A", "B", "C");
    }

    // ------------------------------------------------------------
    // 2. CONTEXT PROPAGATION BETWEEN STEPS (pipeline only)
    // ------------------------------------------------------------
    [Fact]
    public async Task Propagates_context_between_steps()
    {
        var steps = new IAuthCallbackStep[]
        {
            new WriteContextStep("hello-world"),
            new ReadContextStep(ctx =>
            {
                ctx.SessionCookie.Should().NotBeNull();
                ctx.SessionCookie!.Value.Should().Be("hello-world");
            })
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(ctx, CancellationToken.None);
    }

    // ------------------------------------------------------------
    // 3. EXCEPTION SHORT-CIRCUITS PIPELINE (pipeline only)
    // ------------------------------------------------------------
    [Fact]
    public async Task Stops_when_a_step_throws()
    {
        var log = new List<string>();

        var steps = new IAuthCallbackStep[]
        {
            new RecordingStep(log, "A"),
            new ThrowingStep(),
            new RecordingStep(log, "B") // must NOT run
        };

        var pipeline = new AuthCallbackPipeline(steps);

        var ctx = new AuthCallbackContext("code");

        Func<Task> act = () => pipeline.ExecuteAsync(ctx, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        log.Should().Equal("A"); // B never runs
    }

    // ------------------------------------------------------------
    // 4. FINAL RESULT IS PRODUCED (service — requires full context)
    // ------------------------------------------------------------
    [Fact]
    public async Task Produces_final_result()
    {
        var steps = new IAuthCallbackStep[]
        {
            new FinalizeResultStep()
        };

        var pipeline = new AuthCallbackPipeline(steps);
        var service = new AuthCallbackService(pipeline, new FakeClock());

        var result = await service.HandleAsync("code", CancellationToken.None);

        result.Cookie.Value.Should().Be("final-cookie");
        result.RedirectUrl.Should().Be("/done");
    }
}
