using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class PasswordFactory
{
    public static PasswordHash Hash(string plaintext = "SuperSecure123!")
        => new PasswordBuilder().WithPlaintext(plaintext).Build();

    public static string Plain(string plaintext = "SuperSecure123!")
        => plaintext;
}
