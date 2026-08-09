using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

/// <summary>
/// Represents a discovered relationship between a concrete implementing class
/// and the interface it implements during the assembly‑scanning phase.
///
/// <para>
/// Instances of this record are produced by <see cref="Scanner"/> when it
/// enumerates all concrete classes and associates them with the interfaces
/// they implement. Each pair forms a single <see cref="Implementation"/> entry,
/// which is later grouped into <see cref="ImplementedInterfaceGroup"/> objects
/// and ultimately transformed into registration <see cref="Plan"/> instances.
/// </para>
///
/// <para>
/// This record contains no behavior; it is a simple structural carrier used
/// throughout the registration pipeline.
/// </para>
/// </summary>
public sealed record Implementation(
    /// <summary>
    /// The concrete class that implements the interface.
    /// </summary>
    TypeInfo ImplementingClass,

    /// <summary>
    /// The interface implemented by the concrete class.
    ///
    /// <para>
    /// Generic interfaces are preserved exactly as discovered; normalization
    /// (e.g., converting to generic type definitions) is performed upstream
    /// by <see cref="Scanner"/> when necessary for matching.
    /// </para>
    /// </summary>
    Type ImplementedInterface
);
