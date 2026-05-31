using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication.Steps;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // ⭐ Bind strongly-typed Oidc configuration
        services.AddOptions<OidcOptions>()
                .BindConfiguration("Authentication:Oidc")
                .ValidateDataAnnotations()
                .ValidateOnStart();

        // ⭐ Pipeline service
        services.AddScoped<IAuthCallbackService, AuthCallbackService>();

        // ⭐ Pipeline steps
        services.AddScoped<IAuthCallbackStep, ValidateConfigStep>();
        services.AddScoped<IAuthCallbackStep, ExchangeCodeStep>();
        services.AddScoped<IAuthCallbackStep, FetchUserInfoStep>();
        services.AddScoped<IAuthCallbackStep, ValidateUserInfoStep>();
        services.AddScoped<IAuthCallbackStep, ResolveIdentityStep>();
        services.AddScoped<IAuthCallbackStep, CreateSessionCookieStep>();
        services.AddScoped<IAuthCallbackStep, CreateSessionStep>();
        services.AddScoped<IAuthCallbackStep, AuditLoginStep>();
        services.AddScoped<IAuthCallbackStep, BuildRedirectStep>();

        return services;
    }
}
