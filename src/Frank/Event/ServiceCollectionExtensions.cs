using System.Reflection;
using Frank.Abstractions.Event;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Event;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankEvent(this IServiceCollection services)
    {
        return AddFrankEvent(services, []);
    }

    public static IServiceCollection AddFrankEvent(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be IEventHandler<>
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            iface.GetGenericTypeDefinition() == typeof(IEventHandler<>));

        //
        // Implementations: any class implementing IEventHandler<>
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEventHandler<>)));

        configure?.Invoke(options);

        Orchestrator.Orchestrate(
            services,
            [typeof(Frank.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
