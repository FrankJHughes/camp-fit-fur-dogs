using Frank.Abstractions.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Query;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankQuery(this IServiceCollection services)
    {
        return services.AddScoped<IQueryDispatcher, QueryDispatcher>();
    }

}
