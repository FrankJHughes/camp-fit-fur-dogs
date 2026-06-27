using System.Reflection;
using Frank.Abstractions.Event;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Event;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEvent(this IServiceCollection services)
    {
        return AddFrankEvent(services, []);
    }

    public static IServiceCollection AddFrankEvent(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        var includeInterfaceTypes = new Type[]
        {
            typeof(IEventHandler<>)
        };

        var excludeConcreteTypes = Array.Empty<Type>();

        Orchestrator.Orchestrate(
            services,
            includeInterfaceTypes,
            excludeConcreteTypes,
            assemblies);

        return services;
    }
}
