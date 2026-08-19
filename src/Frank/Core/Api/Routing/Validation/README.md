# Validation (API Layer)

The **Validation** subsystem provides FluentValidation‑based request validation
for Minimal API endpoints.  
It integrates seamlessly with ASP.NET Core’s endpoint filter pipeline and the
Frank.Core routing architecture, ensuring that request DTOs are validated
automatically before endpoint handlers execute.

This subsystem keeps endpoints clean, enforces consistent validation behavior,
and centralizes all validation logic in FluentValidation validators.  
It also emits structured observability diagnostics (US‑199) through the unified
`IRequestObservationContext`.

---

## Files

```text
Validation/
├── EndpointFilter.cs
├── RouteGroupBuilderExtensions.cs
└── RouteHandlerBuilderExtensions.cs
```

---

## EndpointFilter<TRequest>

`EndpointFilter<TRequest>` is a reusable Minimal API endpoint filter that
automatically validates request DTOs using FluentValidation and emits structured
observability events.

### Responsibilities

- Extracts the request DTO from the invocation context  
- Executes the corresponding FluentValidation validator  
- Emits validation start/end/failure events  
- Measures validation duration  
- Enriches the unified observability envelope via `AddMetadata`  
- Returns a structured `400 Bad Request` response on failure  
- Allows the endpoint handler to run only when validation succeeds  

### Why this matters

- Ensures consistent validation across all endpoints  
- Keeps endpoint handlers free of validation boilerplate  
- Integrates cleanly with ASP.NET Core’s filter pipeline  
- Provides API‑level validation observability (US‑199)  

---

## RouteGroupBuilderExtensions

`RouteGroupBuilderExtensions` adds automatic request validation to an entire
endpoint group.

### Responsibilities

- Scans endpoint method parameters to detect request DTOs  
- Checks whether a FluentValidation validator exists for that DTO  
- Automatically attaches the correct `EndpointFilter<TRequest>`  
- Skips endpoints with no validator  
- Injects logger + request‑scope observation context into the filter  

### Example

```csharp
app.MapRegisteredApiEndpoints("/api")
   .AddRequestValidation();
```

### Why this matters

- Enables group‑wide validation with a single call  
- Ensures every endpoint in the group is validated consistently  
- Avoids per‑endpoint configuration  
- Ensures observability is applied uniformly  

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

Validation also integrates with the Observations subsystem:

- emits structured validation diagnostics  
- enriches the request‑scope observation context  
- supports correlation‑aware logging and tracing  

---

## Design Principles

- **Automatic** — validation runs without manual wiring  
- **Centralized** — validators live in the application layer  
- **Minimal API‑friendly** — uses endpoint filters, not MVC attributes  
- **Composable** — supports both group‑level and per‑endpoint configuration  
- **Fail‑fast** — invalid requests never reach business logic  
- **Observable** — validation emits structured diagnostics (US‑199)  

---

## Summary

The Validation subsystem provides:

- Automatic FluentValidation execution  
- Group‑level and per‑endpoint configuration  
- A reusable endpoint filter  
- Structured observability events  
- Clean integration with Minimal APIs  

This folder ensures that all request DTOs entering the Camp Fit Fur Dogs API are
validated consistently, predictably, and observably.
