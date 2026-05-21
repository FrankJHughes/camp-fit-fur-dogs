using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class NameFactory
{
    public static FirstName First(string value = "Frank")
        => new FirstNameBuilder().WithValue(value).Build();

    public static LastName Last(string value = "Hughes")
        => new LastNameBuilder().WithValue(value).Build();
}
