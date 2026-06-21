using Frank.AutoRegistration.Shapes;

namespace Frank.AutoRegistration;

public sealed class Validator
{
    public static IReadOnlyList<Violation> SurfaceViolations(List<Plan> plans)
    {
        return
            [.. plans.SelectMany(SurfaceViolations)];
    }

    private static IEnumerable<Violation> SurfaceViolations(Plan plan)
    {
        var minCount = plan.AutoRegisterAttribute!.MinRegistrationCount;
        var maxCount = plan.AutoRegisterAttribute!.MaxRegistrationCount;
        var actualCount = plan.ImplementingClasses.Count();
        if (minCount <= actualCount && actualCount <= maxCount)
        {
            yield break;
        }

        var violation = new Violation(
            plan,
            actualCount,
            minCount,
            maxCount);

        yield return violation;
    }
}
