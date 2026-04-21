using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace SharedKernel.Api;

public static class EndpointDiscovery
{
    private static readonly List<Type> _endpointTypes = new();

    /// <summary>
    /// Scans the given assembly for types implementing IEndpoint
    /// and stores them for later mapping.
    /// </summary>
    public static void RegisterEndpointsFromAssembly(Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(IEndpoint).IsAssignableFrom(t));

        _endpointTypes.AddRange(types);
    }

    /// <summary>
    /// Instantiates and maps all discovered endpoints.
    /// </summary>
    public static void MapDiscoveredEndpoints(IEndpointRouteBuilder app)
    {
        foreach (var endpointType in _endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpoint.Map(app);
        }
    }
}
