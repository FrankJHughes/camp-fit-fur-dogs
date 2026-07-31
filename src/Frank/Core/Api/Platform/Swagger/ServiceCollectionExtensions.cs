using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Platform.Swagger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankCoreApiPlatformSwagger(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }
}
