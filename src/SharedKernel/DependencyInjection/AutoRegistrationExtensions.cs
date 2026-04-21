using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public static class AutoRegistrationExtensions
{
    public static IServiceCollection AddAutoRegistration(
        this IServiceCollection services,
        Assembly assembly,
        IEnumerable<AutoRegistrationRule> rules)
    {
        foreach (var rule in rules)
        {
            var types = assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.Name.EndsWith(rule.Suffix))
                .ToList();

            foreach (var type in types)
            {
                var iface = type.GetInterfaces().FirstOrDefault();
                if (iface is null) continue;

                services.Add(new ServiceDescriptor(iface, type, rule.Lifetime));
            }
        }

        return services;
    }
}
