using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public static class AutoRegistrationExtensions
{
    public static IServiceCollection AddAutoRegistration(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var contracts = typeof(AutoRegisterAttribute).Assembly
            .GetTypes()
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<AutoRegisterAttribute>()))
            .Where(x => x.Attr is not null)
            .ToList();

        var registrations = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(impl => impl.GetInterfaces(),
                (impl, iface) => (impl, iface))
            .Select(x => (
                x.impl,
                x.iface,
                contract: contracts.FirstOrDefault(c => Matches(x.iface, c.Type))))
            .Where(x => x.contract.Attr is not null);

        foreach (var (impl, iface, contract) in registrations)
            services.Add(new ServiceDescriptor(iface, impl, contract.Attr!.Lifetime));

        return services;
    }

    private static bool Matches(Type candidate, Type contract)
        => contract.IsGenericTypeDefinition
            ? candidate.IsGenericType
              && candidate.GetGenericTypeDefinition() == contract
            : candidate == contract;
}
