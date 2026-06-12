using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.TestUtilities.Factories;

public static class CampFitFurDogsApiFactoryExtensions
{
    private const string FrontendConfigKey = "Frontend__BaseUrl";

    public static CampFitFurDogsApiFactory WithFrontendBaseUrl(
        this CampFitFurDogsApiFactory factory,
        string url)
    {
        factory.WithConfigOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [FrontendConfigKey] = url
            });
        });

        return factory;
    }

    public static CampFitFurDogsApiFactory WithConfigOverrides(
        this CampFitFurDogsApiFactory factory,
        Action<IConfigurationBuilder> configure)
    {
        factory.InternalConfigOverrides(configure);
        return factory;
    }

    public static CampFitFurDogsApiFactory WithEnvironment(
        this CampFitFurDogsApiFactory factory,
        string environment)
    {
        factory.SetEnvironment(environment);

        factory.WithConfigOverrides(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = environment
            });
        });

        return factory;
    }
}
