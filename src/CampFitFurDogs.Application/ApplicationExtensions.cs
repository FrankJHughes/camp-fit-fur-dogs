using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Callback;
using Frank.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddApplicationAuthCallback();

        return services;
    }
}
