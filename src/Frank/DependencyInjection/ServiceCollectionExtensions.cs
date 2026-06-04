using System.Reflection;
using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Frank.DependencyInjection;

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
