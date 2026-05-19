using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Domain.Dogs;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class DogFactory
{
    public static Dog Create(CustomerId ownerId)
        => new DogBuilder().WithOwner(ownerId).Build();
}
