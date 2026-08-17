using System.Reflection;
using Frank.Core.Application.Registration.Shapes;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Performs assembly scanning to discover relevant interfaces and their
/// concrete implementations based on the configured <see cref="DiscoveryOptions"/>.
///
/// <para>
/// The scanner is the first stage of the registration pipeline. It identifies:
/// </para>
/// <list type="bullet">
///   <item><b>Relevant interfaces</b> — interfaces that match the inclusion predicates</item>
///   <item><b>Concrete implementations</b> — non‑abstract classes that match the inclusion predicates</item>
///   <item><b>Implemented interface relationships</b> — mapping classes to the interfaces they implement</item>
/// </list>
///
/// <para>
/// The output of the scanner is a collection of <see cref="RelevantInterfaceGroup"/>
/// objects, each containing:
/// </para>
/// <list type="bullet">
///   <item>The relevant interface</item>
///   <item>The implementations discovered for that interface</item>
/// </list>
///
/// <para>
/// These groups are later transformed into registration plans by <see cref="Planner"/>.
/// </para>
/// </summary>
public static class Scanner
{
    /// <summary>
    /// Scans the provided assemblies to discover interfaces and implementations
    /// that satisfy the configured <see cref="DiscoveryOptions"/>.
    ///
    /// <para>
    /// The scan proceeds in three phases:
    /// </para>
    /// <list type="number">
    ///   <item><b>Discover interfaces</b> — all interfaces matching inclusion predicates</item>
    ///   <item><b>Discover implementations</b> — all concrete classes matching inclusion predicates</item>
    ///   <item><b>Left‑join</b> — associate each interface with its implementations</item>
    /// </list>
    ///
    /// <para>
    /// Generic interfaces and implementations are normalized to their generic type
    /// definitions to ensure correct matching (e.g., <c>IFoo&lt;T&gt;</c> matches
    /// <c>FooImpl&lt;T&gt;</c>).
    /// </para>
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <param name="options">The discovery options controlling inclusion.</param>
    /// <returns>
    /// A sequence of <see cref="RelevantInterfaceGroup"/> objects representing
    /// discovered interfaces and their associated implementations.
    /// </returns>
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

    /// <summary>
    /// Determines whether a type is a concrete (non‑abstract) class.
    ///
    /// <para>
    /// Generic type definitions are intentionally allowed so that open generic
    /// implementations (e.g., <c>Repository&lt;T&gt;</c>) are included.
    /// </para>
    /// </summary>
    /// <param name="t">The type to evaluate.</param>
    /// <returns><c>true</c> if the type is a concrete class; otherwise <c>false</c>.</returns>
    private static bool IsConcreteClassType(TypeInfo t)
        => t.IsClass &&
           !t.IsAbstract;

    /// <summary>
    /// Normalizes a type to its comparison key for interface‑implementation
    /// matching.
    ///
    /// <para>
    /// Generic types are reduced to their generic type definitions so that:
    /// </para>
    ///
    /// <example>
    /// <code>
    /// IFoo&lt;T&gt; → IFoo&lt;&gt;
    /// FooImpl&lt;T&gt; → FooImpl&lt;&gt;
    /// </code>
    /// </example>
    ///
    /// <para>
    /// This ensures correct matching between generic interfaces and their
    /// implementations.
    /// </para>
    /// </summary>
    /// <param name="type">The type to normalize.</param>
    /// <returns>
    /// The generic type definition if the type is generic; otherwise the type itself.
    /// </returns>
    private static Type GetComparisonKey(Type type)
        => type.IsGenericType
            ? type.GetGenericTypeDefinition()
            : type;
}
