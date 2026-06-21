using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class SessionFixtures
{
    // A valid 64‑char SHA‑256 hex string
    public const string DefaultTokenHash =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    public static readonly SessionTokenHash TokenHash =
        SessionTokenHash.From(DefaultTokenHash);

    public static readonly CustomerId OwnerId =
        CustomerId.From(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

    public static readonly DateTimeOffset CreatedAt =
        new(2026, 5, 1, 12, 0, 0, TimeSpan.Zero);
}
