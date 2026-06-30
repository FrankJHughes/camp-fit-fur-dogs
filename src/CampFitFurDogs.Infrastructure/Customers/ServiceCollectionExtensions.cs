using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Application.Abstractions.Customer.FindCustomerByExternalId;

namespace CampFitFurDogs.Infrastructure.Customers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomerInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IFindCustomerByExternalIdReader, FindCustomerByExternalIdReader>();
    }
}
