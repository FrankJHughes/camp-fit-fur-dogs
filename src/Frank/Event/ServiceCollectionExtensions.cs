using Frank.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Event;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEvent(this IServiceCollection services)
    {
        return services.AddScoped<IEventDispatcher, EventDispatcher>();
    }

}
