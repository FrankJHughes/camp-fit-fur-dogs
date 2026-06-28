
using System.Reflection;
using Frank.Registration.Shapes;

namespace Frank.Registration;

public static class Scanner
{
    private static readonly Type RegistrationAttributeType = typeof(RegistrationAttribute);

    public static IEnumerable<RelevantInterfaceGroup> Scan(
        IEnumerable<Assembly> assemblies,
        IEnumerable<Type> includeInterfaceTypes,
        RegistrationOptions registrationOptions) =>
        GetRelevantInterfaces(includeInterfaceTypes, assemblies)
        .LeftJoin(
            GetConcreteImplementations(assemblies, registrationOptions), // right side
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

    private static IEnumerable<TypeInfo> GetRelevantInterfaces(
        IEnumerable<Type> includeInterfaceTypes,
        IEnumerable<Assembly> assemblies)
    {
        return includeInterfaceTypes
            .Select(t => t.GetTypeInfo())
            .Where(IsRelevantInterface)
            .Distinct();
    }
    private static bool Contains(IEnumerable<Type> types, TypeInfo t)
    {
        var type = t.AsType();

        return types.Any(t => t == type);
    }

    private static bool IsRelevantInterface(TypeInfo t)
    {
        return t.IsInterface &&
            t.CustomAttributes.Any(ca =>
                ca.AttributeType == RegistrationAttributeType);
    }

    private static IEnumerable<Implementation> GetConcreteImplementations(
        IEnumerable<Assembly> assemblies,
        RegistrationOptions options)
    {
        return assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(IsConcreteClassType)
            .Where(ct => ConcreteTypeNotExcluded(options, ct))
            .Distinct()
            .SelectMany(ct =>
                ct.ImplementedInterfaces,
                (ct, iit) => new Implementation(ct, iit));
    }

    private static bool ConcreteTypeNotExcluded(RegistrationOptions options, TypeInfo ct)
    {
        return !options.ExcludedTypes.Contains(ct)
            && !options.ExclusionPredicates.Any(p => p(ct));
    }

    private static bool IsConcreteClassType(TypeInfo ct)
    {
        return ct.IsClass &&
            !ct.IsAbstract &&
            !ct.ContainsGenericParameters;
    }

}
