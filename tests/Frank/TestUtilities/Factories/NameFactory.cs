using Frank.Identity.Domain.Users;
using Frank.TestUtilities.Builders;

namespace Frank.TestUtilities.Factories;

public static class NameFactory
{
    public static FirstName First(string value = "Frank")
        => new FirstNameBuilder().WithValue(value).Build();

    public static LastName Last(string value = "Hughes")
        => new LastNameBuilder().WithValue(value).Build();
}
