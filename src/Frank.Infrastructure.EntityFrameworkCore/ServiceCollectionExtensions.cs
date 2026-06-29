using Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEntityFrameworkCoreInfrastructure<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        return services.AddFrankEntityFrameworkCoreUnitOfWork<TContext>();
    }
}
