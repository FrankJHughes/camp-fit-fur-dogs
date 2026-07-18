using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;
using Frank.Identity.Application.Callback.Auth0;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Frank.Identity.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback.Oidc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityCallbackOidc(this IServiceCollection services)
    {
        services
            .AddOptions<OidcCallbackSettings>()
            .BindConfiguration("Authentication:Callback:Oidc")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<IOidcUserInfoClient, Auth0OidcUserInfoClient>();
        services.AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, ExchangeCodeStep>();
        services.AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, FetchUserInfoStep>();
        services.AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, ValidateTokensStep>();

        services.AddTransient<ICallbackOidcContextBuilder, CallbackOidcContextBuilder>();

        return services;
    }
}
