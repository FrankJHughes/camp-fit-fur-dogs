using System.Reflection;
using Frank.Core.Application.Abstractions.DomainEvents;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Frank.Core.Application.DomainEvents;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
    iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankDomainEvents(this IServiceCollection services)
    {
        return AddFrankDomainEvents(services, []);
    }

    public static IServiceCollection AddFrankDomainEvents(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DiscoveryOptions>? configure = null)
    {
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be IDomainEventHandler<>
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            iface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

        //
        // Implementations: any class implementing IDomainEventHandler<>
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)));

        configure?.Invoke(options);

        Orchestrator.Orchestrate(
            services,
            [typeof(Frank.Core.Application.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
