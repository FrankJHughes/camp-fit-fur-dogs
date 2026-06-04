
using System.Reflection;
using Frank.DependencyInjection.AutoRegistration.Shapes;

namespace Frank.DependencyInjection.AutoRegistration;

public static class Scanner
{
    private static readonly Type AutoRegisterAttributeType = typeof(AutoRegisterAttribute);

    public static IEnumerable<RelevantInterfaceGroup> Scan(Assembly[] assemblies) => GetRelevantInterfaces(assemblies)
        .LeftJoin(
            GetConcreteImplementations(assemblies), // right side
            GetRelevantInterfaceKey, // key of left side
            GetImplementedInterfaceKey, // key of right side
            (relevantInterface, implementation) =>
                (RelevantInterface: relevantInterface, Implementation: implementation)) // shape of output row
        .GroupBy(
            row => row.RelevantInterface, // key source
            row => row.Implementation, // value source
            (relevantInterface, implementations) =>
                new RelevantInterfaceGroup(relevantInterface, RemoveNullEntries(implementations))); // key value tuple

    private static Type GetImplementedInterfaceKey(Implementation implementation)
    {
        return GetComparisonKey(implementation.ImplementedInterface);
    }

    private static Type GetRelevantInterfaceKey(TypeInfo relevantInterface)
    {
        return GetComparisonKey(relevantInterface.AsType());
    }

    private static IEnumerable<Implementation> RemoveNullEntries(IEnumerable<Implementation?> implementations)
    {
        return implementations
            .Where(implementation => implementation is not null).Select(implementation => implementation!);
    }

    private static Type GetComparisonKey(Type interfaceType)
    {
        return interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
    }

    private static IEnumerable<TypeInfo> GetRelevantInterfaces(Assembly[] assemblies) =>
        assemblies
        .SelectMany(assembly => assembly.DefinedTypes)
        .Where(t => IsRelevantInterface(t))
        .Distinct(); // <= IMPORTANT

    private static bool IsRelevantInterface(TypeInfo t)
    {
        return t.IsInterface &&
            t.CustomAttributes.Any(ca =>
                ca.AttributeType == AutoRegisterAttributeType);
    }

    private static IEnumerable<Implementation> GetConcreteImplementations(Assembly[] assemblies)
    {
        return assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(ct => IsConcreteClassType(ct))
            .Distinct()
            .SelectMany(ct =>
                ct.ImplementedInterfaces,
                (ct, iit) => new Implementation(ct, iit));
    }

    private static bool IsConcreteClassType(TypeInfo ct)
    {
        return ct.IsClass &&
            !ct.IsAbstract &&
            !ct.ContainsGenericParameters;
    }

}
