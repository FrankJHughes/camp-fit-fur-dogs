# Validation

The **Validation** folder provides FluentValidation‑based request validation for
Minimal API endpoints.  
It integrates seamlessly with ASP.NET Core’s endpoint filter pipeline and the
Frank.Core routing architecture, ensuring that request DTOs are validated
automatically before endpoint handlers execute.

This subsystem keeps endpoints clean, enforces consistent validation behavior,
and centralizes all validation logic in FluentValidation validators.

---

## Files

```
Validation/
├── EndpointFilter.cs
├── RouteGroupBuilderExtensions.cs
└── RouteHandlerBuilderExtensions.cs
```

---

## EndpointFilter\<TRequest>

`EndpointFilter<TRequest>` is a reusable Minimal API endpoint filter that
automatically validates request DTOs using FluentValidation.

### Responsibilities

- Extracts the request DTO from the endpoint invocation context  
- Executes the corresponding FluentValidation validator  
- Throws `ValidationException` on failure  
- Allows the endpoint handler to run only when validation succeeds  

### Why this matters

- Ensures consistent validation across all endpoints  
- Keeps endpoint handlers free of validation boilerplate  
- Integrates cleanly with ASP.NET Core’s filter pipeline  

---

## RouteGroupBuilderExtensions

`RouteGroupBuilderExtensions` adds automatic request validation to an entire
endpoint group.

### Responsibilities

- Scans endpoint method parameters to detect request DTOs  
- Checks whether a FluentValidation validator exists for that DTO  
- Automatically attaches the correct `EndpointFilter<TRequest>`  
- Skips endpoints with no validator  

### Example

```csharp
app.MapRegisteredApiEndpoints("/api")
   .AddRequestValidation();
```

### Why this matters

- Enables group‑wide validation with a single call  
- Ensures every endpoint in the group is validated consistently  
- Avoids per‑endpoint configuration  

---

## RouteHandlerBuilderExtensions

`RouteHandlerBuilderExtensions` provides a per‑endpoint way to attach validation.

### Responsibilities

- Adds a specific `EndpointFilter<TRequest>` to a single route handler  
- Useful for endpoints that require explicit validation control  

### Example

```csharp
api.MapPost("/dogs", HandleAsync)
   .WithValidation<CreateDogRequest>();
```

### Why this matters

- Allows fine‑grained validation when needed  
- Complements the group‑level validation extension  

---

## How Validation Fits Into the Architecture

Validation is part of the unified endpoint pipeline created by Frank.Core:

- **Discovery** — endpoints are found automatically  
- **Registration** — endpoints are added to DI automatically  
- **Mapping** — endpoints are mapped automatically  
- **Filtering** — validation runs automatically before handlers  

This keeps vertical slices clean and ensures validation is consistent across the
entire API surface.

---

## Design Principles

- **Automatic** — validation runs without manual wiring  
- **Centralized** — validators live in the application layer  
- **Minimal API‑friendly** — uses endpoint filters, not MVC attributes  
- **Composable** — supports both group‑level and per‑endpoint configuration  
- **Fail‑fast** — invalid requests never reach business logic  

---

## Summary

The Validation subsystem provides:

- Automatic FluentValidation execution  
- Group‑level and per‑endpoint configuration  
- A reusable endpoint filter  
- Clean integration with Minimal APIs  

This folder ensures that all request DTOs entering the Camp Fit Fur Dogs API are
validated consistently and predictably.
