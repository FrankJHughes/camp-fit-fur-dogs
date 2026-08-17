# Exception Handling

The CampFitFurDogs API uses a structured, platform-aligned exception-handling system that converts domain errors, validation failures, and infrastructure exceptions into consistent `ProblemDetails` responses. This ensures predictable error behavior across all endpoints and keeps the API layer thin and declarative.

## Overview

All exceptions flowing through the API pipeline are intercepted by specialized exception handlers. Each handler maps a known exception type to:

- an HTTP status code  
- a platform-defined error code  
- a standardized `ProblemDetails` response  

This approach provides uniform error semantics across vertical slices and prevents leakage of internal implementation details.

## Exception Handler Architecture

Exception handling is built on the platform’s `IExceptionHandler` abstraction and handler attributes. Each handler:

- identifies whether it can process a given exception  
- maps the exception to a platform error code  
- produces a structured `ProblemDetails` response  

Handlers remain small, focused, and aligned with the vertical slice they serve.

### Handler Registration

Handlers are registered through the API platform module:

```csharp
// In CampFitFurDogs.Api.Platform/ServiceCollectionExtensions.cs
public static IServiceCollection AddCampFitFurDogsApiPlatform(
    this IServiceCollection services,
    IConfiguration configuration)
{
    return services
        .AddCampFitFurDogsApplication()
        .AddCampFitFurDogsInfrastructure(configuration)
        .AddCampFitFurDogsApiExceptionHandlers();  // Registers all exception handlers
}
```

This keeps exception handling declarative and consistent with the platform’s hosting model.

## Built-in Exception Handlers

### Domain Exception Handler

Handles domain-level invariant violations, including:

- `InvalidFirstNameException`
- `InvalidLastNameException`
- `InvalidEmailException`
- `InvalidPhoneNumberException`
- `DomainException` (generic domain errors)

**Response:** `400 Bad Request`  
Domain exceptions always reflect business rule violations and never internal failures.

### Validation Exception Handler

Handles FluentValidation failures for incoming requests:

- aggregates validation errors by property  
- returns all field-level messages in a single response  
- supports UI clients that require per-field error mapping  

**Response:** `400 Bad Request`

Example:

```json
{
  "title": "Validation Error",
  "detail": "A validation error occurred. Please check the fields and try again.",
  "status": 400,
  "errors": {
    "name": ["Name is required", "Name must be at most 100 characters"],
    "breed": ["Breed is required"]
  }
}
```

### Bad Request Exception Handler

Handles malformed HTTP requests, including:

- invalid query parameters  
- model binding failures  
- type conversion errors  

**Response:** `400 Bad Request`

### Unexpected Exception Handler

Catches all unhandled exceptions:

- logs full details internally  
- returns a generic error message to avoid leaking sensitive information  
- ensures consistent fallback behavior  

**Response:** `500 Internal Server Error`

## Exception Response Structure

All handlers produce `ProblemDetails` responses following RFC 7807:

```csharp
public class ProblemDetails
{
    public string Title { get; set; }
    public string Detail { get; set; }
    public int Status { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
}
```

Validation handlers populate `Errors`; other handlers omit it.

## Error Codes

The platform defines standard error codes used across all handlers:

- `ErrorCode.ValidationFailed` — FluentValidation failure (400)  
- `ErrorCode.DomainError` — domain invariant violation (400)  
- `ErrorCode.BadRequest` — malformed HTTP request (400)  
- `ErrorCode.Unauthorized` — missing/invalid authentication (401)  
- `ErrorCode.Forbidden` — insufficient permissions (403)  
- `ErrorCode.NotFound` — resource not found (404)  
- `ErrorCode.InternalServerError` — unhandled exception (500)  

These codes ensure consistent error semantics across vertical slices.

## Adding Custom Exception Handlers

Application-specific exceptions can be handled by defining a new handler:

```csharp
namespace CampFitFurDogs.Api.ExceptionHandlers;

[ExceptionHandler(400)]
public sealed class DogNotFoundExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is DogNotFoundException;

    public IErrorCode GetErrorCode(Exception ex) => ErrorCode.NotFound;

    public ProblemDetails CreateProblemDetails(Exception ex)
        => new()
        {
            Title = "Dog Not Found",
            Detail = ex.Message,
            Status = StatusCodes.Status404NotFound,
            Type = "https://httpstatuses.com/404"
        };
}
```

Register the handler:

```csharp
services.AddExceptionHandler<DogNotFoundExceptionHandler>();
```

## Exception Handling Pipeline

1. Request reaches the endpoint  
2. Application or domain logic may throw exceptions  
3. Exception handler middleware intercepts the exception  
4. The appropriate handler processes it  
5. A `ProblemDetails` response is generated  
6. The client receives a structured error with the correct status code  

This pipeline ensures predictable behavior across all features.

## Best Practices

### Use Domain Exceptions
Throw domain-specific exceptions for invariant violations:

```csharp
if (string.IsNullOrWhiteSpace(name))
    throw new InvalidDogNameException("Dog name cannot be empty.");
```

### Provide Clear Error Messages
Messages should help users resolve the issue:

```csharp
throw new ArgumentException("Dog name must be between 2 and 100 characters.");
```

### Use FluentValidation for Structural Validation
Validators keep request validation consistent:

```csharp
public class RegisterDogCommandValidator : AbstractValidator<RegisterDogCommand>
{
    public RegisterDogCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters");
    }
}
```

### Avoid Leaking Internal Details
Unexpected exceptions should return generic messages while logging full details internally.

## Testing Exception Handlers

Handlers should be unit tested to verify correct mapping:

```csharp
[Fact]
public void ValidationExceptionHandler_MapsValidationErrors_Correctly()
{
    var handler = new ValidationExceptionHandler();
    var exception = new ValidationException("Name is required");

    var problemDetails = handler.CreateProblemDetails(exception);

    Assert.Equal(400, problemDetails.Status);
    Assert.Contains("validation", problemDetails.Title.ToLower());
}
```

## Source References

- `src/CampFitFurDogs/Api/ExceptionHandlers/*` — handler implementations  
- `src/Frank/Core/Application/Abstractions/Exceptions` — platform exception abstractions  
- `src/CampFitFurDogs/Application/Exceptions/*` — domain exception definitions  
