using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedKernel(
        this IServiceCollection services,
        Assembly[] assemblies)
        => services.AddAutoRegistration(
            [.. assemblies, typeof(SharedKernel.AssemblyMarker).Assembly]);
}
