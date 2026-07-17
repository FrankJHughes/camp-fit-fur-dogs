using System.Reflection;
using Frank.Core.Application.Cqrs.Commands;
using Frank.Core.Application.Cqrs.Queries;
using Frank.Core.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Tests.Registration;

public class AutoRegister_ConcreteTypeTests
{
    private static readonly Assembly[] Assemblies =
    [
        // Scan all assemblies that contain Frank command handlers
        typeof(RegistrationAttribute).Assembly
    ];

    [Fact]
    public void Concrete_Types_Must_Be_Registered_When_Requested()
    {
        var services = new ServiceCollection();
        services.AddFrankCqrsCommands(Assemblies);
        services.AddFrankCqrsQueries(Assemblies);

        var descriptors = services.ToList();
        var offenders = new List<string>();

        //
        // STEP 1 — Find all attributed interfaces
        //
        var attributed =
            from asm in Assemblies
            from type in asm.DefinedTypes
            let attr = type.GetCustomAttribute<RegistrationAttribute>()
            where type.IsInterface && attr is not null && attr.RegisterConcreteType
            select type.AsType();

        //
        // STEP 2 — For each attributed interface, find concrete implementations
        //
        foreach (var openIface in attributed)
        {
            var closed =
                from asm in Assemblies
                from t in asm.DefinedTypes
                where t.IsClass && !t.IsAbstract
                from implIface in t.GetInterfaces() // IMPORTANT: inherited interfaces included
                where implIface.IsConstructedGenericType &&
                      // Compare generic type definitions by FullName — fixes load‑context mismatches
                      implIface.GetGenericTypeDefinition().FullName == openIface.FullName
                select (closedIface: implIface, implType: t.AsType());

            //
            // STEP 3 — Ensure concrete types are registered
            //
            foreach (var (closedIface, implType) in closed)
            {
                var hasConcrete = descriptors.Any(d => d.ServiceType == implType);

                if (!hasConcrete)
                    offenders.Add($"{closedIface.Name} -> missing concrete {implType.Name}");
            }
        }

        Assert.False(offenders.Count != 0,
            "Missing concrete registrations:\n" + string.Join("\n", offenders));
    }
}
