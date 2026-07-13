using Frank.Core.Application.Abstractions.Environment;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Environment;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEnvironment(this IServiceCollection services)
    {
        services.AddScoped<IEnvironment, SystemEnvironment>();
        return services;
    }
}
