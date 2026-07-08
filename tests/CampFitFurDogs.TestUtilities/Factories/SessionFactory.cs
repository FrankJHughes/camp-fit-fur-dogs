using CampFitFurDogs.Domain.Sessions;
using Frank.Domain.Users;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class SessionFactory
{
    public static Session Create(UserId ownerId)
        => new SessionBuilder()
            .WithOwner(ownerId)
            .Build();
}
