using CampFitFurDogs.Domain.Authentication.Sessions;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class SessionFactory
{
    public static Session Create(CustomerId ownerId)
        => new SessionBuilder()
            .WithOwner(ownerId)
            .Build();
}
