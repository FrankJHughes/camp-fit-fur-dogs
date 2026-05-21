using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class CustomerFactory
{
    public static Customer Create()
        => new CustomerBuilder().Build();
}
