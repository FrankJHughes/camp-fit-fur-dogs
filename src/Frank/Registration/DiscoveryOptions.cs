using System.Reflection;

namespace Frank.Registration;

public sealed class DiscoveryOptions
{
    internal List<Func<TypeInfo, bool>> InterfaceInclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> InterfaceExclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> ImplementationInclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> ImplementationExclusionPredicates { get; } = [];

    public DiscoveryOptions IncludeInterfaces(Func<TypeInfo, bool> predicate)
    {
        InterfaceInclusionPredicates.Add(predicate);
        return this;
    }

    public DiscoveryOptions ExcludeInterfaces(Func<TypeInfo, bool> predicate)
    {
        InterfaceExclusionPredicates.Add(predicate);
        return this;
    }

    public DiscoveryOptions IncludeImplementations(Func<TypeInfo, bool> predicate)
    {
        ImplementationInclusionPredicates.Add(predicate);
        return this;
    }

    public DiscoveryOptions ExcludeImplementations(Func<TypeInfo, bool> predicate)
    {
        ImplementationExclusionPredicates.Add(predicate);
        return this;
    }

    internal bool ShouldIncludeInterface(TypeInfo iface)
        => InterfaceInclusionPredicates.Any(p => p(iface))
           && !InterfaceExclusionPredicates.Any(p => p(iface));

    internal bool ShouldIncludeImplementation(TypeInfo impl)
        => ImplementationInclusionPredicates.Any(p => p(impl))
           && !ImplementationExclusionPredicates.Any(p => p(impl));
}
