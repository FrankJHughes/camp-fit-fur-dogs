using System.Reflection;

namespace Frank.Core.Application.Registration;

public sealed class DiscoveryOptions
{
    internal List<Func<TypeInfo, bool>> InterfaceInclusionPredicates { get; } = [];
    internal List<Func<TypeInfo, bool>> ImplementationInclusionPredicates { get; } = [];

    public DiscoveryOptions IncludeInterfaces(Func<TypeInfo, bool> predicate)
    {
        InterfaceInclusionPredicates.Add(predicate);
        return this;
    }

    public DiscoveryOptions IncludeImplementations(Func<TypeInfo, bool> predicate)
    {
        ImplementationInclusionPredicates.Add(predicate);
        return this;
    }

    internal bool ShouldIncludeInterface(TypeInfo iface)
        => InterfaceInclusionPredicates.Any(p => p(iface));

    internal bool ShouldIncludeImplementation(TypeInfo impl) =>
        ImplementationInclusionPredicates.Count > 0 &&
        ImplementationInclusionPredicates.All(p => p(impl));
}
