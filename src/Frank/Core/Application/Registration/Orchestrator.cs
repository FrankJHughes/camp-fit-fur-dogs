using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Coordinates the full registration‑discovery pipeline, including scanning,
/// planning, validation, and final service registration.
///
/// <para>
/// The orchestrator is the top‑level entry point for the registration system.
/// It processes assemblies using the following sequence:
/// </para>
///
/// <list type="number">
///   <item>
///     <description>
///     <b>Scan</b> — Uses <see cref="Scanner"/> to discover interfaces and
///     implementations based on <see cref="DiscoveryOptions"/>.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Plan</b> — Uses <see cref="Planner"/> to build registration plans
///     describing expected implementation counts.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Validate</b> — Uses <see cref="Validator"/> to surface violations
///     when implementation counts fall outside required ranges.
///     </description>
///   </item>
///   <item>
///     <description>
///     <b>Register</b> — Uses <see cref="Registrar"/> to apply the validated
///     registration plan to the <see cref="IServiceCollection"/>.
///     </description>
///   </item>
/// </list>
///
/// <para>
/// If any violations are detected, the orchestrator throws an
/// <see cref="InvalidOperationException"/> containing a formatted diagnostic
/// message produced by <see cref="Formatter"/>.
/// </para>
/// </summary>
public sealed class Orchestrator
{
    /// <summary>
    /// Executes the full registration‑discovery pipeline over the provided
    /// assemblies using the specified <see cref="DiscoveryOptions"/>.
    ///
    /// <para>
    /// This method is typically invoked during application startup to ensure
    /// that all required interfaces and implementations are correctly discovered
    /// and registered according to the platform’s conventions.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection into which validated registrations will be applied.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies to scan for interfaces and implementations.
    /// </param>
    /// <param name="discoveryOptions">
    /// Options controlling which interfaces and implementations are included
    /// during scanning.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when one or more registration violations are detected. The
    /// exception message contains a formatted description of all violations.
    /// </exception>
    public static void Orchestrate(
        IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        DiscoveryOptions discoveryOptions)
    {
        var groups = Scanner.Scan(assemblies, discoveryOptions);

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
