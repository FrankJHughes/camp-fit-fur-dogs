using System.Reflection;
using Frank.Registration.Shapes;

namespace Frank.Registration;

public static class Scanner
{
    public static IEnumerable<RelevantInterfaceGroup> Scan(
        IEnumerable<Assembly> assemblies,
        DiscoveryOptions options)
    {
        // Phase 1: discover all interfaces in the scan boundary
        var interfaces = assemblies
            .Distinct()
            .SelectMany(a => a.DefinedTypes)
            .Where(t => t.IsInterface)
            .Where(options.ShouldIncludeInterface)
            .ToList();

        // Phase 2: discover all concrete implementations
        var implementations = assemblies
            .Distinct()
            .SelectMany(a => a.DefinedTypes)
            .Where(IsConcreteClassType)
            .Where(options.ShouldIncludeImplementation)
            .SelectMany(
                ct => ct.ImplementedInterfaces,
                (ct, iface) => new Implementation(ct, iface))
            .ToList();

        // Phase 3: left-join interfaces → implementations
        return interfaces
            .LeftJoin(
                implementations,
                iface => GetComparisonKey(iface.AsType()),
                impl => GetComparisonKey(impl.ImplementedInterface),
                (iface, impl) => (iface, impl))
            .GroupBy(
                row => row.iface,
                row => row.impl,
                (iface, impls) =>
                    new RelevantInterfaceGroup(
                        iface,
                        impls.Where(i => i != null)!));
    }

    private static bool IsConcreteClassType(TypeInfo t)
        => t.IsClass &&
           !t.IsAbstract &&
           !t.ContainsGenericParameters;

    private static Type GetComparisonKey(Type type)
        => type.IsGenericType
            ? type.GetGenericTypeDefinition()
            : type;
}
