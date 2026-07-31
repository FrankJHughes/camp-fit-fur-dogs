using System.Reflection;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Exceptions;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankCoreInfrastructureExceptionHandlerRegistry(this IServiceCollection services)
    {
        return services
            .AddSingleton<ExceptionHandlerRegistry>()
            ;

    }

    public static IServiceCollection AddFrankCoreInfrastructureExceptionHandlers(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddFrankCoreInfrastructureExceptionHandlerRegistry();

        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be IExceptionHandler
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.AsType() == typeof(IExceptionHandler));

        //
        // Implementations: any class implementing IExceptionHandler
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i == typeof(IExceptionHandler)));

        //
        // Allow user overrides
        //
        configure?.Invoke(options);

        //
        // Unified discovery + registration
        //

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
