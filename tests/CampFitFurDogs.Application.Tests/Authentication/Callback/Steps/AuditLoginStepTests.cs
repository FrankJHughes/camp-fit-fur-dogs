using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Authentication.Callback.Steps;
using CampFitFurDogs.Application.Tests.Fakes.Authentication.Callback;

namespace CampFitFurDogs.Application.Tests.Authentication.Callback.Steps;

public sealed class AuditLoginStepTests
{
    private sealed class FakeAuditLogger : IAuditLogger
    {
        public Guid? ReceivedCustomerId { get; private set; }
        public string? ReceivedExternalId { get; private set; }

        public Task LoginSucceeded(Guid customerId, string externalId)
        {
            ReceivedCustomerId = customerId;
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
            CustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
        };

        await step.ExecuteAsync(ctx, CancellationToken.None);

        audit.ReceivedCustomerId.Should().Be(ctx.CustomerId);
        audit.ReceivedExternalId.Should().Be("sub-123");
    }

    [Fact]
    public void CanExecute_OnlyWhenCustomerIdIsSet()
    {
        var step = new AuditLoginStep(new FakeAuditLogger());

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            CustomerId = null
        }).Should().BeFalse();

        step.CanExecute(new ApplicationAuthCallbackContext
        {
            External = FakeFrankAuthCallbackResult.Create(),
            Now = DateTimeOffset.UtcNow,
            CustomerId = Guid.NewGuid()
        }).Should().BeTrue();
    }
}
