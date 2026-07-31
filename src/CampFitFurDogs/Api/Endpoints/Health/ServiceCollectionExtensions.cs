
using Frank.Core.Api.Endpoints;
using Frank.Core.Application.Registration;

namespace CampFitFurDogs.Api.Endpoints.Health;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampFitFurDogsApiEndpointsHealth(
        this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeImplementations(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddFrankCoreApiEndpoints([
                typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly],
                options => updateOptions(options)
            );
    }
}
