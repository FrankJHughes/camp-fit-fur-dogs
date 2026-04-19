using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Domain.Customers;

namespace CampFitFurDogs.Api.Customers;

public class CreateCustomerEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/customers", async (
            CreateCustomerCommand cmd,
            ICommandDispatcher dispatcher) =>
        {
            try
            {
                var id = await dispatcher.DispatchAsync(cmd, CancellationToken.None);
                return Results.Created($"/api/customers/{id}", new { CustomerId = id });
            }
            catch (EmailAlreadyExistsException)
            {
                return Results.Conflict(new { Error = "An account with this email already exists. You can sign in or use a different email." });
            }
            catch (ArgumentNullException ex)
            {
                return Results.BadRequest(new { Error = FormatValidationMessage(ex.ParamName) });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { Error = ex.Message.Split(" (Parameter")[0] });
            }
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
