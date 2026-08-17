# Exception Handlers

The **ExceptionHandlers** folder contains all API‑level exception handlers used by
the Camp Fit Fur Dogs platform.  
Each handler implements `IExceptionHandler` and is automatically discovered and
registered through the Frank.Core infrastructure exception‑handling pipeline.

Handlers in this folder translate domain, validation, authentication, and
infrastructure errors into consistent `ProblemDetails` responses.

---

## Folder Contents

```
ExceptionHandlers/
│
├── BadConfigurationExceptionHandler.cs
├── BadRequestExceptionHandler.cs
├── DomainExceptionHandler.cs
├── DuplicateEmailExceptionHandler.cs
├── ExternalAuthProviderExceptionHandler.cs
├── UnexpectedExceptionHandler.cs
├── UserIdClaimNotFoundExceptionHandler.cs
├── UserNotAuthenticatedExceptionHandler.cs
├── ValidationExceptionHandler.cs
└── ServiceCollectionExtensions.cs
```

---

## Handler Overview

### BadConfigurationExceptionHandler  
Handles misconfiguration errors such as missing or invalid application settings.  
Returns **500 Internal Server Error** with `ErrorCode.BadConfiguration`.

### BadRequestExceptionHandler  
Handles client‑side request errors (invalid input, malformed requests).  
Returns **400 Bad Request** with `ErrorCode.BadRequest`.

### DomainExceptionHandler  
Handles domain‑level invariant violations and identity domain errors.  
Returns **400 Bad Request** with `ErrorCode.DomainError`.

### DuplicateEmailExceptionHandler  
Handles attempts to register or update an account with an email already in use.  
Returns **409 Conflict** with `ErrorCode.DuplicateEmail`.

### ExternalAuthProviderExceptionHandler  
Handles failures from external authentication providers (OIDC/OAuth).  
Returns **502 Bad Gateway** with `ErrorCode.ExternalAuthProviderFailure`.

### UnexpectedExceptionHandler  
Catch‑all fallback handler for any unhandled exception.  
Returns **500 Internal Server Error** with `ErrorCode.Unexpected`.

### UserIdClaimNotFoundExceptionHandler  
Handles missing user ID claim during authentication or identity resolution.  
Returns **401 Unauthorized** with `ErrorCode.InvalidUserIdentity`.

### UserNotAuthenticatedExceptionHandler  
Handles attempts to access protected resources without authentication.  
Returns **401 Unauthorized** with `ErrorCode.UserNotAuthenticated`.

### ValidationExceptionHandler  
Handles FluentValidation failures and returns structured field‑level errors.  
Returns **400 Bad Request** with `ErrorCode.ValidationFailed`.

---

## Registration

All handlers are automatically registered using:

```csharp
services.AddCampFitFurDogsApiExceptionHandlers();
```

This extension:

- Scans the assembly containing `AssemblyMarker`
- Filters to include only types under the `CampFitFurDogs.Api.ExceptionHandlers` namespace
- Registers all `IExceptionHandler` implementations with Frank.Core

---

## Design Principles

Exception handlers follow these principles:

- **Consistency** — all errors return structured `ProblemDetails` responses  
- **Separation of concerns** — domain, validation, identity, and infrastructure errors are isolated  
- **Predictability** — each handler maps to a specific `ErrorCode` and HTTP status  
- **Discoverability** — handlers are automatically registered via assembly scanning  
- **Priority ordering** — `[ExceptionHandler(order)]` determines execution precedence  

---

## Summary

The ExceptionHandlers folder defines the complete error‑translation layer for the
Camp Fit Fur Dogs API:

- 10 specialized handlers  
- Full coverage of domain, validation, identity, configuration, and unexpected errors  
- Unified registration and discovery  
- Consistent, client‑friendly error responses  

This structure ensures that all exceptions are handled gracefully and predictably
across the entire API surface.

