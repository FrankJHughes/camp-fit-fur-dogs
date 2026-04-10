using Microsoft.Extensions.DependencyInjection;

using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Customers.CreateCustomer;

namespace CampFitFurDogs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<
            ICommandHandler<CreateCustomerCommand, Guid>,
            CreateCustomerHandler>();

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        return services;
    }
}
