using Frank.Core.Api.Endpoints;
using Frank.Identity.Api.Settings;
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
            .ValidateOnStart(); // Dependents: GetLoginUrlEndpoint,LogoutEndpoint

        return services.AddFrankCoreApiEndpoints([
            typeof(AssemblyMarker).Assembly
        ]);
    }
}
