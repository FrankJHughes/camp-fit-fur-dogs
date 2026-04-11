using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Dogs.GetDogProfile;

namespace CampFitFurDogs.Api.Dogs;

public static class GetDogProfileEndpoint
{
    public static void MapGetDogProfile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (
            Guid id,
            Guid customerId,
            IQueryDispatcher dispatcher) =>
        {
            try
            {
                var query = new GetDogProfileQuery(id, customerId);
                var result = await dispatcher.Dispatch(query, CancellationToken.None);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.NotFound();
            }
        });
    }
}
