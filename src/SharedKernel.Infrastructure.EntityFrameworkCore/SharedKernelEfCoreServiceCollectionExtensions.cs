using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using SharedKernel.DependencyInjection;

namespace SharedKernel.Infrastructure.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedKernelEfCore<TContext>(
        this IServiceCollection services,
        Assembly[] assemblies)
        where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();

        services.AddAutoRegistration(assemblies);

        return services;
    }
}
