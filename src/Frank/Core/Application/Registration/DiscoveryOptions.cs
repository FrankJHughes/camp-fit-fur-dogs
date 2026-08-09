using System.Reflection;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Provides configuration options for controlling type discovery during
/// registration scanning. These options allow callers to specify which
/// interfaces and implementations should be included when the registration
/// system inspects assemblies.
///
/// <para>
/// Discovery is predicate‑driven: callers supply functions that evaluate
/// <see cref="TypeInfo"/> instances and determine whether they should be
/// included in the scan. This enables flexible filtering based on naming
/// conventions, attributes, inheritance, or any other type metadata.
/// </para>
///
/// <para>
/// These options are consumed internally by the registration orchestrator
/// and are not intended for direct use outside the scanning pipeline.
/// </para>
/// </summary>
public sealed class DiscoveryOptions
{
    /// <summary>
    /// Gets the collection of predicates used to determine whether an interface
    /// should be included during discovery. Each predicate receives a
    /// <see cref="TypeInfo"/> representing an interface and returns <c>true</c>
    /// if it should be included.
    /// </summary>
    internal List<Func<TypeInfo, bool>> InterfaceInclusionPredicates { get; } = [];

    /// <summary>
    /// Gets the collection of predicates used to determine whether an
    /// implementation type should be included during discovery. Each predicate
    /// receives a <see cref="TypeInfo"/> representing a concrete type and
    /// returns <c>true</c> if it should be included.
    /// </summary>
    internal List<Func<TypeInfo, bool>> ImplementationInclusionPredicates { get; } = [];

    /// <summary>
    /// Adds a predicate that determines whether an interface should be included
    /// during scanning. Multiple predicates may be added; an interface is
    /// included if **any** predicate returns <c>true</c>.
    /// </summary>
    /// <param name="predicate">
    /// A function that evaluates an interface type and returns <c>true</c> if it
    /// should be included.
    /// </param>
    /// <returns>
    /// The same <see cref="DiscoveryOptions"/> instance, enabling fluent chaining.
    /// </returns>
    public DiscoveryOptions IncludeInterfaces(Func<TypeInfo, bool> predicate)
    {
        InterfaceInclusionPredicates.Add(predicate);
        return this;
    }

    /// <summary>
    /// Adds a predicate that determines whether an implementation type should be
    /// included during scanning. Multiple predicates may be added; an
    /// implementation is included only if **all** predicates return <c>true</c>.
    /// </summary>
    /// <param name="predicate">
    /// A function that evaluates an implementation type and returns <c>true</c>
    /// if it should be included.
    /// </param>
    /// <returns>
    /// The same <see cref="DiscoveryOptions"/> instance, enabling fluent chaining.
    /// </returns>
    public DiscoveryOptions IncludeImplementations(Func<TypeInfo, bool> predicate)
    {
        ImplementationInclusionPredicates.Add(predicate);
        return this;
    }

    /// <summary>
    /// Determines whether the specified interface should be included based on
    /// the configured interface inclusion predicates. An interface is included
    /// if any predicate returns <c>true</c>.
    /// </summary>
    /// <param name="iface">The interface type being evaluated.</param>
    /// <returns>
    /// <c>true</c> if the interface should be included; otherwise, <c>false</c>.
    /// </returns>
    internal bool ShouldIncludeInterface(TypeInfo iface)
        => InterfaceInclusionPredicates.Any(p => p(iface));

    /// <summary>
    /// Determines whether the specified implementation type should be included
    /// based on the configured implementation inclusion predicates. An
    /// implementation is included only if at least one predicate exists and
    /// all predicates return <c>true</c>.
    /// </summary>
    /// <param name="impl">The implementation type being evaluated.</param>
    /// <returns>
    /// <c>true</c> if the implementation should be included; otherwise, <c>false</c>.
    /// </returns>
    internal bool ShouldIncludeImplementation(TypeInfo impl) =>
        ImplementationInclusionPredicates.Count > 0 &&
        ImplementationInclusionPredicates.All(p => p(impl));
}
