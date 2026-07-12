using Frank.Abstractions.ImmutableContext;
using Frank.Application.Abstractions.Authentication;
using Frank.Application.Abstractions.Identity.Callback;
using Frank.Application.Identity.Callback.Steps;
using Frank.Application.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Application.Identity.Callback;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationAuthCallback(this IServiceCollection services)
    {

        // services
        //     .AddOptions<AuthCallbackSettings>()
        //     .BindConfiguration("Frontend")
        //     .ValidateDataAnnotations()
        //     .ValidateOnStart();

        services
            .AddOptions<ApplicationAuthCallbackSettings>()
            .BindConfiguration("Authentication:Callback")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ⭐ Token service
        services.AddScoped<ISessionTokenService, SessionTokenService>();

        // ⭐ Pipeline steps (register as implementations of IAuthCallbackStep so discovery works)
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, AuditLoginStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, BuildCookieStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, CreateSessionStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, ResolveUserStep>();

        services.AddTransient<IImmutableContextBuilder<ApplicationAuthCallbackRequest, ApplicationAuthCallbackContext, ApplicationAuthCallbackContextBuilderResult>,
            ApplicationAuthCallbackContextBuilder>();

        return services;
    }
}
