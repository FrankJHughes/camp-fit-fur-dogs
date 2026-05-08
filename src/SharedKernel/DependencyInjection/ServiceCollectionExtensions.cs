using System.Reflection;
using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedKernel(
        this IServiceCollection services,
        Assembly[] assemblies)
    {
        services.AddAutoRegistration(
            [.. assemblies, typeof(SharedKernel.AssemblyMarker).Assembly]);

        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }
}
