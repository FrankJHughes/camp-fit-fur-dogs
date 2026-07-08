using Frank.Domain.Users;

namespace Frank.TestUtilities.Fixtures;

public static class PhoneNumberFixtures
{
    public const string Valid = "916-555-1234";

    public static PhoneNumber From(string value)
        => PhoneNumber.From(value);
}
