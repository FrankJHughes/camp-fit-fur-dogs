using Microsoft.Extensions.DependencyInjection;

using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Customers.CreateCustomer;
using CampFitFurDogs.Application.Dogs.RegisterDog;
using CampFitFurDogs.Application.Dogs.GetDogProfile;

namespace CampFitFurDogs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<
            ICommandHandler<CreateCustomerCommand, Guid>,
            CreateCustomerHandler>();

        services.AddTransient<
            ICommandHandler<RegisterDogCommand, Guid>,
            RegisterDogHandler>();

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        services.AddTransient<
            IQueryHandler<GetDogProfileQuery, DogProfileResponse>,
            GetDogProfileHandler>();

        services.AddScoped<IQueryDispatcher, QueryDispatcher>();

        return services;
    }
}
