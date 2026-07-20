using Frank.Identity.Application.Abstractions.Oidc;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Infrastructure.Auth0;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityAuth0Infrastructure(this IServiceCollection services)
    {
        return services
            .AddTransient<IOidcUserInfoClient, Auth0OidcUserInfoClient>()
            .AddTransient<IOidcTokenClient, Auth0OidcTokenClient>()
            .AddTransient<IOidcTokenValidator, Auth0OidcTokenValidator>();
    }
}

