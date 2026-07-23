using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Frank.Identity.Application.Abstractions.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Users;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplicationUsers(this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeInterfaces(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddScoped<IUserResolver, UserResolver>()

            .AddFrankCoreApplicationCqrsCommands([
                typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions))

            .AddFrankCoreApplicationCqrsQueries([
                typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions));
    }
}
