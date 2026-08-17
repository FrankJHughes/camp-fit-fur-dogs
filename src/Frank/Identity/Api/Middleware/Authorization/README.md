# Identity API — Authorization Middleware

The **Authorization** folder contains the middleware responsible for enforcing
the Identity API’s authorization baseline:  
**all endpoints require an authenticated user unless explicitly marked
`[AllowAnonymous]`.**

This enforcement is intentionally lightweight, predictable, and free of domain
logic. It ensures that authorization behavior is consistent across the entire
Identity API surface and aligns with the purity rules defined in stories such as:

- US‑110 — Owner Login  
- US‑111 — Session Management  
- US‑133 — Account Lockout  
- US‑148 — Email Verification  

Authorization middleware operates *after* authentication and *before* endpoint
execution, forming a clean boundary between the HTTP pipeline and the Identity
application layer.

---

## Folder Structure

```
Authorization/
├── RequireAuthenticatedUserMiddleware.cs
└── ApplicationBuilderExtensions.cs
```

---

## RequireAuthenticatedUserMiddleware

This middleware enforces the rule that all Identity API endpoints require an
authenticated user unless explicitly marked with `[AllowAnonymous]`.

### Responsibilities

- Detect whether the current endpoint allows anonymous access  
- If anonymous → continue the pipeline  
- If not anonymous → verify `ICurrentUser.IsAuthenticated`  
- If unauthenticated → return HTTP 401 immediately  
- If authenticated → continue the pipeline  

### Design Principles

- **Purity** — No domain logic; only authorization enforcement  
- **Safety** — Unauthorized requests fail fast  
- **Predictability** — Anonymous access is opt‑in and explicit  
- **Minimalism** — Middleware performs only the required checks  

---

## ApplicationBuilderExtensions

Registers the authorization middleware into the ASP.NET Core pipeline.

### Responsibilities

- Adds `RequireAuthenticatedUserMiddleware` to the request pipeline  
- Ensures authorization enforcement is centralized and consistent  
- Keeps middleware registration isolated from endpoint logic  

### Contract

```csharp
app.UseFrankIdentityApiMiddlewareAuthorization();
```

### Notes

- Middleware runs before endpoint execution  
- Authentication must already be configured (session or OIDC)  
- Authorization behavior is uniform across all Identity endpoints  

---

## How Authorization Middleware Fits Into the Identity Architecture

```
[ Client Request ]
       ↓
[ Authentication ]
       ↓
[ Authorization Middleware ]
       ↓
[ Endpoint Execution ]
       ↓
[ Application Pipelines ]
```

This ensures:

- All identity endpoints operate inside a secure authorization boundary  
- Anonymous endpoints are explicitly declared  
- Authorization remains simple, predictable, and free of domain concerns  

---

## Summary

The Authorization folder provides:

- A single, predictable enforcement point for authenticated access  
- A clean middleware extension for pipeline registration  
- A purity‑aligned, minimal authorization layer for the Identity API  

Together, these components ensure that authorization behavior is consistent,
safe, and easy to reason about across the entire subsystem.

---
