
using CampFitFurDogs.TestUtilities.Factories;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public class ApiFactoryFixture : IDisposable
{
    public CampFitFurDogsApiFactory Factory { get; }

    public ApiFactoryFixture()
    {
        Factory = new CampFitFurDogsApiFactory()
            .WithFrontendBaseUrl("https://camp-fit-fur-dogs.vercel.app")
            .WithEnvironment("Development");
    }

    public void Dispose()
    {
        Factory.Dispose();
    }
}
