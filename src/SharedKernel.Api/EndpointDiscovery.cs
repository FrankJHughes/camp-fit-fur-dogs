using System.Reflection;
using Microsoft.AspNetCore.Routing;
using System.Collections.Concurrent;

namespace SharedKernel.Api;

public static class EndpointDiscovery
{
    private static readonly ConcurrentDictionary<Type, byte> _endpointTypes = new();

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
                typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
            _endpointTypes.TryAdd(type, 0);
    }

    /// <summary>
    /// Instantiates and maps all discovered endpoints.
    /// </summary>
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        foreach (var endpointType in _endpointTypes.Keys)
        {
            var endpoint = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpoint.Map(app);
        }
    }
}
