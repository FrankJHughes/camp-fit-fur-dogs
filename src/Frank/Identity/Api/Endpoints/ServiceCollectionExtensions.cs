using Frank.Core.Api.Endpoints;
using Frank.Core.Application.Registration;
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

        static DiscoveryOptions updateOptions(DiscoveryOptions options) => options.IncludeImplementations(t =>
            !string.IsNullOrWhiteSpace(t.Namespace) &&
            t.Namespace.StartsWith(typeof(ServiceCollectionExtensions).Namespace!));

        return services
            .AddFrankCoreApiEndpoints([
                typeof(AssemblyMarker).Assembly],
                options => updateOptions(options)
            );
    }
}
