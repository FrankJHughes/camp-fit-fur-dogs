using CampFitFurDogs.Application.Abstractions;
using Frank.Core.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
    }
}
