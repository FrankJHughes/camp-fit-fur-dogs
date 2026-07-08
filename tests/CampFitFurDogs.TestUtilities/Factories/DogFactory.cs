using Frank.Domain.Users;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class DogFactory
{
    public static Dog Create(UserId ownerId)
        => new DogBuilder().WithOwner(ownerId).Build();
}
