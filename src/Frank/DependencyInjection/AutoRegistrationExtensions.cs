using System.Reflection;
using Frank.DependencyInjection.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.DependencyInjection;

public static partial class AutoRegistrationExtensions
{
    public static IServiceCollection AddAutoRegistration(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        Orchestrator.Orchestrate(services, assemblies);

        return services;
    }
}
