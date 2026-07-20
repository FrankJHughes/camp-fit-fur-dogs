using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Save;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityApplicationCallback(this IServiceCollection services)
    {
        return services

            .AddFrankIdentityApplicationCallbackOidc()
            .AddFrankIdentityApplicationCallbackSave();

    }
}
