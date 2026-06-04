using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Frank.DependencyInjection.AutoRegistration.Shapes;

namespace Frank.DependencyInjection.AutoRegistration;

public sealed class Orchestrator
{
    public static void Orchestrate(IServiceCollection services, Assembly[] assemblies)
    {

        var groups = Scanner.Scan(assemblies);

        var plans = Planner.Plan(groups);

        var violations = Validator.SurfaceViolations(plans!);

        if (violations.Count > 0)
        {
            throw new InvalidOperationException(
                Formatter.Format(violations));
        }

        Registrar.Register(services, plans);
    }
}
