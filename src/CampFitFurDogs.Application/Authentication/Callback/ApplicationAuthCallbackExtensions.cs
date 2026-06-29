using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Authentication.Callback.Steps;
using Frank.Abstractions.ImmutableContext;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application.Authentication.Callback;

public static class ApplicationAuthCallbackExtensions
{
    public static IServiceCollection AddApplicationAuthCallback(this IServiceCollection services)
    {

        // ⭐ Token service
        services.AddScoped<ISessionTokenService, SessionTokenService>();

        // ⭐ Pipeline steps (register as implementations of IAuthCallbackStep so discovery works)
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, AuditLoginStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, BuildCookieStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, BuildRedirectStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, CreateSessionStep>();
        services.AddTransient<IImmutableContextBuildStep<ApplicationAuthCallbackContext>, ResolveCustomerStep>();

        services.AddTransient<IImmutableContextBuilder<ApplicationAuthCallbackRequest, ApplicationAuthCallbackContext, ApplicationAuthCallbackContextBuilderResult>,
            ApplicationAuthCallbackContextBuilder>();

        return services;
    }
}
