using Frank.Core.Application.Abstractions.Clock;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Clock;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreInfrastructureClock(this IServiceCollection services)
    {
        return services
            .AddScoped<IClock, SystemClock>()
            ;
    }
}
