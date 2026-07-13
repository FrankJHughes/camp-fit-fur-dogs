using System.Reflection;
using Frank.Core.Application.Abstractions.Query;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Query;

public static class ServiceCollectionExtensions
{
    private static bool HasRegistrationAttribute(TypeInfo iface) =>
        iface.GetCustomAttributes(typeof(RegistrationAttribute), inherit: true).Length != 0;

    public static IServiceCollection AddFrankQuery(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<DiscoveryOptions>? configure = null)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

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

        Orchestrator.Orchestrate(
            services,
            [typeof(Frank.Core.Application.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
