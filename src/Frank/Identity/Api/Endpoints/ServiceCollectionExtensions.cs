using Frank.Core.Api.Endpoints;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Api.Endpoints;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApiEndpoints(this IServiceCollection services)
    {

        services
            .AddOptions<FrontendSettings>()
            .BindConfiguration("Frontend")
            .ValidateDataAnnotations()
            .ValidateOnStart(); // AuthLoginEndpoint dependency

        return services.AddFrankEndpoints([typeof(Frank.Identity.Api.AssemblyMarker).Assembly]);
    }
}
