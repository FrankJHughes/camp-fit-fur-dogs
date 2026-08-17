using Frank.Identity.Domain.Users;
using Frank.TestUtilities.Builders;

namespace Frank.TestUtilities.Factories;

public static class EmailFactory
{
    public static Email Unique(string prefix = "test")
        => new EmailBuilder().WithValue($"{prefix}-{Guid.NewGuid()}@example.com").Build();

    public static Email From(string value)
        => new EmailBuilder().WithValue(value).Build();
}
