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

    public static IServiceCollection AddFrankQuery(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        var includeInterfaceTypes = new Type[]
        {
            typeof(IQueryHandler<,>)
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
