using Frank.Abstractions.Command;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Command;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCommand(this IServiceCollection services)
    {
        return services.AddScoped<ICommandDispatcher, CommandDispatcher>();
    }
}
