using System.Reflection;
using Frank.Abstractions;
using Frank.AutoRegistration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
