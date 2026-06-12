using CampFitFurDogs.TestUtilities.Factories;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public class ApiFactoryFixture : IDisposable
{
    public CampFitFurDogsApiFactory Factory { get; }

    public ApiFactoryFixture()
    {
        Factory = new CampFitFurDogsApiFactory()
            .WithFrontendBaseUrl("https://camp-fit-fur-dogs.vercel.app");
    }

    public void Dispose()
    {
        Factory.Dispose();
    }
}
