using System.Reflection;
using Frank.Registration;

namespace Frank.Tests.DependencyInjection;

public class AutoRegister_ConcreteTypeTests
{
    private static readonly Assembly[] Assemblies =
    [
        typeof(RegistrationAttribute).Assembly
    ];

    [Fact]
    public void Concrete_Types_Must_Be_Registered_When_Requested()
    {
        var services = new ServiceCollection();
        services.AddFrank(Assemblies);

        var descriptors = services.ToList();
        var offenders = new List<string>();

        var attributed =
            from asm in Assemblies
            from type in asm.DefinedTypes
            let attr = type.GetCustomAttribute<RegistrationAttribute>()
            where type.IsInterface && attr is not null && attr.RegisterConcreteType
            select type.AsType();

        foreach (var openIface in attributed)
        {
            var closed =
                from asm in Assemblies
                from t in asm.DefinedTypes
                where t.IsClass && !t.IsAbstract
                from implIface in t.ImplementedInterfaces
                where implIface.IsConstructedGenericType &&
                      implIface.GetGenericTypeDefinition() == openIface
                select (closedIface: implIface, implType: t.AsType());

            foreach (var (closedIface, implType) in closed)
            {
                var hasConcrete = descriptors.Any(d => d.ServiceType == implType);

                if (!hasConcrete)
                    offenders.Add($"{closedIface.Name} -> missing concrete {implType.Name}");
            }
        }

        Assert.False(offenders.Any(),
            "Missing concrete registrations:\n" + string.Join("\n", offenders));
    }
}
