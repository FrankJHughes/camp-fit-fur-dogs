using Frank.Core.Application.Abstractions.ImmutableContext;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback.Oidc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOidcCallback(this IServiceCollection services)
    {
        services
            .AddOptions<OidcCallbackSettings>()
            .BindConfiguration("Authentication:Callback:Oidc")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<IOidcUserInfoClient, Auth0OidcUserInfoClient>();
        services.AddTransient<IImmutableContextBuildStep<OidcCallbackContext>, ExchangeCodeStep>();
        services.AddTransient<IImmutableContextBuildStep<OidcCallbackContext>, FetchUserInfoStep>();
        services.AddTransient<IImmutableContextBuildStep<OidcCallbackContext>, ValidateTokensStep>();

        services.AddTransient<IImmutableContextBuilder<OidcCallbackContextBuilderRequest, OidcCallbackContext, OidcCallbackContextBuilderResult>,
            OidcCallbackContextBuilder>();

        return services;
    }
}
