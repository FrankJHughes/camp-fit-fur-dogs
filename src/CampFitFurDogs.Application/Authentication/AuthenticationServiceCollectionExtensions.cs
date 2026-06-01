using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using Microsoft.Extensions.DependencyInjection;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthCallbackPipeline(this IServiceCollection services)
    {

        // Register concrete steps
        // ⭐ Token service
        services.AddScoped<ISessionTokenService, SessionTokenService>();

        // ⭐ Pipeline steps
        services.AddScoped<ValidateConfigStep>();
        services.AddScoped<ExchangeCodeStep>();
        services.AddScoped<FetchUserInfoStep>();
        services.AddScoped<ValidateUserInfoStep>();
        services.AddScoped<ResolveIdentityStep>();
        services.AddScoped<CreateSessionCookieStep>();
        services.AddScoped<CreateSessionStep>();
        services.AddScoped<AuditLoginStep>();
        services.AddScoped<BuildRedirectStep>();

        // Register the pipeline using the builder
        services.AddScoped<AuthCallbackPipeline>(sp =>
            new AuthCallbackPipelineBuilder(sp)
                .Add<ValidateConfigStep>()
                .Add<ExchangeCodeStep>()
                .Add<FetchUserInfoStep>()
                .Add<ValidateUserInfoStep>()
                .Add<ResolveIdentityStep>()
                .Add<CreateSessionCookieStep>()
                .Add<CreateSessionStep>()
                .Add<AuditLoginStep>()
                .Add<BuildRedirectStep>()
                .Build());

        return services;
    }
}
