# Service Registration

The application layer services are registered through a fluent extension method that composes all handlers, validators, and persistence abstractions. This keeps the vertical slice cohesive and ensures that all Dogs‑related behavior is discoverable and wired automatically.

## The Registration Call

From `Program.cs`:

```csharp
services.AddCampFitFurDogsApiPlatform(configuration);
```

This single call chains three subsystem registrations:

```csharp
public static IServiceCollection AddCampFitFurDogsApiPlatform(
    this IServiceCollection services, IConfiguration configuration)
{
    return services
        .AddCampFitFurDogsApplication()
        .AddCampFitFurDogsInfrastructure(configuration)
        .AddCampFitFurDogsApiExceptionHandlers();
}
```

The platform method forms the backbone of the product’s dependency graph.

## Application Services

`AddCampFitFurDogsApplication()` registers all application‑layer components:

- command handlers (e.g., `RegisterDogCommandHandler`)  
- query handlers (e.g., `GetDogQueryHandler`)  
- FluentValidation validators discovered via assembly scanning  
- the Dogs vertical slice via `AddApplicationDogs()`  

This ensures that CQRS handlers and validators are available to the dispatch pipeline without manual registration.

## Infrastructure Services

`AddCampFitFurDogsInfrastructure(configuration)` registers persistence and external integrations:

- EF Core database context  
- unit of work implementation  
- reader and writer abstractions (`IRegisterDogWriter`, `IGetDogReader`, etc.)  
- database connection configuration from `appsettings.json`  

Infrastructure services provide the concrete implementations required by the application layer’s abstractions.

## Exception Handlers

`AddCampFitFurDogsApiExceptionHandlers()` registers all API‑level exception handlers:

- domain exception handlers  
- validation exception handlers  
- not‑found handlers  
- unexpected exception handlers  

These handlers convert exceptions into RFC 7807 `ProblemDetails` responses, ensuring consistent error semantics across vertical slices.

## Order Matters

The order of registration is intentional:

1. **Application layer** — handlers, validators, and abstractions  
2. **Infrastructure layer** — concrete implementations for application dependencies  
3. **Exception handlers** — able to catch both application and infrastructure exceptions  

This ordering ensures that all dependencies are available when the API pipeline executes.

## Summary

Service registration in the CampFitFurDogs application layer provides:

- automatic discovery of CQRS handlers and validators  
- clean separation between application and infrastructure  
- consistent exception handling across all vertical slices  
- a single composition root for the entire product  

It keeps the application layer cohesive, predictable, and aligned with the vertical‑slice architecture.

