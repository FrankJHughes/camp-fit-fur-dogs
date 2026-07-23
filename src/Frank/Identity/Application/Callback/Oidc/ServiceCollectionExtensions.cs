using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback.Oidc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplicationCallbackOidc(this IServiceCollection services)
    {
        return services
            .AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, ExchangeCodeStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, FetchUserInfoStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, ValidateTokensStep>()

            .AddTransient<ICallbackOidcContextBuilder, CallbackOidcContextBuilder>();
    }
}
