# Identity Infrastructure

The **Infrastructure** layer contains all external‑facing, environment‑dependent
components that support the Identity subsystem.  
These components integrate with ASP.NET Core, Auth0, OIDC, HTTP, logging, and
runtime configuration.  
Infrastructure is intentionally thin, stateless, and implementation‑focused,
providing concrete adapters for abstractions defined in the Application layer.

This folder currently includes:

- Auth0 OIDC integration  
- User accessor infrastructure  
- Audit logging  
- Settings binding  
- Assembly marker for scanning and registration  

---

## Purpose

The Infrastructure layer provides:

- Concrete implementations of Identity abstractions  
- Integration with external systems (Auth0, HTTP, ASP.NET Core)  
- Configuration binding and validation  
- Request‑scoped user resolution  
- Structured audit logging  
- DI registration for all infrastructure components  

It ensures that the Identity subsystem remains clean, testable, and environment‑agnostic.

---

## Subfolders

### **Auth0**

Contains all OIDC integration components:

- Token exchange client  
- Token validator (JWKS, issuer, audience, signature, lifetime)  
- UserInfo client  
- DI registration for OIDC + audit logging  
- `OidcSettings` binding and validation  

Implements the full Auth0 OIDC vertical slice.

---

### **Users**

Contains infrastructure for accessing the authenticated user:

- `CurrentUser` — wraps `HttpContext.User`  
- Exposes `Id`, `Name`, and `IsAuthenticated`  
- Safe claim lookup and typed parsing  
- Scoped DI registration  

Used by application services that require request‑scoped identity.

---

### **AuditLogging**

Provides structured audit logging for authentication events:

- `IAuditLogger` implementation  
- DI registration  
- Machine‑readable log output for SIEM and observability pipelines  

Integrated automatically through the Auth0 subsystem.

---

### **Settings**

Contains strongly typed configuration models:

- `OidcSettings` — authority, client credentials, callback URL  
- Bound from `Identity:Oidc`  
- Validated on startup  

Used by all Auth0 OIDC clients.

---

### **AssemblyMarker**

A simple marker type used for:

- Assembly scanning  
- Reflection‑based registration  
- Resource discovery  

Provides a stable anchor for referencing the Infrastructure assembly.

---

## Design Principles

The Infrastructure layer follows these principles:

- **Thin adapters**  
  No domain logic — only external integration.

- **Stateless components**  
  All OIDC clients rely on externally managed `HttpClient` instances.

- **Configuration safety**  
  Required settings validated on startup.

- **Vertical slice alignment**  
  Each subsystem (Auth0, Users, AuditLogging) is isolated and self‑contained.

- **Environment independence**  
  Infrastructure can be swapped without affecting domain or application layers.

---

## Example Startup Registration

```csharp
services.AddFrankIdentityInfrastructure();
```

This registers:

- User accessor  
- Auth0 OIDC clients  
- Audit logging  
- OIDC settings  

All infrastructure components are composed through this single entry point.

---

## Summary

The **Infrastructure** folder provides the concrete, environment‑specific
implementations required by the Identity subsystem:

- Auth0 OIDC integration  
- User identity resolution  
- Audit logging  
- Settings binding  
- Assembly marker  

It ensures that the Identity subsystem remains clean, testable, and fully
decoupled from external systems.

