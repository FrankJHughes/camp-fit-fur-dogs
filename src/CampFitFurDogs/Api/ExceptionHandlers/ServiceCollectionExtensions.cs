using Frank.Core.Application.Registration;
using Frank.Core.Infrastructure.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCampFitFurDogsApiExceptionHandlers(
        this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeInterfaces(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddFrankCoreInfrastructureExceptionHandlers([
                typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly],
                options => updateOptions(options)
            );
    }
}
