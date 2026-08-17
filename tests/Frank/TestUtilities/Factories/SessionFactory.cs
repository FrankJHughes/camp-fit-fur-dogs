using Frank.Identity.Domain.Sessions;
using Frank.Identity.Domain.Users;
using Frank.TestUtilities.Builders;

namespace Frank.TestUtilities.Factories;

public static class SessionFactory
{
    public static Session Create(UserId ownerId)
        => new SessionBuilder()
            .WithOwner(ownerId)
            .Build();
}
