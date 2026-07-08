using Frank.Domain.Users;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthCallbackConfig
{
    public required UserId CustomerId { get; init; }
    public required string RedirectUrl { get; init; }
    public FakeAuditLogger? AuditLogger { get; init; }
}
