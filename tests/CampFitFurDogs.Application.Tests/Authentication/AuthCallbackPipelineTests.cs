using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
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

        public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            _log.Add(_id);
            return Task.CompletedTask;
        }
    }

    private sealed class WriteContextStep : IAuthCallbackStep
    {
        private readonly string _cookieValue;

        public WriteContextStep(string cookieValue)
        {
            _cookieValue = cookieValue;
        }

        public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            ctx.Result = new AuthCallbackResult(
                CustomerId: CustomerId.From(Guid.Empty),
                Cookie: SessionCookie.FromPlaintextToken(_cookieValue),
                RedirectUrl: ""
            );

            return Task.CompletedTask;
        }
    }

    private sealed class ReadContextStep : IAuthCallbackStep
    {
        private readonly Action<AuthCallbackContext> _assert;

        public ReadContextStep(Action<AuthCallbackContext> assert)
        {
            _assert = assert;
        }

        public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            _assert(ctx);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingStep : IAuthCallbackStep
    {
        public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            throw new InvalidOperationException("boom");
        }
    }

    private sealed class FinalizeResultStep : IAuthCallbackStep
    {
        public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
        {
            ctx.Result ??= new AuthCallbackResult(
                CustomerId: CustomerId.From(Guid.Empty),
                Cookie: SessionCookie.FromPlaintextToken("final-cookie"),
                RedirectUrl: "/done"
            );

            return Task.CompletedTask;
        }
    }

    // ------------------------------------------------------------
    // 1. EXECUTES STEPS IN ORDER
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

        var clock = new FakeClock();
        var service = new AuthCallbackService(steps, clock);

        await service.HandleAsync("code", CancellationToken.None);

        log.Should().Equal("A", "B", "C");
    }

    // ------------------------------------------------------------
    // 2. CONTEXT PROPAGATION BETWEEN STEPS
    // ------------------------------------------------------------
    [Fact]
    public async Task Propagates_context_between_steps()
    {
        var steps = new IAuthCallbackStep[]
        {
            new WriteContextStep("hello-world"),
            new ReadContextStep(ctx =>
            {
                ctx.Result.Should().NotBeNull();
                ctx.Result!.Cookie.Value.Should().Be("hello-world");
            })
        };

        var clock = new FakeClock();
        var service = new AuthCallbackService(steps, clock);

        await service.HandleAsync("code", CancellationToken.None);
    }

    // ------------------------------------------------------------
    // 3. EXCEPTION SHORT-CIRCUITS PIPELINE
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

        var clock = new FakeClock();
        var service = new AuthCallbackService(steps, clock);

        var act = () => service.HandleAsync("code", CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        log.Should().Equal("A"); // B never runs
    }

    // ------------------------------------------------------------
    // 4. FINAL RESULT IS PRODUCED
    // ------------------------------------------------------------
    [Fact]
    public async Task Produces_final_result()
    {
        var steps = new IAuthCallbackStep[]
        {
            new RecordingStep(new List<string>(), "A"),
            new FinalizeResultStep()
        };

        var clock = new FakeClock();
        var service = new AuthCallbackService(steps, clock);

        var result = await service.HandleAsync("code", CancellationToken.None);

        result.Should().NotBeNull();
        result.Cookie.Value.Should().Be("final-cookie");
        result.RedirectUrl.Should().Be("/done");
    }
}
