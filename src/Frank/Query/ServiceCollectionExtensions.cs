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
        var types = new Type[]
        {
            typeof(IQueryHandler<,>)
        };

        Orchestrator.Orchestrate(services, types, assemblies);

        return services;
    }
}
