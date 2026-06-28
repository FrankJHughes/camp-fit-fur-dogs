using System.Reflection;
using Frank.Abstractions.Query;
using Frank.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Query;

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
        options.IncludeInterface(iface =>
            HasRegistrationAttribute(iface) &&
            iface.IsGenericType &&
            iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

        //
        // Implementations: any class implementing IQueryHandler<TQuery, TResponse>
        //
        options.IncludeImplementation(impl =>
            impl.ImplementedInterfaces.Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

        configure?.Invoke(options);

        Orchestrator.Orchestrate(
            services,
            [typeof(Frank.AssemblyMarker).Assembly, .. assemblies],
            options);

        return services;
    }
}
