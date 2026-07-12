using Frank.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Api.Endpoints.Identity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityEndpoints(this IServiceCollection services)
    {

        services
            .AddOptions<FrontendSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart(); // AuthLoginEndpoint dependency


        return services.AddFrankEndpoints([typeof(Frank.Api.AssemblyMarker).Assembly]);
    }
}
