using System.Reflection;
using Frank.Registration.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Registration;

public sealed class Registrar
{
    public static void Register(IServiceCollection services, IEnumerable<Plan> plans)
    {
        foreach (var plan in plans)
        {
            Register(services, plan);
        }
    }

    private static void Register(IServiceCollection services, Plan plan)
    {
        foreach (var implementingClass in plan.ImplementingClasses)
        {
            ExecutePlan(services, plan, implementingClass);
        }
    }

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
