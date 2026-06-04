using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Frank.Abstractions;
using Frank.DependencyInjection;

namespace Frank.Infrastructure.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankEfCore<TContext>(
        this IServiceCollection services,
        Assembly[] assemblies)
        where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();

        services.AddAutoRegistration(assemblies);

        return services;
    }
}
