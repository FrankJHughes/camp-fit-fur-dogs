using Frank.Abstractions.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore.UnitOfWork;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEntityFrameworkCoreUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        return services.AddScoped<IUnitOfWork, EntityFrameworkCoreUnitOfWork<TContext>>();
    }
}
