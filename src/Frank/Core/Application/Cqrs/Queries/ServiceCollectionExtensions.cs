using System.Reflection;
using Frank.Core.Application.Abstractions.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Frank.Core.Application.Cqrs.Queries;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
        iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankCoreApplicationCqrsQueryDispatcher(this IServiceCollection services)
    {
        services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();
        return services;
    }

    public static IServiceCollection AddFrankCoreApplicationCqrsQueries(
        this IServiceCollection services,
        IEnumerable<Assembly>? assembliesToSearch = null,
        Action<DiscoveryOptions>? configure = null)
    {
        _ = services.AddFrankCoreApplicationCqrsQueryDispatcher();

        if (assembliesToSearch is null || !assembliesToSearch.Any())
        {
            return services;
        }

        var options = new DiscoveryOptions();

        //
        // Interface must:
        //   - be IQueryHandler<TQuery, TResponse>
        //   - AND be decorated with [Registration]
        //
        options.IncludeInterfaces(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

        //
        // Implementations: any class implementing IQueryHandler<TQuery, TResponse>
        //
        options.IncludeImplementations(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

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
