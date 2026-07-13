using System;
using Frank.Identity.Domain.Users;

namespace Frank.TestUtilities.Fixtures;

public static class EmailFixtures
{
    public const string Default = "test@example.com";

    public static string Random(string prefix = "test")
        => $"{prefix}-{Guid.NewGuid()}@example.com";

    public static Email Unique(string prefix = "test")
        => Email.From($"{prefix}-{Guid.NewGuid()}@example.com");

    public static Email From(string value)
        => Email.From(value);
}
