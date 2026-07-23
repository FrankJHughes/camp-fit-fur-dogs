using Frank.Core.Application.Abstractions.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.EnvironmentVariables;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreInfrastructureEnvironmentVariables(this IServiceCollection services)
    {
        services.AddScoped<IEnvironmentVariables, SystemEnvironmentVariables>();
        return services;
    }
}
