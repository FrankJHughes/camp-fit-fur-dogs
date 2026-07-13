using System.Reflection;
using Frank.Core.Application.Abstractions.Endpoints;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Endpoints;

public static class EndpointServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEndpoints(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DiscoveryOptions>? configure = null)
    {
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

        Orchestrator.Orchestrate(
            services,
            [
                typeof(Frank.Core.Application.AssemblyMarker).Assembly,
                .. assemblies
            ],
            options);

        return services;
    }
}
