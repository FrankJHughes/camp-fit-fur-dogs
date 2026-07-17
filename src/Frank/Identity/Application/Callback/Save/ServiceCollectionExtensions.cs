using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Core.Application.Abstractions.Authentication;
using Frank.Identity.Application.Callback.Save.Steps;
using Frank.Core.Application.Settings;
using Microsoft.Extensions.DependencyInjection;
using Frank.Identity.Application.Abstractions.Callback.Save;

namespace Frank.Identity.Application.Callback.Save;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityCallbackSave(this IServiceCollection services)
    {

        services
            .AddOptions<SaveCallbackSettings>()
            .BindConfiguration("Authentication:Callback")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ⭐ Token service
        services.AddScoped<ISessionTokenService, SessionTokenService>();

        // ⭐ Pipeline steps (register as implementations of IAuthCallbackStep so discovery works)
        services.AddTransient<IImmutableContextBuildStep<SaveCallbackContext>, AuditLoginStep>();
        services.AddTransient<IImmutableContextBuildStep<SaveCallbackContext>, BuildCookieStep>();
        services.AddTransient<IImmutableContextBuildStep<SaveCallbackContext>, CreateSessionStep>();
        services.AddTransient<IImmutableContextBuildStep<SaveCallbackContext>, ResolveUserStep>();

        services.AddTransient<IImmutableContextBuilder<SaveCallbackContextBuilderRequest, SaveCallbackContext, SaveCallbackContextBuilderResult>,
            CallbackSaveContextBuilder>();

        return services;
    }
}
