using Microsoft.AspNetCore.Builder;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Domain.Customers;
using SharedKernel.Abstractions;
using SharedKernel.Api;

namespace CampFitFurDogs.Api.Customers;

public class CreateCustomerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers", async (
            CreateCustomerRequest request,
            ICommandDispatcher dispatcher) =>
        {
            var command = new CreateCustomerCommand(
                request.FirstName,
                request.LastName,
                request.Email, request.Phone,
                request.Password
            );

            var id = await dispatcher.DispatchAsync(command, CancellationToken.None);
            return Results.Created($"/api/customers/{id}", new { CustomerId = id });
        });
    }

    private static string FormatValidationMessage(string? paramName) =>
        paramName switch
        {
            "firstName" => "A first name is needed to create your account.",
            "lastName" => "A last name is needed to create your account.",
            _ => "Please check the information you provided and try again."
        };
}
