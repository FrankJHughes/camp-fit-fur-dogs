using System.Reflection;
using Frank.Abstractions.Exceptions;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.Exceptions;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankException(this IServiceCollection services)
    {
        return AddFrankException(services, Array.Empty<Assembly>());
    }

    public static IServiceCollection AddFrankException(
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
        options.IncludeInterface(iface =>
            HasRegistrationAttribute(iface) &&
            iface.AsType() == typeof(IExceptionHandler));

        //
        // Implementations: any class implementing IExceptionHandler
        //
        options.IncludeImplementation(impl =>
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
            [typeof(Frank.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
