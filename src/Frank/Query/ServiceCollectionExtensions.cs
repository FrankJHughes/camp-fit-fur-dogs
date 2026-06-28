using System.Reflection;
using Frank.Abstractions.Query;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Query;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankQuery(this IServiceCollection services)
    {
        return AddFrankQuery(services, []);
    }

    public static IServiceCollection AddFrankQuery(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<RegistrationOptions>? configure = null)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        var includeInterfaceTypes = new Type[]
        {
            typeof(IQueryHandler<,>)
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
