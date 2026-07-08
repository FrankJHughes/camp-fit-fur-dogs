using Microsoft.Extensions.DependencyInjection;
using Frank.Domain.Users;
using CampFitFurDogs.Application.Abstractions.Customer.FindCustomerByExternalId;

namespace CampFitFurDogs.Infrastructure.Customers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<IUserRepository, CustomerRepository>()
            .AddScoped<IFindCustomerByExternalIdReader, FindCustomerByExternalIdReader>();
    }
}
