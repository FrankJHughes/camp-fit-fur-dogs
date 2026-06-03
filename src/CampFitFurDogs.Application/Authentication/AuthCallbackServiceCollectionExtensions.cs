using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps

;
using Microsoft.Extensions.DependencyInjection;

public static class AuthCallbackServiceCollectionExtensions
{
    public static IServiceCollection AddAuthCallbackPipeline(this IServiceCollection services)
    {

        // Register concrete steps
        // ⭐ Token service
        services.AddScoped<ISessionTokenService, SessionTokenService>();

        // ⭐ Pipeline steps (register as implementations of IAuthCallbackStep so discovery works)
        services.AddScoped<IAuthCallbackStep, ValidateConfigurationStep>();
        services.AddScoped<IAuthCallbackStep, ExchangeCodeStep>();
        services.AddScoped<IAuthCallbackStep, FetchUserStep>();
        services.AddScoped<IAuthCallbackStep, ValidateUserStep>();
        services.AddScoped<IAuthCallbackStep, ResolveIdentityStep>();
        services.AddScoped<IAuthCallbackStep, IssueCookieStep>();
        services.AddScoped<IAuthCallbackStep, CreateSessionStep>();
        services.AddScoped<IAuthCallbackStep, AuditLoginStep>();
        services.AddScoped<IAuthCallbackStep, BuildRedirectStep>();

        services.AddScoped<AuthCallbackExecutor>();
        return services;
    }
}
