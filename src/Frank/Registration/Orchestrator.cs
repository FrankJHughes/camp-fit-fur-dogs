using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Registration;

public sealed class Orchestrator
{
    public static void Orchestrate(
        IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        IEnumerable<Type> includeInterfaceTypes,
        RegistrationOptions registrationOptions)
    {

        var groups = Scanner.Scan(assemblies, includeInterfaceTypes, registrationOptions);

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
