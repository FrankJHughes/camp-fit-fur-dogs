# Platform CORS Configuration

The **Platform CORS** subsystem defines the Frank.Core API’s default Cross‑Origin
Resource Sharing (CORS) policy.  
It resolves origins from configuration, validates them, normalizes them into
canonical origin strings, and applies a hardened, environment‑aware CORS policy
for both the frontend and identity/OIDC flows.

This subsystem ensures that CORS behavior is predictable, centralized, and
driven entirely by configuration rather than hard‑coded values.

---

## Files

```
Cors/
└── CorsServiceCollectionExtensions.cs
```

---

## CorsServiceCollectionExtensions

`CorsServiceCollectionExtensions` registers the API’s default CORS policy using
origins resolved from configuration.

### Responsibilities

- Resolves and normalizes origins from configuration:
  - **Frontend:BaseUrl**
  - **Identity:Oidc:Authority**
- Validates that configured values are absolute URIs.
- Normalizes URIs into canonical origin format:
  - `scheme://host`
  - `scheme://host:port` (when non‑default)
- Applies a default CORS policy:
  - `.WithOrigins(frontendOrigin, oidcOrigin)`
  - `.AllowAnyHeader()`
  - `.AllowAnyMethod()`
  - `.AllowCredentials()`
  - `.SetPreflightMaxAge(TimeSpan.FromSeconds(preflightSeconds))`
- Validates preflight max‑age:
  - must be between **1** and **86400** seconds
  - defaults to **3600** seconds

### Why this matters

This subsystem ensures:

- CORS configuration is **environment‑driven**, not hard‑coded  
- origins are **validated and normalized** before being applied  
- frontend and identity flows behave consistently across environments  
- preflight caching is **controlled and safe**  
- CORS policy is **centralized**, reducing slice‑level duplication  

---

## Origin Resolution

Origins are resolved using:

```json
Frontend:BaseUrl
Identity:Oidc:Authority
```

If present:

1. They must be valid absolute URIs.
2. They are normalized into canonical origin strings.

If missing:

- `Frontend:BaseUrl` → fallback: `http://localhost:3000`
- `Identity:Oidc:Authority` → **required**, no fallback

Invalid or missing values produce clear exceptions.

---

## Preflight Max‑Age

The subsystem reads:

```json
Cors:PreflightMaxAgeSeconds
```

Rules:

- Missing → defaults to **3600**
- Must parse as an integer
- Must be between **1** and **86400**
- Otherwise throws a descriptive exception

This prevents misconfiguration that could cause browsers to cache preflight
responses too long or not at all.

---

## Usage

```csharp
services.AddFrankCoreApiPlatformCors(Configuration);
```

This registers the default CORS policy for the entire API.

---

## How Platform CORS Fits Into the Architecture

Platform CORS is part of the API’s hosting and configuration layer.  
It ensures that:

- CORS behavior is consistent across environments  
- origins are validated and normalized  
- identity and frontend flows work reliably  
- the API exposes a predictable cross‑origin surface  

This subsystem complements:

- **[Observations](ca://s?q=Tell_me_more_about_Observations_middleware)**  
- **[Security Headers](ca://s?q=Tell_me_more_about_Security_Headers_middleware)**  
- **[Exceptions](ca://s?q=Tell_me_more_about_Exception_handling_middleware)**  
- **[CORS Logging](ca://s?q=Explain_CORS_logging_middleware)**  

Together, they form a robust, secure, and observable API platform.

---

## Design Principles

- **Configuration‑first**  
  All origins come from configuration, not code.

- **Strict validation**  
  Invalid URIs fail fast with clear errors.

- **Canonical normalization**  
  Origins are always emitted in consistent format.

- **Safe defaults**  
  Preflight max‑age defaults to 3600 seconds.

- **Centralized policy**  
  One place defines the API’s CORS behavior.

---

## Notes

- This subsystem configures the **default** CORS policy; slices do not need to
  define their own.
- Works seamlessly with your CORS logging middleware.
- Safe for all environments, including production.

---
