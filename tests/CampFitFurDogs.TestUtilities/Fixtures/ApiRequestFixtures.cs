using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class ApiRequestFixtures
{
    public static object Customer()
        => new CustomerBuilder().BuildApiRequest();

    public static object Dog()
        => new DogBuilder().BuildApiRequest();
}
