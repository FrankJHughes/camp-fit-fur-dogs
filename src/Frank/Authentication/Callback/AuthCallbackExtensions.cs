using Frank.Abstractions.Authentication.Callback;
using Frank.Abstractions.ImmutableContext;
using Frank.Authentication.Callback.Oidc;
using Frank.Authentication.Callback.Oidc.Steps;
using Frank.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Authentication.Callback;

public static class AuthCallbackExtensions
{
    public static IServiceCollection AddFrankAuthCallback(this IServiceCollection services)
    {
        services.AddOptions<AuthCallbackOidcSettings>()
                .BindConfiguration("Authentication:Callback:Oidc")
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddTransient<IImmutableContextBuildStep<OidcAuthCallbackContext>, ExchangeCodeStep>();
        services.AddTransient<IImmutableContextBuildStep<OidcAuthCallbackContext>, FetchUserInfoStep>();
        services.AddTransient<IImmutableContextBuildStep<OidcAuthCallbackContext>, ValidateTokensStep>();

        services.AddTransient<IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>,
            OidcAuthCallbackContextBuilder>();

        return services;
    }
}
