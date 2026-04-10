using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Customers;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
