using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

/// <summary>
/// Represents a grouping of concrete implementing classes associated with a
/// single implemented interface.
///
/// <para>
/// During scanning, each discovered implementation is paired with the interface
/// it implements. The planner then groups these pairs by implemented interface,
/// producing an <see cref="ImplementedInterfaceGroup"/> for each distinct
/// interface (including generic type definitions).
/// </para>
///
/// <para>
/// These groups are later transformed into registration <see cref="Plan"/>
/// instances, which define how each interface should be registered.
/// </para>
/// </summary>
public sealed record ImplementedInterfaceGroup(
    /// <summary>
    /// The interface implemented by the grouped classes.
    ///
    /// <para>
    /// This may be a generic type definition (e.g., <c>IFoo&lt;&gt;</c>) when
    /// normalization is applied upstream by <see cref="Scanner"/>.
    /// </para>
    /// </summary>
    Type ImplementedInterface,

    /// <summary>
    /// The concrete classes that implement the interface.
    ///
    /// <para>
    /// Each class is represented as a <see cref="TypeInfo"/> and may include
    /// open generic types (e.g., <c>FooImpl&lt;T&gt;</c>) when applicable.
    /// </para>
    /// </summary>
    IEnumerable<TypeInfo> ImplementingClasses
);
