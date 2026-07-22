using System.Reflection;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Exceptions;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankInfrastructureExceptions(this IServiceCollection services)
    {
        return AddFrankInfrastructureExceptions(services, Array.Empty<Assembly>());
    }

    public static IServiceCollection AddFrankInfrastructureExceptions(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddSingleton<ExceptionHandlerRegistry>();

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
        Orchestrator.Orchestrate(
            services,
            [typeof(Frank.Core.Application.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
