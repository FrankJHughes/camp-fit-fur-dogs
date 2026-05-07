using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;


namespace SharedKernel.DependencyInjection;

public static class AutoRegistrationExtensions
{
    private static readonly Type AutoRegisterAttributeType = typeof(AutoRegisterAttribute);
    public static IServiceCollection AddAutoRegistration(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var registrations = new StringBuilder();
        var violations = new StringBuilder();
        var violationCount = 0;
        foreach (var (relevantInterface, relevantInterfaceGroups) in GetRelevantInterfaceGroups(assemblies))
        {
            var autoRegisterAttribute = relevantInterface.GetCustomAttribute<AutoRegisterAttribute>();
            var minCount = autoRegisterAttribute!.MinRegistrationCount;
            var maxCount = autoRegisterAttribute!.MaxRegistrationCount;

            foreach (var (implementedInterface, implementations) in GetImplementedInterfaceGroups(relevantInterfaceGroups))
            {
                var count = implementations.Count();
                var isViolation = count < minCount || count > maxCount;
                if (!isViolation)
                {
                    if (violationCount > 0)
                    {
                        continue;
                    }
                    Register(services, implementedInterface, implementations, autoRegisterAttribute);
                    Report(registrations, implementedInterface, implementations, minCount, maxCount, count);
                    continue;
                }
                violationCount += 1;
                Report(violations, implementedInterface, implementations, minCount, maxCount, count);
            }
        }

        if (violationCount > 0)
        {
            throw new InvalidOperationException(violations.ToString());
        }

        return services;
    }

    private static IEnumerable<(Type closedInterface, IEnumerable<TypeInfo> closedInterfaceGroup)> GetImplementedInterfaceGroups(IEnumerable<(TypeInfo ct, Type iit)> openInterfaceGroup)
    {
        return openInterfaceGroup
            .GroupBy(
                open => open.iit,
                open => open.ct,
                (closedInterface, closedImplementation) =>
                    (closedInterface, closedImplementation));
    }

    private static IEnumerable<(TypeInfo rit, IEnumerable<(TypeInfo ct, Type iit)> ctxiitEnum)> GetRelevantInterfaceGroups(Assembly[] assemblies)
    {
        return GetRelevantInterfaceTypeInfoEnum(assemblies)
            .LeftJoin(
                GetAllConcreteClassTypes(assemblies), // right side
                rit => GetInterfaceTypeForCompare(rit.AsType()), // key of left side
                ctxiit => GetInterfaceTypeForCompare(ctxiit.iit), // key of right side
                (rit, ctxiit) => (rit, ctxiit)) // shape of output row
            .GroupBy(
                row => row.rit, // shape of output key
                row => row.ctxiit, // shape of output group
                (rit, ctxiitEnum) => (rit, ctxiitEnum.Where(ctxiit => ctxiit.ct is not null)));

    }

    private static Type GetInterfaceTypeForCompare(Type iit)
    {
        return iit.IsGenericType ? iit.GetGenericTypeDefinition() : iit;
    }

    private static void Report(StringBuilder sb, Type implementedInterface, IEnumerable<TypeInfo> implementations, int minCount, int maxCount, int count)
    {
        var implementedInterfaceDeclaration = GetInterfaceDeclaration(implementedInterface);
        sb.AppendLine($"{implementedInterfaceDeclaration}");
        sb.AppendLine($"requires between {minCount} and {maxCount} implementations. It has {count}:");

        if (!implementations.Any())
        {
            return;
        }

        sb.AppendJoin("\n",
            implementations.Select(implementation => $"\t{implementation.Name} : {GetInterfaceDeclaration(implementedInterface)}"));
        sb.AppendLine();
    }

    private static void Register(IServiceCollection services, Type implementedInterface, IEnumerable<Type> implementations, AutoRegisterAttribute autoRegisterAttribute)
    {
        if (!implementations.Any())
        {
            return;
        }

        foreach (var implementation in implementations)
        {
            Register(services, implementedInterface, autoRegisterAttribute, implementation);
        }
    }

    private static void Register(IServiceCollection services, Type implementedInterface, AutoRegisterAttribute autoRegisterAttribute, Type implementation)
    {
        services.Add(new ServiceDescriptor(implementedInterface, implementation, autoRegisterAttribute.Lifetime));

        if (!autoRegisterAttribute.RegisterConcreteType)
        {
            return;
        }
        services.Add(new ServiceDescriptor(implementation, implementation, autoRegisterAttribute.Lifetime));
    }

    private static IEnumerable<TypeInfo> GetRelevantInterfaceTypeInfoEnum(Assembly[] assemblies) =>
        assemblies
        .SelectMany(assembly => assembly.DefinedTypes)
        .Where(t =>
            IsRelevantInterfaceType(t))
        .Distinct();

    private static bool IsRelevantInterfaceType(TypeInfo t)
    {
        return t.IsInterface &&
            t.CustomAttributes.Any(ca =>
                ca.AttributeType == AutoRegisterAttributeType);
    }

    private static IEnumerable<(TypeInfo ct, Type iit)> GetAllConcreteClassTypes(Assembly[] assemblies)
    {
        return assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(ct => IsConcreteClassType(ct))
            .Distinct()
            .SelectMany(ct =>
                ct.ImplementedInterfaces,
                (ct, iit) => (ct, iit));
    }

    private static bool IsConcreteClassType(TypeInfo ct)
    {
        return ct.IsClass &&
            !ct.IsAbstract &&
            !ct.ContainsGenericParameters;
    }

    static string GetInterfaceDeclaration(Type type)
    {
        if (!type.IsInterface)
            throw new ArgumentException("Type must be an interface.", nameof(type));

        string name = type.Name;
        if (type.IsGenericType)
        {
            // Remove the `N suffix from generic type names
            name = name[..name.IndexOf('`')];

            var args = type.GetGenericArguments()
                           .Select(t => t.Name);

            return $"{name}<{string.Join(", ", args)}>";
        }
        else
        {
            return name;
        }
    }
}
