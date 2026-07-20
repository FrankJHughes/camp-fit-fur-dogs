using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Frank.Identity.Application.Abstractions.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Sessions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplicationSessions(this IServiceCollection services)
    {
        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeInterfaces(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services

            .AddScoped<ISessionTokenGenerator, SessionTokenGenerator>()

            .AddFrankCqrsCommands([
                typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions))

            .AddFrankCqrsQueries([
                typeof(AssemblyMarker).Assembly],
                discoveryOptions => updateOptions(discoveryOptions));
    }
}
