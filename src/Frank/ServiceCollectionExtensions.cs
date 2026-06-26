using System.Reflection;
using FluentValidation;
using Frank.AutoRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrank(
        this IServiceCollection services,
        Assembly[] assemblies)
    {
        services.AddAutoRegistration(
            [.. assemblies, typeof(Frank.AssemblyMarker).Assembly]);

        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }
}
