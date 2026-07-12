using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Builders;
using Frank.Domain.Users;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class DogFactory
{
    public static Dog Create(UserId ownerId)
        => new DogBuilder().WithOwner(ownerId).Build();
}
