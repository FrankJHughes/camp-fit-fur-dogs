# API Layer

The API layer is the composition root of the CampFitFurDogs product. It defines the HTTP surface, transforms requests into application commands and queries, and delegates all business behavior to the application and domain layers. The API remains intentionally thin, relying on the Frank platform for hosting, routing, exception handling, authentication, and environment adaptation.

## Organization

The API project follows the vertical-slice structure used across the solution:

- `Endpoints/Dogs/RegisterDogEndpoint.cs` — POST handler for dog registration  
- `Endpoints/Dogs/GetDogEndpoint.cs` — GET handler for fetching a dog  
- `Endpoints/Dogs/ListDogsEndpoint.cs` — GET handler for listing dogs by owner  
- `Endpoints/Dogs/EditDogEndpoint.cs` — PUT handler for updating a dog  
- `Endpoints/Dogs/RemoveDogEndpoint.cs` — DELETE handler for removing a dog  
- `Platform/` — API-level middleware, exception handlers, and configuration  
- `Helpers/Hosting.cs` — hosting modules and environment-aware startup helpers  
- `Program.cs` — application bootstrap, module loading, and DI setup  

Each endpoint belongs to a vertical slice and should remain cohesive with its corresponding application, domain, and infrastructure components.

## Endpoint Structure

Endpoints implement the platform’s `IEndpoint` interface and map themselves to routes. They bind request data, delegate to the application layer, and return typed responses.

```csharp
public class RegisterDogEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapPost("/dogs", async (
            RegisterDogEndpointRequest request,
            [FromServices] ICurrentUser currentUser,
            [FromServices] ICommandDispatcher dispatcher) =>
        {
            // Transform request to command
            var command = new RegisterDogCommand(
                currentUser.Id!.Value,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            // Dispatch through CQRS pipeline
            var dogId = await dispatcher.DispatchAsync(command, CancellationToken.None);

            // Return response
            return Results.Created($"/dogs/{dogId}", new RegisterDogEndpointResponse(dogId));
        });
    }
}
```

### Endpoint Responsibilities

Endpoints should:

- bind and validate incoming request data  
- translate requests into commands or queries  
- delegate all business logic to the application layer  
- return typed, structured responses  
- avoid domain or persistence logic entirely  

This keeps the API layer declarative and aligned with the platform’s purity rules.

## Request Validation

FluentValidation validators are automatically invoked before endpoint handlers execute. Validation failures are converted into structured `ProblemDetails` responses by the exception-handling pipeline.

```csharp
public class RegisterDogCommandValidator : AbstractValidator<RegisterDogCommand>
{
    public RegisterDogCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        // etc.
    }
}
```

Validation errors result in:

- `400 Bad Request`  
- field-level error messages  
- consistent error formatting across all endpoints  

## Response Types

The API layer uses standard HTTP semantics:

- `201 Created` — successful resource creation (with Location header)  
- `200 OK` — successful queries and updates  
- `204 No Content` — successful deletions  
- `400 Bad Request` — validation or malformed request errors  
- `401 Unauthorized` — missing or invalid authentication  
- `403 Forbidden` — insufficient permissions  
- `404 Not Found` — resource does not exist  
- `500 Internal Server Error` — unhandled exceptions  

All error responses are produced through the platform’s exception-handling system.

## Platform Integration

The API layer integrates with Frank platform modules for:

- hosting and environment adaptation  
- authentication and session management  
- exception handling and `ProblemDetails` formatting  
- routing and endpoint grouping  
- observability and structured logging  

This ensures consistent behavior across environments and keeps product code focused on business capabilities.

## Summary

The API layer is a thin, declarative composition surface that:

- exposes product capabilities through HTTP  
- delegates all business logic to the application layer  
- relies on the platform for hosting, routing, validation, and error handling  
- maintains strict vertical-slice cohesion  

It is the entry point for all product interactions and the boundary between external clients and internal business logic.

