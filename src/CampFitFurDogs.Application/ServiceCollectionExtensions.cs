using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddAuthCallbackPipeline();
        services.AddScoped<IAuthCallbackService, AuthCallbackService>();

        return services;
    }
}
