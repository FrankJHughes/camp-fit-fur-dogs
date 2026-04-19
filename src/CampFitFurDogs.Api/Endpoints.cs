using System.Reflection;

namespace CampFitFurDogs.Api;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        var endpointTypes = typeof(IEndpoint).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                        && typeof(IEndpoint).IsAssignableFrom(t))
            .OrderBy(t => t.FullName);

        foreach (var type in endpointTypes)
        {
            var method = type.GetMethod(
                "Map",
                BindingFlags.Public | BindingFlags.Static,
                new[] { typeof(IEndpointRouteBuilder) });

            method?.Invoke(null, new object[] { group });
        }
    }
}
