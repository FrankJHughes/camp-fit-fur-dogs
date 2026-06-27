using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Registration;

public sealed class Orchestrator
{
    public static void Orchestrate(
        IServiceCollection services,
        IEnumerable<Type> includeInterfaceTypes,
        IEnumerable<Type> excludeConcreteTypes,
        IEnumerable<Assembly> assemblies)
    {

        var groups = Scanner.Scan(includeInterfaceTypes, excludeConcreteTypes, assemblies);

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
