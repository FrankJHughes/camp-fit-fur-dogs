using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Middleware.Observations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApiObservations(this IServiceCollection services)
    {
        _ = services
            .AddHttpClient("*")
            .AddHttpMessageHandler<OutboundObservationContextHandler>();

        return services
            .AddHttpContextAccessor()
            .AddTransient<OutboundObservationContextHandler>();
    }

}
