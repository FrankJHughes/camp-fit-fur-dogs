using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

/// <summary>
/// Represents a discovered interface and all of its associated implementations
/// within the registration‑scanning pipeline.
///
/// <para>
/// A <see cref="RelevantInterfaceGroup"/> is produced by the
/// <see cref="Registration.Scanner"/> after applying interface‑inclusion and
/// implementation‑inclusion predicates. It contains:
/// </para>
///
/// <list type="bullet">
///   <item><b>The relevant interface</b> — an interface that matched the
///   configured <see cref="Registration.DiscoveryOptions"/>.</item>
///   <item><b>The implementations</b> — all concrete classes discovered that
///   implement that interface.</item>
/// </list>
///
/// <para>
/// This grouping is the bridge between scanning and planning:
/// </para>
/// <list type="bullet">
///   <item>The <see cref="Registration.Planner"/> transforms each group into one
///   or more <see cref="Plan"/> instances.</item>
///   <item>The <see cref="Registration.Validator"/> evaluates those plans for
///   registration‑count violations.</item>
///   <item>The <see cref="Registration.Registrar"/> executes the validated plans
///   into the dependency injection container.</item>
/// </list>
///
/// <para>
/// The record itself contains no behavior; it is a structural carrier used
/// throughout the registration pipeline.
/// </para>
/// </summary>
public sealed record RelevantInterfaceGroup(
    /// <summary>
    /// The interface that was identified as relevant during scanning.
    ///
    /// <para>
    /// This is always an interface type (<see cref="TypeInfo.IsInterface"/>),
    /// and may represent a generic type definition when normalization has been
    /// applied upstream.
    /// </para>
    /// </summary>
    TypeInfo RelevantInterface,

    /// <summary>
    /// The collection of discovered implementations associated with the
    /// relevant interface.
    ///
    /// <para>
    /// Each implementation is represented as an <see cref="Implementation"/>
    /// record pairing the concrete class with the interface it implements.
    /// </para>
    /// </summary>
    IEnumerable<Implementation> Implementations
);
