using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Dogs.RegisterDog;

namespace CampFitFurDogs.Api.Dogs;

public static class RegisterDogEndpoint
{
    public static void MapRegisterDog(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            RegisterDogCommand cmd,
            ICommandDispatcher dispatcher) =>
        {
            try
            {
                var id = await dispatcher.Dispatch(cmd, CancellationToken.None);
                return Results.Created($"/api/dogs/{id}", new { DogId = id });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new
                {
                    Error = ex.Message.Split(" (Parameter")[0]
                });
            }
        });
    }
}
