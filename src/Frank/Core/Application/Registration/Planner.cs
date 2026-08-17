using System.Reflection;
using Frank.Core.Application.Registration.Shapes;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Produces registration <see cref="Plan"/> instances from discovered
/// <see cref="RelevantInterfaceGroup"/> collections.
///
/// <para>
/// The planner is responsible for interpreting the results of the scanning
/// phase and converting them into structured plans that describe how each
/// interface should be registered. These plans include:
/// </para>
///
/// <list type="bullet">
///   <item>The interface being registered</item>
///   <item>The <see cref="RegistrationAttribute"/> that defines registration rules</item>
///   <item>The implementing classes discovered for that interface</item>
/// </list>
///
/// <para>
/// The planner does not perform validation. Instead, it produces plans that
/// are later evaluated by <see cref="Validator"/> to surface violations.
/// </para>
/// </summary>
public static class Planner
{
    /// <summary>
    /// Generates a collection of <see cref="Plan"/> instances from the provided
    /// <see cref="RelevantInterfaceGroup"/> items.
    ///
    /// <para>
    /// Each relevant interface group may produce multiple plans if the interface
    /// has multiple implemented interface variants (e.g., generic interface
    /// specializations).
    /// </para>
    /// </summary>
    /// <param name="groups">
    /// The relevant interface groups discovered during scanning.
    /// </param>
    /// <returns>
    /// A list of <see cref="Plan"/> objects describing how each interface should
    /// be registered.
    /// </returns>
    public static List<Plan> Plan(IEnumerable<RelevantInterfaceGroup> groups)
    {
        return
            [.. groups
                .SelectMany(group =>
                {
                    return PlanRelevantInterfaceGroup(group);
                })];
    }

    /// <summary>
    /// Produces one or more <see cref="Plan"/> instances for a single
    /// <see cref="RelevantInterfaceGroup"/>.
    ///
    /// <para>
    /// The method extracts the <see cref="RegistrationAttribute"/> from the
    /// interface and groups implementations by their implemented interface
    /// (including generic variants). Each group becomes a separate plan.
    /// </para>
    /// </summary>
    /// <param name="group">
    /// The relevant interface group containing the interface and its discovered
    /// implementations.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="Plan"/> objects derived from the group.
    /// </returns>
    private static IEnumerable<Plan> PlanRelevantInterfaceGroup(RelevantInterfaceGroup group)
    {
        var (relevantInterface, implementations) = group;
        var autoRegisterAttribute = relevantInterface.GetCustomAttribute<RegistrationAttribute>()!;

        foreach (var (implementedInterface, implementingClasses) in GroupByImplementedInterface(implementations))
        {
            yield return new Plan(
                autoRegisterAttribute!,
                implementedInterface,
                implementingClasses
            );
        }
    }

    /// <summary>
    /// Groups implementation types by the interface they implement.
    ///
    /// <para>
    /// This grouping is necessary because a single relevant interface may have
    /// multiple implemented interface variants (e.g., generic specializations).
    /// Each group becomes a separate <see cref="ImplementedInterfaceGroup"/>,
    /// which is later transformed into a <see cref="Plan"/>.
    /// </para>
    /// </summary>
    /// <param name="implementations">
    /// The discovered implementations associated with a relevant interface.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="ImplementedInterfaceGroup"/> objects, each
    /// containing an implemented interface and its corresponding implementing
    /// classes.
    /// </returns>
    private static IEnumerable<ImplementedInterfaceGroup> GroupByImplementedInterface(IEnumerable<Implementation> implementations)
    {
        return
            implementations
            .GroupBy(
                implementation => implementation.ImplementedInterface, // key source
                implementation => implementation.ImplementingClass,    // value source
                (implementedInterface, implementingClasses) =>
                    new ImplementedInterfaceGroup(implementedInterface, implementingClasses)); // (key, values)
    }
}
