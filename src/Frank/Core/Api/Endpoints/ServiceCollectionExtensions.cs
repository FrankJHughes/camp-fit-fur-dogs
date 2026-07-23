using System.Reflection;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Endpoints;

public static class EndpointServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApiEndpoints(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be IEndpoint
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            iface.AsType() == typeof(IEndpoint) &&
            iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0);

        //
        // Implementations: any class implementing IEndpoint
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i => i == typeof(IEndpoint)));

        configure?.Invoke(options);

        IEnumerable<Assembly> assemblies = [
                typeof(Frank.Core.Application.AssemblyMarker).Assembly,
                .. assembliesToSearch
        ];

        Orchestrator.Orchestrate(
            services,
            assemblies,
            options);

        return services;
    }
}
