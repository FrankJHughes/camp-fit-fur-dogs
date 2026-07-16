using CampFitFurDogs.TestUtilities.Builders;
using Frank.TestUtilities.Builders;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public static class ApiRequestFixtures
{
    public static object User()
        => new UserBuilder().BuildApiRequest();

    public static object Dog()
        => new DogBuilder().BuildApiRequest();
}
