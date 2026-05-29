using CampFitFurDogs.Application.Abstractions.Audit;

namespace CampFitFurDogs.Api.Tests.Fakes;

public class FakeAuditLogger : IAuditLogger
{
    public bool WasCalled { get; private set; }
    public Guid LastCustomerId { get; private set; }
    public string LastExternalId { get; private set; } = string.Empty;

    public Task LoginSucceeded(Guid customerId, string externalId)
    {
        WasCalled = true;
        LastCustomerId = customerId;
        LastExternalId = externalId;
        return Task.CompletedTask;
    }
}
