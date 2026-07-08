using Frank.Domain.Users;
using Frank.TestUtilities.Builders;

namespace Frank.TestUtilities.Factories;

public static class PhoneNumberFactory
{
    public static PhoneNumber Valid(string digits = "916-555-1234")
        => new PhoneNumberBuilder().WithDigits(digits).Build();

    public static PhoneNumber LocalSevenDigit(string digits = "555-1234")
        => new PhoneNumberBuilder().WithDigits(digits).Build();
}
