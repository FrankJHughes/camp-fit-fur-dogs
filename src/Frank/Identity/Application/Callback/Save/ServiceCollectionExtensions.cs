using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Callback.Save.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback.Save;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplicationCallbackSave(this IServiceCollection services)
    {

        return services

            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, AuditLoginStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, BuildCookieStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, CreateSessionStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, ResolveUserStep>()

            .AddTransient<ICallbackSaveContextBuilder, CallbackSaveContextBuilder>();

    }
}
