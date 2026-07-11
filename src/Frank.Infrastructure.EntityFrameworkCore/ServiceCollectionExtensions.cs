using Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEntityFrameworkCoreInfrastructure(this IServiceCollection services)
    {
        return services.AddFrankIdentityUnitOfWork();
    }
}
