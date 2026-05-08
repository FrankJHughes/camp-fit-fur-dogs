using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.DependencyInjection.AutoRegistration;

namespace SharedKernel.DependencyInjection;

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
