using Frank.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IFrankIdentityUnitOfWork, FrankIdentityUnitOfWork>();
    }
}
