# AuthenticatedUserService Guide

`AuthenticatedUserService` provides the **current authenticated user’s ID** to the Application and API layers.  
It is the concrete implementation of `ICurrentUser` and is the **only allowed mechanism** for retrieving the authenticated Owner’s identity.

This service is part of the **Infrastructure** layer and is registered automatically via Frank’s auto‑registration.

---

# Purpose

`AuthenticatedUserService` bridges the gap between:

- **ASP.NET Core authentication** (ClaimsPrincipal)  
- **Application layer identity requirements** (`ICurrentUser`)  

It provides a **safe, validated, exception‑driven** way to access the authenticated user’s ID.

It is used by:

- API endpoints (indirectly, via dispatchers)  
- Application command handlers  
- Application query handlers  
- Domain event handlers (if needed)  
- Infrastructure components that require the current user  

---

# Implementation

````csharp
using System.Security.Claims;
using CampFitFurDogs.Application.Exceptions;
using Frank.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CampFitFurDogs.Infrastructure.Identity;

public sealed class AuthenticatedUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid Id
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UserNotAuthenticatedException();

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst("sub")?.Value;

            if (id is null)
                throw new UserIdClaimNotFoundException();

            return Guid.Parse(id);
        }
    }
}
````

---

# Behavior

## 1. Authentication Required  
If the request is not authenticated:

- `UserNotAuthenticatedException` is thrown  
- API layer converts this to a **401 Unauthorized** ProblemDetails response  
- No handler logic executes  

## 2. Claim Requirements  
The service requires **one** of the following claims:

- `ClaimTypes.NameIdentifier`  
- `"sub"`  

If neither is present:

- `UserIdClaimNotFoundException` is thrown  
- API layer converts this to **401 Unauthorized**  

## 3. ID Format  
The claim value must be a **valid GUID**.  
If malformed, `Guid.Parse` throws — intentionally.

---

# Layer Placement

| Layer | Allowed? | Reason |
|-------|----------|--------|
| **Api** | ✔ | Endpoints may request `ICurrentUser` |
| **Application** | ✔ | Handlers depend on `ICurrentUser` |
| **Domain** | ✘ | Domain must not know about HTTP or claims |
| **Infrastructure** | ✔ | Implementation lives here |
| **Frank** | ✘ | Not cross‑cutting across products |

---

# Exceptions

| Exception | Meaning | HTTP Result |
|----------|----------|-------------|
| `UserNotAuthenticatedException` | No authenticated principal | 401 |
| `UserIdClaimNotFoundException` | Missing required claim | 401 |
| `FormatException` | Malformed GUID claim | 401 |

All exceptions flow through **Frank’s error boundary**.

---

# Security Considerations

- No user data is stored  
- No tokens are accessed  
- No cookies are read directly  
- Only validated claims are used  
- No fallback to email or username  
- No implicit identity resolution  

This aligns with:

- Security Governance  
- Identity Mapping Governance  
- Session Management Governance  

---

# Integration With Session Validation

When session validation middleware is implemented:

1. Middleware reads the session cookie  
2. Middleware resolves the Owner ID  
3. Middleware creates a `ClaimsPrincipal` with:  
   - `ClaimTypes.NameIdentifier = <OwnerId>`  
4. `AuthenticatedUserService` reads that claim  
5. Application handlers receive the correct Owner ID  

This ensures:

- No handler touches cookies  
- No handler touches HttpContext  
- No handler touches session storage  

---

# Usage in Application Handlers

````csharp
public sealed class RegisterDogCommandHandler : ICommandHandler<RegisterDogCommand, RegisterDogResult>
{
    private readonly ICurrentUser _currentUser;

    public RegisterDogCommandHandler(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<RegisterDogResult> Handle(RegisterDogCommand command, CancellationToken ct)
    {
        var ownerId = _currentUser.Id; // validated, guaranteed

        ...
    }
}
````

Handlers **must not** parse claims directly.

---

# Usage in API Endpoints

Endpoints **never** read claims directly.

They rely on:

- `ICurrentUser`  
- Dispatchers  
- Session middleware  

This preserves **API Endpoint Purity**.

---

# Testing

## API Tests  
Use the test factory’s fake authentication handler:

````csharp
client.WithAuthenticatedUser(ownerId);
````

## Application Tests  
Mock `ICurrentUser`:

````csharp
_currentUser.Id.Returns(ownerId);
````

## Infrastructure Tests  
Do not test this service — it is covered by API tests.

---

# Summary

`AuthenticatedUserService` is the **single source of truth** for the authenticated Owner ID.  
It enforces:

- Authentication  
- Claim presence  
- Claim correctness  
- Layer purity  
- Security boundaries  

It is a critical part of the authentication and session architecture.

