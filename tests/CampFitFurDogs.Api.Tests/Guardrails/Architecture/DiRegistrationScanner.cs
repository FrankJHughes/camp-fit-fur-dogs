using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails.Architecture;

public static class DiRegistrationScanner
{
    public static IEnumerable<(Type Type, Type? Interface)> FindTypesWithInterfaces(
        Assembly assembly,
        Func<Type, bool> predicate)
    {
        return assembly
            .GetTypes()
            .Where(predicate)
            .Select(t => (Type: t, Interface: t.GetInterfaces().FirstOrDefault()))
            .Where(x => x.Interface != null);
    }

    public static IEnumerable<object?> GetRegistrations(IServiceProvider provider, Type iface)
    {
        using var scope = provider.CreateScope();
        return scope.ServiceProvider.GetServices(iface);
    }
}
