using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrankValidators(
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
