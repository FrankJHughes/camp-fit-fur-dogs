using CampFitFurDogs.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.ApiRequests;

public static class ApiRequestFactory
{
    public static object Customer()
        => new CustomerBuilder().BuildApiRequest();

    public static object Dog()
        => new DogBuilder().BuildApiRequest();
}
