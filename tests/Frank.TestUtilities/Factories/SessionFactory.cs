using Frank.Domain.Sessions;
using Frank.Domain.Users;
using Frank.TestUtilities.Builders;

namespace Frank.TestUtilities.Factories;

public static class SessionFactory
{
    public static Session Create(UserId ownerId)
        => new SessionBuilder()
            .WithOwner(ownerId)
            .Build();
}
