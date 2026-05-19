using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class PasswordFixtures
{
    public const string Plain = "SuperSecure123!";

    public static PasswordHash Hash(string? plaintext = null)
        => PasswordHash.Create(plaintext ?? Plain);
}
