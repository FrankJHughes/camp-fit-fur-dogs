using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Customers.CreateCustomer;
namespace CampFitFurDogs.Api.Customers;

public static class CreateCustomerEndpoint
{
    public static void MapCreateCustomer(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers", async (
            CreateCustomerCommand cmd,
            ICommandDispatcher dispatcher) =>
        {
            var id = await dispatcher.Dispatch(cmd, CancellationToken.None);
            return Results.Created($"/api/customers/{id}", new { CustomerId = id });
        });
    }
}
