using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Dogs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDogs(
        this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) =>
            options.IncludeInterfaces(t =>
                !string.IsNullOrWhiteSpace(t.Namespace) &&
                t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services

            .AddFrankCqrsCommands([
                typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions))

            .AddFrankCqrsQueries([
                typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions));

    }
}


