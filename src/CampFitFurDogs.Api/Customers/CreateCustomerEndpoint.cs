using CampFitFurDogs.Api.Validation;
using CampFitFurDogs.Application.Abstractions.Customers.CreateCustomer;
using CampFitFurDogs.Domain.Customers;
using FluentValidation;
using Frank.Abstractions;
using Frank.Api;

namespace CampFitFurDogs.Api.Customers;

public sealed class CreateCustomerEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers", async (
            CreateCustomerRequest request,
            IValidator<CreateCustomerRequest> validator,
            ICommandDispatcher dispatcher,
            CancellationToken ct) =>
        {
            // Syntactic validation (request-level)
            await request.Validate(validator, ct);

            // Hash password only if provided (local identity)
            string? passwordHash = null;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                passwordHash = PasswordHash.Create(request.Password).Value;
            }

            // Build command (semantic validation happens in command validator)
            var command = new CreateCustomerCommand(
                FirstName: request.FirstName,
                LastName: request.LastName,
                Email: request.Email,
                Phone: request.Phone,
                Password: passwordHash,
                ExternalAuthProviderId: null);

            var id = await dispatcher.DispatchAsync(command, ct);

            return Results.Created($"/api/customers/{id}", new { CustomerId = id });
        });
    }
}
