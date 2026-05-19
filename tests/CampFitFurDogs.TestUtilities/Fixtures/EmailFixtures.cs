using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class EmailFixtures
{
    public const string Default = "test@example.com";

    public static Email Unique(string prefix = "test")
        => Email.From($"{prefix}-{Guid.NewGuid()}@example.com");

    public static Email From(string value)
        => Email.From(value);
}
