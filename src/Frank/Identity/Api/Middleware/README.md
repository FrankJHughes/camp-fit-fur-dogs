# Identity API — Middleware

The **Middleware** folder contains all cross‑cutting pipeline components used by
the API surface hosted in this assembly.  
Although these middleware components live in the Identity API project, they are
**not limited to identity endpoints**.  
They reside here because they depend on Identity abstractions such as:

- `ICurrentUser`
- `ISessionTokenGenerator`
- `IGetSessionReader`
- `IGetUserByIdReader`
- Identity’s correlation + observation pipeline

These middleware layers form the backbone of the API’s runtime behavior:
authorization, observability, and session authentication.

---

## Folder Structure

```
Middleware/
├── Authorization/
│   ├── RequireAuthenticatedUserMiddleware.cs
│   └── ApplicationBuilderExtensions.cs
│
├── Observations/
│   ├── ObservationInstrumentationMiddleware.cs
│   └── ApplicationBuilderExtensions.cs
│
└── Sessions/
    ├── SessionValidationMiddleware.cs
    ├── SessionAuthenticationHandler.cs
    └── ApplicationBuilderExtensions.cs
```

---

# Authorization Middleware

Ensures that **all endpoints require an authenticated user** unless explicitly
marked `[AllowAnonymous]`.

### Components

#### RequireAuthenticatedUserMiddleware
- Checks endpoint metadata for `IAllowAnonymous`
- If anonymous → continue pipeline  
- If not anonymous → require `ICurrentUser.IsAuthenticated`
- Returns **401 Unauthorized** for unauthenticated requests

#### ApplicationBuilderExtensions
Registers the authorization middleware:

```csharp
app.UseFrankIdentityApiMiddlewareAuthorization();
```

### Why It Lives Here
It depends on Identity’s `ICurrentUser` abstraction.

---

# Observations Middleware

Provides **unified, structured telemetry** for every HTTP request.

### Components

#### ObservationInstrumentationMiddleware
- Propagates or generates correlation IDs  
- Emits begin/complete/error trace events  
- Measures request duration  
- Increments request + error counters  
- Reports exceptions to `IErrorBoundaryObserver`  
- Enriches telemetry with user ID (when authenticated)

#### ApplicationBuilderExtensions
Registers the observability middleware:

```csharp
app.UseFrankIdentityApiMiddlewareObservations();
```

### Why It Lives Here
It depends on Identity’s user‑resolution pipeline (`ICurrentUser`) and correlation context.

---

# Sessions Middleware

Implements the **Session authentication scheme** used across the API surface.

### Components

#### SessionValidationMiddleware
Performs the actual session validation:
- Reads plaintext session cookie  
- Hashes cookie using `ISessionTokenGenerator`  
- Loads session via `IGetSessionReader`  
- Enforces domain invariants (not revoked, not expired)  
- Loads user via `IGetUserByIdReader`  
- Attaches authenticated principal to `HttpContext.User`  
- Populates `CurrentOwnerId` in `HttpContext.Items`

Invalid sessions delete the cookie and throw `SessionNotFoundException`.

#### SessionAuthenticationHandler
Integrates validated principals with ASP.NET Core authentication:
- Returns `AuthenticateResult.Success` if `HttpContext.User` is already authenticated  
- Returns `AuthenticateResult.NoResult` otherwise  
- Performs **no validation** — that belongs to the middleware

#### ApplicationBuilderExtensions
Registers the session validation middleware:

```csharp
app.UseFrankIdentityApiMiddlewareSessionValidation();
```

### Why It Lives Here
It depends on Identity’s session and user readers, and token hashing.

---

# Middleware Pipeline Overview

```
[ Client Request ]
       ↓
[ Observations Middleware ]
       ↓
[ SessionValidationMiddleware ]
       ↓
[ SessionAuthenticationHandler ]
       ↓
[ Authorization Middleware ]
       ↓
[ Endpoint Execution ]
       ↓
[ Application Pipelines ]
```

This ensures:

- Every request is traced, timed, correlated, and observed  
- Session cookies are validated early and safely  
- Authenticated principals are available to authorization  
- Authorization is consistent and predictable  
- Identity purity rules are preserved end‑to‑end  

---

# Summary

The Middleware folder provides three cross‑cutting subsystems:

### Authorization
Predictable enforcement of authenticated access.

### Observations
Unified telemetry for tracing, metrics, correlation, and error reporting.

### Sessions
Full session authentication pipeline: validation → principal → handler.

Together, these middleware layers form the operational backbone of the API
surface hosted in this assembly, while remaining clean, pure, and free of domain
logic.

---
