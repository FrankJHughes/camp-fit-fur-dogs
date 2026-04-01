using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Future stories: register MediatR, FluentValidation, application services
        return services;
    }
}
