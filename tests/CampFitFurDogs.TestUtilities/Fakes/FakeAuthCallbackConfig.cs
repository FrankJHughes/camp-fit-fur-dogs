using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthCallbackConfig
{
    public required CustomerId CustomerId { get; init; }
    public required string RedirectUrl { get; init; }
    public FakeAuditLogger? AuditLogger { get; init; }
}
