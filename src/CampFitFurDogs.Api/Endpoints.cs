using CampFitFurDogs.Api.Customers;
using CampFitFurDogs.Api.Dogs;

namespace CampFitFurDogs.Api;

public static class Endpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");
        group.MapCustomerEndpoints();
        group.MapDogEndpoints();
    }
}
