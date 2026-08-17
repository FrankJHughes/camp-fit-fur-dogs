# Identity Infrastructure — Auth0 OIDC Integration

The **Auth0** folder contains all infrastructure components required to integrate
the Identity subsystem with Auth0’s OpenID Connect (OIDC) endpoints.  
These components implement the OIDC vertical slice for:

- Authorization‑code token exchange  
- ID token validation  
- UserInfo retrieval  
- DI registration for all Auth0 OIDC services  

This folder provides a complete, production‑ready Auth0 integration aligned with
your Identity architecture.

---

## Purpose

The Auth0 subsystem provides:

- A token client for exchanging authorization codes  
- A token validator for verifying ID tokens using Auth0 JWKS  
- A userinfo client for retrieving profile claims  
- Configuration binding for `OidcSettings`  
- DI registration for all OIDC components  
- Integration with audit logging  

It ensures secure, standards‑compliant OIDC flows for owner authentication.

---

## Files

### **Auth0OidcTokenClient**

Implements `IOidcTokenClient`.

Responsibilities:

- Calls Auth0’s `/oauth/token` endpoint  
- Exchanges authorization codes for access + ID tokens  
- Parses JSON responses  
- Throws `OidcProtocolException` on protocol failures  

Used during:

- OIDC callback processing  
- Owner login flows  
- Token acquisition for downstream calls  

---

### **Auth0OidcTokenValidator**

Implements `IOidcTokenValidator`.

Responsibilities:

- Retrieves JWKS from Auth0  
- Builds `TokenValidationParameters`  
- Validates ID tokens (issuer, audience, signature, lifetime)  
- Extracts the `sub` claim and all string‑typed claims  
- Throws `OidcProtocolException` on validation failures  

Used during:

- OIDC callback validation  
- Authentication security enforcement  
- Identity provider trust verification  

---

### **Auth0OidcUserInfoClient**

Implements `IOidcUserInfoClient`.

Responsibilities:

- Calls Auth0’s `/userinfo` endpoint  
- Sends bearer‑authenticated GET requests  
- Maps JSON payload into `OidcUserInfo`  
- Extracts standard OIDC claims and all string‑typed claims  
- Throws `OidcProtocolException` on failures  

Used during:

- Profile enrichment  
- Account creation flows  
- External identity mapping  

---

### **ServiceCollectionExtensions**

Registers all Auth0 OIDC services.

Responsibilities:

- Binds `OidcSettings` from `Identity:Oidc`  
- Validates configuration on startup  
- Registers:
  - `IAuditLogger`  
  - `IOidcUserInfoClient`  
  - `IOidcTokenClient`  
  - `IOidcTokenValidator`  
- Ensures correct lifetimes (`Transient` for stateless OIDC clients)  

Used during:

- Application startup  
- Identity subsystem initialization  

---

## Design Principles

The Auth0 subsystem follows these architectural principles:

- **Vertical slice isolation**  
  Each OIDC operation (token, validation, userinfo) has its own interface + implementation.

- **Strict protocol compliance**  
  JWKS validation, issuer/audience checks, lifetime enforcement.

- **Stateless clients**  
  All OIDC clients rely on externally managed `HttpClient` instances.

- **Configuration safety**  
  `OidcSettings` is validated on startup to prevent misconfiguration.

- **Structured audit logging**  
  All authentication flows integrate with the audit‑logging subsystem.

---

## Example UserInfo Response

```json
{
  "sub": "auth0|abc123",
  "email": "owner@example.com",
  "given_name": "Frank",
  "family_name": "Identity",
  "picture": "https://example.com/avatar.png"
}
```

---

## Summary

The **Auth0** folder provides the complete OIDC integration for the Identity
subsystem:

- Token exchange  
- Token validation  
- UserInfo retrieval  
- Configuration binding  
- DI registration  
- Audit logging integration  

It ensures secure, standards‑compliant authentication flows backed by Auth0.

