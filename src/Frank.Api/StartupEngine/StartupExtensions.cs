using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api.StartupEngine;

public static class StartupExtensions
{
    public static IServiceCollection AddStartupEngine(this IServiceCollection services)
    {
        services.AddSingleton<StartupEngine>();
        return services;
    }
}
