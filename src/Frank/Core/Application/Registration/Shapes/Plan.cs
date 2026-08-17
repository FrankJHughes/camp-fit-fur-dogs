using System.Reflection;

namespace Frank.Core.Application.Registration.Shapes;

/// <summary>
/// Represents a fully constructed registration plan for a single implemented
/// interface.
///
/// <para>
/// A <see cref="Plan"/> is produced by the <see cref="Registration.Planner"/>
/// after scanning and grouping implementations. It contains all information
/// required to register services into the dependency injection container:
/// </para>
///
/// <list type="bullet">
///   <item>
///     The <see cref="RegistrationAttribute"/> that defines registration rules
///     such as lifetime, minimum/maximum implementation counts, and whether
///     concrete types should also be registered.
///   </item>
///   <item>
///     The implemented interface being registered.
///   </item>
///   <item>
///     The concrete implementing classes discovered for that interface.
///   </item>
/// </list>
///
/// <para>
/// Plans are validated by <see cref="Registration.Validator"/> and executed by
/// <see cref="Registration.Registrar"/>. They contain no behavior themselves;
/// they are structural carriers used throughout the registration pipeline.
/// </para>
/// </summary>
public sealed record Plan(
    /// <summary>
    /// The attribute applied to the interface that defines registration rules,
    /// including lifetime, minimum/maximum implementation counts, and whether
    /// concrete types should also be registered.
    /// </summary>
    RegistrationAttribute AutoRegisterAttribute,

    /// <summary>
    /// The interface that is being registered.
    ///
    /// <para>
    /// This may be a generic type definition when normalization has been applied
    /// upstream by the scanner.
    /// </para>
    /// </summary>
    Type ImplementedInterface,

    /// <summary>
    /// The concrete classes that implement the interface.
    ///
    /// <para>
    /// Each class is represented as a <see cref="TypeInfo"/> and may include
    /// open generic types when applicable.
    /// </para>
    /// </summary>
    IEnumerable<TypeInfo> ImplementingClasses
);
