using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;

namespace CampFitFurDogs.Application.Tests.Authentication;

public sealed class AuthCallbackServiceTests
{
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

        var service = new AuthCallbackService(steps);

        await service.HandleAsync("code", CancellationToken.None);

        Assert.Equal(new[] { "A", "B", "C" }, log);
    }
}
