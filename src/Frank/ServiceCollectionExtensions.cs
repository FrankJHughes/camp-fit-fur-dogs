using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Frank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrank(
        this IServiceCollection services,
        Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }
}
