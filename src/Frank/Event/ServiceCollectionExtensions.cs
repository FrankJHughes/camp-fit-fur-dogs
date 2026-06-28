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

    public static IServiceCollection AddFrankEvent(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<RegistrationOptions>? configure = null)
    {
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        var includeInterfaceTypes = new Type[]
        {
            typeof(IEventHandler<>)
        };

        var registrationOptions = new RegistrationOptions();
        configure?.Invoke(registrationOptions);

        Orchestrator.Orchestrate(
            services,
            assemblies,
            includeInterfaceTypes,
            registrationOptions);

        return services;
    }
}
