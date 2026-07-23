using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure.Auth0;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityInfrastructureAuth0(this IServiceCollection services)
    {
        services
            .AddOptions<OidcSettings>()
            .BindConfiguration("Identity:Oidc")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddTransient<IOidcUserInfoClient, Auth0OidcUserInfoClient>()
            .AddTransient<IOidcTokenClient, Auth0OidcTokenClient>()
            .AddTransient<IOidcTokenValidator, Auth0OidcTokenValidator>();
    }
}

