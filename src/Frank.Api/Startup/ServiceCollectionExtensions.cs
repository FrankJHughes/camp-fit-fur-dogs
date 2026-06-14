using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api.Startup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartupEngine(this IServiceCollection services)
    {
        services.AddSingleton<StartupEngine>();
        return services;
    }
}
