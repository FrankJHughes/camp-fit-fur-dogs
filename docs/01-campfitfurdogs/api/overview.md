# API Layer

The **CampFitFurDogs.Api** layer defines the production HTTP boundary of the Camp Fit Fur Dogs platform.  
It exposes all public endpoints, request/response DTOs, validators, exception handlers, hosting modules, and API‑specific DI wiring.  
The API layer is intentionally thin: it shapes the HTTP contract and delegates all business behavior to the Application and Domain layers.

The API is **no longer the composition root**.  
Startup, hosting, environment adaptation, and platform activation now live in **CampFitFurDogs.Host**, making the API assembly pure, host‑agnostic, and reusable.

---

## Organization

The API project follows the vertical‑slice structure used across the solution:

- `Endpoints/Dogs/RegisterDogEndpoint.cs` — POST handler for dog registration  
- `Endpoints/Dogs/GetDogEndpoint.cs` — GET handler for fetching a dog  
- `Endpoints/Dogs/ListDogsByCurrentUserEndpoint.cs` — GET handler for listing dogs by owner  
- `Endpoints/Dogs/EditDogEndpoint.cs` — PUT handler for updating a dog  
- `Endpoints/Dogs/RemoveDogEndpoint.cs` — DELETE handler for removing a dog  
- `Abstractions/Endpoints` — request/response DTOs and validators  
- `ExceptionHandlers/` — API‑level exception mapping  
- `HostingModules/` — environment‑specific configuration modules  
- `Platform/` — API‑specific DI registration  

The API assembly contains **no Program.cs**, **no hosting logic**, and **no environment adaptation**.

All startup orchestration is performed by the Host project.

---

## Endpoint Structure

Endpoints implement the platform’s `IEndpoint` interface and map themselves to routes.  
They bind request data, delegate to the application layer, and return typed responses.

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
            var command = new RegisterDogCommand(
                currentUser.Id!.Value,
                request.Name,
                request.Breed,
                DateOnly.Parse(request.DateOfBirth),
                request.Sex);

            var dogId = await dispatcher.DispatchAsync(command, CancellationToken.None);

            return Results.Created($"/dogs/{dogId}", new RegisterDogEndpointResponse(dogId));
        });
    }
}
```

### Endpoint Responsibilities

Endpoints must:

- bind and validate incoming request data  
- **emit request validation observability events**  
- translate requests into commands or queries  
- delegate all business logic to the application layer  
- return typed, structured responses  
- avoid domain or persistence logic entirely  

This keeps the API declarative and aligned with platform purity rules.

---

## Request Validation & Observability

FluentValidation validators are automatically invoked **before** endpoint handlers execute.  
Validation failures are converted into structured `ProblemDetails` responses by the exception‑handling pipeline.

```csharp
public class RegisterDogEndpointRequestValidator : AbstractValidator<RegisterDogEndpointRequest>
{
    public RegisterDogEndpointRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Breed).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}
```

### Validation Observability

The API layer emits structured observability events for all validation activity:

- **`api.validation.start`** — correlation ID, route, DTO type  
- **`api.validation.end`** — duration  
- **`api.validation.failed`** — validation errors  
- **`api.validation.exception`** — unexpected validator failures  

These events:

- occur **before** the Application layer receives the request  
- provide visibility into malformed or invalid client payloads  
- integrate with Frank.Core Observations  
- are activated by the Host project’s middleware pipeline  

Validation errors result in:

- `400 Bad Request`  
- field‑level error messages  
- consistent error formatting across all endpoints  

---

## Response Types

The API layer uses standard HTTP semantics:

- `201 Created` — successful resource creation  
- `200 OK` — successful queries and updates  
- `204 No Content` — successful deletions  
- `400 Bad Request` — validation or malformed request errors  
- `401 Unauthorized` — missing or invalid authentication  
- `403 Forbidden` — insufficient permissions  
- `404 Not Found` — resource does not exist  
- `500 Internal Server Error` — unhandled exceptions  

All error responses are produced through the platform’s exception‑handling system.

---

## Platform Integration

The API layer integrates with Frank platform modules for:

- routing and endpoint grouping  
- exception handling and `ProblemDetails` formatting  
- authentication and session management  
- **request validation observability**  
- structured logging and tracing  

The API layer does **not** configure hosting, environment adaptation, or middleware.  
Those responsibilities belong to the Host project.

---

## Host Layer Extraction

The API assembly previously contained:

- Program.cs  
- Hosting helpers  
- Environment adaptation logic  

These have been extracted into **CampFitFurDogs.Host**.

The Host project now:

- configures the WebApplicationBuilder  
- applies hosting modules  
- registers platform services  
- activates API endpoints  
- configures middleware  
- runs the application  

This extraction makes the API assembly:

- pure  
- reusable  
- host‑agnostic  
- environment‑independent  

---

## Summary

The API layer is a thin, declarative HTTP boundary that:

- exposes product capabilities through endpoints  
- delegates all business logic to the application layer  
- relies on the platform for routing, validation, and error handling  
- emits structured validation observability events  
- remains host‑agnostic after the extraction of Program.cs  

It is the boundary between external clients and internal business logic — clean, predictable, and fully aligned with vertical‑slice architecture.
