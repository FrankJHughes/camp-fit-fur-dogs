using Frank.Application.Abstractions.Audit;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Application.Identity.Callback.Steps;
using Frank.TestUtilities.Fakes.Authentication.Callback;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

public sealed class AuditLoginStepTests
{
    private sealed class FakeAuditLogger : IAuditLogger
    {
        public Guid? ReceivedUserId { get; private set; }
        public string? ReceivedExternalId { get; private set; }

        public Task LoginSucceeded(Guid userId, string externalId)
        {
            ReceivedUserId = userId;
            ReceivedExternalId = externalId;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ExecuteAsync_LogsLogin()
    {
        var audit = new FakeAuditLogger();
        var step = new AuditLoginStep(audit);

        var ctx = new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create("sub-123"),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
        };

        await step.ExecuteAsync(ctx, CancellationToken.None);

        audit.ReceivedUserId.Should().Be(ctx.UserId);
        audit.ReceivedExternalId.Should().Be("sub-123");
    }

    [Fact]
    public void CanExecute_OnlyWhenUserIdIsSet()
    {
        var step = new AuditLoginStep(new FakeAuditLogger());

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = null
        }).Should().BeFalse();

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            UserId = Guid.NewGuid()
        }).Should().BeTrue();
    }
}
