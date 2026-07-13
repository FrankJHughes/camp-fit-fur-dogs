using Frank.Core.Application.Abstractions.Time;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Infrastructure.Time;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankTime(this IServiceCollection services)
    {
        services.AddScoped<IClock, SystemClock>();
        return services;
    }
}
