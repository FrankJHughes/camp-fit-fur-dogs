namespace CampFitFurDogs.Api.Dogs;

public static class DogEndpoints
{
    public static void MapDogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/dogs");

        group.MapRegisterDog();
        group.MapGetDogProfile();
    }
}
