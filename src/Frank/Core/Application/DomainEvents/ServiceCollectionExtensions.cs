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

    public static IServiceCollection AddFrankCoreApplicationDomainEventDispatcher(this IServiceCollection services)
    {
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }

    public static IServiceCollection AddFrankCoreApplicationDomainEvents(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddFrankCoreApplicationDomainEventDispatcher();

        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

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
