using CampFitFurDogs.Application.Abstractions.Time;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Pipeline;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackServiceTests
{
    private sealed class FakeClock : ISystemClock
    {
        public DateTimeOffset UtcNow { get; set; } =
            new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
    }

    private sealed class RecordingStep : IAuthCallbackStep
    {
        private readonly List<string> _log;
        private readonly string _id;
        public AuthCallbackStepMetadata Metadata =>
            new(_id, $"Step {_id}", AuthCallbackStepCategory.Precondition);

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

    private static readonly string[] expected = new[] { "A", "B", "C" };

    [Fact]
    public async Task ExecutesStepsInOrder()
    {
        var log = new List<string>();

        var steps = new IAuthCallbackStep[]
        {
            new RecordingStep(log, "A"),
            new RecordingStep(log, "B"),
            new RecordingStep(log, "C")
        };

        var pipeline = new AuthCallbackPipeline(steps);
        var service = new AuthCallbackService(pipeline, new FakeClock());

        // This will fail unless the pipeline produces required fields,
        // so we call the pipeline directly for ordering tests.
        var ctx = new AuthCallbackContext("code");

        await pipeline.ExecuteAsync(ctx, CancellationToken.None);

        Assert.Equal(expected, log);
    }
}
