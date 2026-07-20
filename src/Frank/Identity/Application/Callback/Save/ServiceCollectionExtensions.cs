using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Callback.Save.Steps;
using Frank.Identity.Application.Settings;
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

        services.AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, AuditLoginStep>();
        services.AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, BuildCookieStep>();
        services.AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, CreateSessionStep>();
        services.AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, ResolveUserStep>();

        services.AddTransient<ICallbackSaveContextBuilder, CallbackSaveContextBuilder>();

        return services;
    }
}
