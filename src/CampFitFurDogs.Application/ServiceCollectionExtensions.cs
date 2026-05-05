using Microsoft.Extensions.DependencyInjection;

using SharedKernel.DependencyInjection;

namespace CampFitFurDogs.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
        => services.AddSharedKernel(
            [typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly]);
}
