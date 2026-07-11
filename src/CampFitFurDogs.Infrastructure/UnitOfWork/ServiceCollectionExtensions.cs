using CampFitFurDogs.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
    }
}
