using Frank.Core.Application.Registration.Shapes;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Evaluates registration <see cref="Plan"/> instances and surfaces any
/// violations related to implementation count requirements.
///
/// <para>
/// The validator is responsible for enforcing the registration rules defined
/// by <see cref="RegistrationAttribute"/>. These rules specify the minimum and
/// maximum number of implementations allowed for a given interface.
/// </para>
///
/// <para>
/// The validator does not throw exceptions directly. Instead, it returns a
/// collection of <see cref="Violation"/> objects. The caller (typically
/// <see cref="Orchestrator"/>) decides how to handle them.
/// </para>
/// </summary>
public sealed class Validator
{
    /// <summary>
    /// Evaluates all provided <see cref="Plan"/> instances and returns a
    /// flattened list of <see cref="Violation"/> objects representing any
    /// registration rule failures.
    ///
    /// <para>
    /// Each plan is validated independently. Plans that satisfy their
    /// registration constraints produce no violations.
    /// </para>
    /// </summary>
    /// <param name="plans">The registration plans to validate.</param>
    /// <returns>
    /// A read‑only list of <see cref="Violation"/> objects. The list is empty
    /// when all plans satisfy their constraints.
    /// </returns>
    public static IReadOnlyList<Violation> SurfaceViolations(List<Plan> plans)
    {
        return
            [.. plans.SelectMany(SurfaceViolations)];
    }

    /// <summary>
    /// Validates a single <see cref="Plan"/> and yields a violation if the
    /// number of discovered implementations falls outside the allowed range.
    ///
    /// <para>
    /// A plan is considered valid when:
    /// </para>
    /// <code>
    /// MinRegistrationCount ≤ ActualRegistrationCount ≤ MaxRegistrationCount
    /// </code>
    ///
    /// <para>
    /// If the plan violates these constraints, a <see cref="Violation"/> is
    /// produced containing the plan, the actual count, and the expected range.
    /// </para>
    /// </summary>
    /// <param name="plan">The plan to validate.</param>
    /// <returns>
    /// Zero or one <see cref="Violation"/> objects depending on whether the plan
    /// satisfies its registration constraints.
    /// </returns>
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
