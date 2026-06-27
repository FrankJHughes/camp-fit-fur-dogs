using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankHosting(this IServiceCollection services)
    {
        services.AddSingleton<HostingEngine>();
        return services;
    }
}
