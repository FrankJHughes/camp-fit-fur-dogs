using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Tests.DependencyInjection;

public class AutoRegister_RegistrationTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(AutoRegisterAttribute).Assembly
    ];

    [Fact]
    public void All_AutoRegistered_Interfaces_Must_Have_Closed_Registrations()
    {
        var services = new ServiceCollection();
        services.AddSharedKernel(Assemblies);

        var provider = services.BuildServiceProvider();

        var attributed =
            from asm in Assemblies
            from type in asm.DefinedTypes
            let attr = type.GetCustomAttribute<AutoRegisterAttribute>()
            where type.IsInterface && attr is not null
            select type.AsType();

        var offenders = new List<string>();

        foreach (var openIface in attributed)
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
                if (provider.GetService(closedIface) is null)
                    offenders.Add(closedIface.FullName!);
            }
        }

        Assert.False(offenders.Any(),
            "Missing registrations:\n" + string.Join("\n", offenders));
    }
}
