using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Tests.DependencyInjection;

public class AutoRegister_MinMaxTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(AutoRegisterAttribute).Assembly
    ];

    [Fact]
    public void AutoRegister_MinMax_Rules_Must_Be_Enforced()
    {
        var services = new ServiceCollection();
        services.AddSharedKernel(Assemblies);

        var provider = services.BuildServiceProvider();

        var attributed =
            from asm in Assemblies
            from type in asm.DefinedTypes
            let attr = type.GetCustomAttribute<AutoRegisterAttribute>()
            where type.IsInterface && attr is not null
            select (openIface: type.AsType(), attr);

        var offenders = new List<string>();

        foreach (var (openIface, attr) in attributed)
        {
            var closed =
                from asm in Assemblies
                from t in asm.DefinedTypes
                where t.IsClass && !t.IsAbstract
                from implIface in t.ImplementedInterfaces
                where implIface.IsConstructedGenericType &&
                      implIface.GetGenericTypeDefinition() == openIface
                select implIface;

            foreach (var closedIface in closed)
            {
                var implTypes =
                    services
                        .Where(d => d.ServiceType == closedIface)
                        .Select(d => d.ImplementationType)
                        .Where(t => t is not null)
                        .Distinct()
                        .ToList();

                var count = implTypes.Count;

                if (count < attr.MinRegistrationCount || count > attr.MaxRegistrationCount)
                {
                    offenders.Add(
                        $"{closedIface.Name}: found {count}, expected {attr.MinRegistrationCount}-{attr.MaxRegistrationCount}");
                }
            }
        }

        Assert.False(offenders.Any(),
            "Min/Max violations:\n" + string.Join("\n", offenders));
    }
}
