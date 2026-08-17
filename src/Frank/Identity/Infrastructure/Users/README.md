# Identity Infrastructure — Users

The **Users** folder contains infrastructure components that expose information
about the currently authenticated user within the Identity subsystem.  
These components wrap ASP.NET Core’s `HttpContext.User` and provide a clean,
strongly typed abstraction for accessing user identity details inside
application services, handlers, and vertical slices.

This folder currently contains:

- A concrete implementation of `ICurrentUser`
- DI registration for the user accessor

---

## Purpose

The Users subsystem provides:

- A consistent way to access the authenticated user  
- Strongly typed identity values (`Guid` user ID, display name)  
- A safe wrapper around `HttpContext.User`  
- Scoped lifetime alignment with the HTTP request pipeline  

It ensures that application code does not need to interact directly with
ASP.NET Core’s claims APIs.

---

## Files

### **CurrentUser**

Implements `ICurrentUser`.

Responsibilities:

- Wraps `IHttpContextAccessor`  
- Exposes:
  - `IsAuthenticated`
  - `Id` (parsed from `ClaimTypes.NameIdentifier`)
  - `Name` (from `ClaimTypes.Name`)
- Provides helper methods for claim lookup  
- Returns `null` safely when claims or context are missing  

Used during:

- Authorization checks  
- Domain logic requiring the current user’s ID  
- Logging and auditing  
- Request‑scoped user resolution  

---

### **ServiceCollectionExtensions**

Registers the user accessor.

Responsibilities:

- Adds:
  - `ICurrentUser` → `CurrentUser` (scoped)
- Ensures each HTTP request receives its own `CurrentUser` instance  
- Aligns with the lifetime of `HttpContext` and `IHttpContextAccessor`  

Used during:

- Application startup  
- Identity subsystem initialization  

---

## Design Principles

The Users subsystem follows these principles:

- **Request‑scoped identity**  
  User information is tied to the current HTTP request.

- **Strong typing**  
  User ID is parsed into a `Guid?` instead of raw strings.

- **Minimal surface area**  
  Only exposes the identity information needed by application code.

- **Safe access**  
  Handles missing contexts, missing claims, and malformed values gracefully.

---

## Example Claims Principal

```text
ClaimTypes.NameIdentifier = "3f2c8e5b-9a1d-4c2f-8e3a-123456789abc"
ClaimTypes.Name = "Frank"
```

---

## Summary

The **Users** folder provides the infrastructure required to safely and
consistently access the authenticated user:

- Strongly typed user accessor  
- Scoped DI registration  
- Clean abstraction over `HttpContext.User`  

It ensures that user identity flows cleanly through the Identity subsystem and
into application logic.

