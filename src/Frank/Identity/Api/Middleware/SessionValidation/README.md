# Identity API — Session Validation Middleware

The **SessionValidation** folder contains the middleware and authentication
components responsible for validating session cookies, enforcing session domain
invariants, and attaching authenticated principals to the ASP.NET Core request
pipeline.

Although these components live in the Identity API assembly, they are **not
limited to identity endpoints**.  
They reside here because they depend on Identity abstractions such as:

- `ISessionTokenGenerator`
- `IGetSessionReader`
- `IGetUserByIdReader`
- `ICurrentUser` (resolved later by the Identity subsystem)

This middleware forms the backbone of the **Session authentication scheme** used
throughout the API surface hosted in this assembly.

---

## Folder Structure

```
Sessions/
├── SessionValidationMiddleware.cs
├── SessionAuthenticationHandler.cs
└── ApplicationBuilderExtensions.cs
```

---

## SessionValidationMiddleware

Performs the actual validation of the session cookie and constructs the
authenticated principal.

### Responsibilities

- **Skip validation** for known anonymous endpoints:
  - `/api/identity/callback`
  - `/api/identity/login-url`
  - `/api/identity/logout`
- **Read** plaintext session cookie
- **Hash** the cookie using `ISessionTokenGenerator`
- **Load** the session using `IGetSessionReader`
- **Enforce domain invariants**:
  - Session must exist  
  - Session must not be revoked  
  - Session must not be expired  
- **Load the owning user** using `IGetUserByIdReader`
- **Attach authenticated principal** to `HttpContext.User`
- **Populate `CurrentOwnerId`** in `HttpContext.Items`

### Design Principles

- **Purity** — No identity provider tokens or claims beyond what the domain allows  
- **Safety** — Invalid sessions delete the cookie and throw `SessionNotFoundException`  
- **Minimalism** — Only session validation; no authorization or endpoint logic  
- **Delegation** — Domain invariants enforced by the session model  

---

## SessionAuthenticationHandler

Integrates the session principal created by the validation middleware with
ASP.NET Core’s authentication system.

### Responsibilities

- Return `AuthenticateResult.Success` if `HttpContext.User` is already authenticated  
- Return `AuthenticateResult.NoResult` otherwise  
- Never perform validation — that belongs exclusively to the middleware  
- Allow authorization policies to rely on the “Session” scheme  

### Why It Exists

ASP.NET Core requires an authentication handler for each scheme.  
This handler simply reflects the principal created by `SessionValidationMiddleware`.

---

## ApplicationBuilderExtensions

Registers the session validation middleware into the ASP.NET Core pipeline.

### Responsibilities

- Adds `SessionValidationMiddleware` to the pipeline  
- Ensures session validation runs before authentication and authorization  
- Keeps registration isolated from endpoint logic  

### Contract

```csharp
app.UseFrankIdentityApiMiddlewareSessionValidation();
```

---

## How Session Validation Fits Into the API Architecture

```
[ Client Request ]
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

- Session cookies are validated early  
- Authenticated principals are available to authorization  
- Invalid sessions fail fast and safely  
- Identity purity rules are preserved  

---

## Summary

The SessionValidation folder provides:

- **SessionValidationMiddleware** — validates cookies, loads sessions, enforces invariants  
- **SessionAuthenticationHandler** — integrates validated principals with ASP.NET Core  
- **ApplicationBuilderExtensions** — registers the middleware cleanly  

Together, these components form the complete session authentication pipeline for
the API surface hosted in this assembly.

---
