using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace SharedKernel.Api;

public static class EndpointDiscovery
{
    private static readonly HashSet<Type> _endpointTypes = [];

    /// <summary>
    /// Scans the given assembly for types implementing IEndpoint
    /// and stores them for later mapping.
    /// </summary>
    public static void AddEndpoints(Assembly assembly)
    {
        var endpointTypes = assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(SharedKernel.Api.IEndpoint).IsAssignableFrom(t));

        _endpointTypes.UnionWith(endpointTypes);
    }

    /// <summary>
    /// Instantiates and maps all discovered endpoints.
    /// </summary>
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        foreach (var endpointType in _endpointTypes)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpoint.Map(app);
        }
    }
}
