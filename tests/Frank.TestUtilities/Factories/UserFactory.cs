using Frank.Domain.Users;
using Frank.TestUtilities.Builders;

namespace Frank.TestUtilities.Factories;

public static class UserFactory
{
    public static User Create()
        => new UserBuilder().Build();
}
