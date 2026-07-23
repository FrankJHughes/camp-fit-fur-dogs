using Frank.Identity.Application.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.EntityFrameworkCore.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankIdentityEntityFrameworkCoreUnitOfWork(this IServiceCollection services)
    {
        return services.AddScoped<IFrankIdentityUnitOfWork, FrankIdentityUnitOfWork>();
    }
}
