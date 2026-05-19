using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class PhoneNumberFactory
{
    public static PhoneNumber Valid(string digits = "916-555-1234")
        => new PhoneNumberBuilder().WithDigits(digits).Build();

    public static PhoneNumber LocalSevenDigit(string digits = "555-1234")
        => new PhoneNumberBuilder().WithDigits(digits).Build();
}
