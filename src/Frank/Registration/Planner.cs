using System.Reflection;
using Frank.Registration.Shapes;

namespace Frank.Registration;

public static class Planner
{
    public static List<Plan> Plan(IEnumerable<RelevantInterfaceGroup> groups)
    {
        return
            [.. groups
            .SelectMany(group =>
                {
                    return PlanRelevantInterfaceGroup(group);
                })];

    }


    private static IEnumerable<Plan> PlanRelevantInterfaceGroup(RelevantInterfaceGroup group)
    {
        var (relevantInterface, implementations) = group;
        var autoRegisterAttribute = relevantInterface.GetCustomAttribute<AutoRegisterAttribute>()!;

        foreach (var (implementedInterface, implementingClasses) in GroupByImplementedInterface(implementations))
        {
            yield return new Plan(
                autoRegisterAttribute!,
                implementedInterface,
                implementingClasses
            );
        }
    }

    private static IEnumerable<ImplementedInterfaceGroup> GroupByImplementedInterface(IEnumerable<Implementation> implementations)
    {
        return
            implementations
            .GroupBy(
                implementation => implementation.ImplementedInterface, // key source
                implementation => implementation.ImplementingClass, // value source
                (implementedInterface, implementingClasses) =>
                    new ImplementedInterfaceGroup(implementedInterface, implementingClasses)); // (key, values)
    }

}
