using CampFitFurDogs.TestUtilities.Builders;
using Frank.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.ApiRequests;

public static class ApiRequestFactory
{
    public static object Customer()
        => new UserBuilder().BuildApiRequest();

    public static object Dog()
        => new DogBuilder().BuildApiRequest();
}
