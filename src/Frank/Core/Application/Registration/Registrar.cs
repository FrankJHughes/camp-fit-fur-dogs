using System.Reflection;
using Frank.Core.Application.Registration.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application.Registration;

/// <summary>
/// Applies validated registration <see cref="Plan"/> instances to an
/// <see cref="IServiceCollection"/>.
///
/// <para>
/// The registrar is the final stage of the registration pipeline. After
/// scanning, planning, and validation have completed, the registrar executes
/// each plan by creating the appropriate <see cref="ServiceDescriptor"/>
/// entries.
/// </para>
///
/// <para>
/// Each plan specifies:
/// </para>
/// <list type="bullet">
///   <item>The implemented interface to register</item>
///   <item>The implementing classes discovered during scanning</item>
///   <item>The <see cref="RegistrationAttribute"/> that defines lifetime and
///   concrete‑type registration rules</item>
/// </list>
///
/// <para>
/// The registrar does not perform validation; it assumes all plans have already
/// passed validation by <see cref="Validator"/>.
/// </para>
/// </summary>
public sealed class Registrar
{
    /// <summary>
    /// Registers all provided <see cref="Plan"/> instances into the given
    /// <see cref="IServiceCollection"/>.
    ///
    /// <para>
    /// Each plan may contain multiple implementing classes, all of which are
    /// registered according to the rules defined by the plan’s
    /// <see cref="RegistrationAttribute"/>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection into which registrations will be added.
    /// </param>
    /// <param name="plans">
    /// The collection of registration plans to execute.
    /// </param>
    public static void Register(IServiceCollection services, IEnumerable<Plan> plans)
    {
        foreach (var plan in plans)
        {
            Register(services, plan);
        }
    }

    /// <summary>
    /// Registers all implementing classes defined in a single <see cref="Plan"/>.
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <param name="plan">The plan describing the interface and its implementations.</param>
    private static void Register(IServiceCollection services, Plan plan)
    {
        foreach (var implementingClass in plan.ImplementingClasses)
        {
            ExecutePlan(services, plan, implementingClass);
        }
    }

    /// <summary>
    /// Executes the registration rules for a single implementing class within a
    /// <see cref="Plan"/>.
    ///
    /// <para>
    /// This includes:
    /// </para>
    /// <list type="bullet">
    ///   <item>Registering the implementing class as the service for the
    ///   implemented interface</item>
    ///   <item>Optionally registering the concrete type itself, depending on
    ///   <see cref="RegistrationAttribute.RegisterConcreteType"/></item>
    /// </list>
    ///
    /// <para>
    /// The lifetime used for both registrations is defined by
    /// <see cref="RegistrationAttribute.Lifetime"/>.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    /// <param name="plan">The plan describing the registration rules.</param>
    /// <param name="implementingClass">The implementing class to register.</param>
    private static void ExecutePlan(IServiceCollection services, Plan plan, TypeInfo implementingClass)
    {
        services.Add(
            new ServiceDescriptor(
                plan.ImplementedInterface,
                implementingClass,
                plan.AutoRegisterAttribute.Lifetime));

        if (plan.AutoRegisterAttribute.RegisterConcreteType)
        {
            services.Add(
                new ServiceDescriptor(
                    implementingClass,
                    implementingClass,
                    plan.AutoRegisterAttribute.Lifetime));
        }
    }
}
