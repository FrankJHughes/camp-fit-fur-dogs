# Platform

The **Platform** folder contains composition‑level wiring for the Camp Fit Fur Dogs API.  
Where the Application, Infrastructure, and API layers each define their own services,  
the Platform layer brings them together into a unified, ready‑to‑run API surface.

This folder exists to keep `Program.cs` minimal, declarative, and free of cross‑layer
registration details.

---

## Files

```
Platform/
└── ServiceCollectionExtensions.cs
```

---

## ServiceCollectionExtensions.cs

Provides a single extension method that registers the entire Camp Fit Fur Dogs API
platform in one call.

### Responsibilities

- Compose the **Application** layer  
- Compose the **Infrastructure** layer  
- Register all **API exception handlers**  
- Provide a clean, single entry point for API startup

### Registration Flow

Calling:

```csharp
services.AddCampFitFurDogsApiPlatform(configuration);
```

performs the following:

1. **Application Layer**  
   Registers CQRS handlers, validators, domain services, and application‑level abstractions.

2. **Infrastructure Layer**  
   Registers EF Core, repositories, external integrations, and configuration‑driven services.

3. **API Exception Handlers**  
   Registers all `IExceptionHandler` implementations under  
   `CampFitFurDogs.Api.ExceptionHandlers`.

This ensures the entire platform is initialized consistently and predictably.

---

## Design Principles

The Platform layer follows these principles:

- **Composition over configuration** — platform wiring is centralized and declarative  
- **Separation of concerns** — startup code does not know about individual layers  
- **Minimalism** — `Program.cs` stays clean and focused  
- **Extensibility** — new layers or cross‑cutting concerns can be added here without
  touching the rest of the system

---

## Summary

The Platform folder defines the top‑level composition for the Camp Fit Fur Dogs API:

- One extension method  
- Three layers composed  
- Clean startup  
- Predictable initialization

This structure ensures the API boots with a fully assembled platform while keeping
startup code elegant and maintainable.

