# Frank.Identity.Infrastructure — Auth0 Integration

This document describes the `/docs/03-frank-identity/infrastructure` area and maps it back to the implementation under `/src/Frank/Identity/Infrastructure`.

## Purpose

The Auth0 Integration subsystem provides the runtime configuration, provider metadata handling, and token‑validation infrastructure required for using Auth0 as the external identity provider. While the Identity Application layer performs authentication flows, claim mapping, and session issuance, the Infrastructure layer ensures Auth0 is correctly configured, reachable, and validated at runtime.

This subsystem contains **no identity logic**, **no domain invariants**, and **no persistence rules** — it is strictly operational support for the Identity vertical.

---

## Source Alignment

- Primary implementation area: `/src/Frank/Identity/Infrastructure`
- Current folder: `/docs/03-frank-identity/infrastructure`

Auth0 integration is implemented as part of the Identity Infrastructure module and consumed by the Identity Application layer.

---

## Responsibilities of the Subsystem

### [Provider Configuration Binding](ca://s?q=Explain_identity_configuration_binding)
Auth0 settings are bound from configuration at startup.

Responsibilities:

- bind Auth0 domain (issuer)  
- bind client ID and client secret  
- bind audience and scopes  
- bind JWKS endpoint  
- validate configuration presence and shape  

Configuration binding ensures Auth0 behavior is environment‑correct.

---

### [Provider Metadata Retrieval](ca://s?q=Explain_identity_provider_metadata)
Auth0 publishes OIDC metadata used by the platform.

Responsibilities:

- retrieve `.well-known/openid-configuration`  
- retrieve JWKS signing keys  
- cache provider metadata for performance  
- validate metadata shape and required fields  

Metadata retrieval ensures the platform trusts Auth0 correctly.

---

### [Token Validation Infrastructure](ca://s?q=Explain_identity_token_validation)
Infrastructure configures the token validation pipeline used by Application services.

Responsibilities:

- configure issuer validation  
- configure audience validation  
- configure signature validation using JWKS  
- configure clock skew tolerance  
- configure required claims (sub, iss, exp, iat)  

Token validation rules are runtime configuration, not domain logic.

---

### [Environment‑Aware Behavior](ca://s?q=Explain_identity_environment_detection)
Auth0 integration adapts to the hosting environment.

Responsibilities:

- enable verbose diagnostics in development  
- enforce strict validation in production  
- toggle metadata caching based on environment  
- support local development overrides (e.g., mock provider)  

Environment detection ensures safe and predictable provider behavior.

---

### [Provider Health & Reachability](ca://s?q=Explain_identity_provider_health)
Infrastructure ensures Auth0 is reachable and healthy.

Responsibilities:

- perform startup health checks (optional)  
- validate JWKS endpoint availability  
- validate metadata endpoint availability  
- surface provider failures to observability  

Provider health checks improve reliability and diagnosability.

---

### [Observability & Logging](ca://s?q=Explain_identity_observability)
Auth0 integration emits structured logs for provider interactions.

Responsibilities:

- log metadata retrieval  
- log JWKS refresh events  
- log token validation failures  
- attach correlation and causation metadata  
- integrate with platform‑wide observability (US‑183)  

Observability ensures provider interactions are diagnosable.

---

## How Auth0 Integration Connects to the Broader Platform

Auth0 Infrastructure collaborates with:

- **Frank.Identity.Application**  
  - application services perform authentication flows  
  - infrastructure provides token validation and provider metadata  

- **Frank.Identity.Domain**  
  - domain models remain pure; infrastructure never mutates them  
  - domain exceptions surface through provider validation failures  

- **Frank.Identity.EntityFrameworkCore**  
  - persistence stores identity users and sessions after Auth0 authentication  

- **Frank.Core.Infrastructure**  
  - logging, configuration, environment detection  
  - HTTP client factory for metadata/JWKS retrieval  

- **Frank.Core.Api**  
  - middleware uses Auth0 validation for session and authorization flows  

Auth0 integration is the runtime bridge between external identity and the platform.

---

## Runtime Collaboration Points

Auth0 Infrastructure interacts with the runtime by:

- binding provider configuration  
- retrieving provider metadata  
- refreshing JWKS signing keys  
- validating tokens during authentication  
- emitting structured logs for provider interactions  
- supporting environment‑specific behavior  
- surfacing provider failures to observability  

It ensures Auth0 authentication is secure, predictable, and diagnosable.

---

## Composition Flow (Auth0 → Infrastructure → Application → Domain → API)

```
Auth0 Provider
    ↓
OIDC Metadata & JWKS (Infrastructure)
    ↓
Token Validation (Infrastructure)
    ↓
Authentication Flow (Application)
    ↓
Identity Domain Models Created
    ↓
Session Issuance (Application)
    ↓
Identity API Endpoints
```

Auth0 Integration provides the runtime foundation for all external authentication flows.

---

## What Belongs in This Document

- Auth0 configuration binding  
- provider metadata retrieval  
- JWKS handling  
- token validation infrastructure  
- environment‑aware behavior  
- observability and logging  
- provider health checks  

This document does **not** include:

- authentication logic  
- claim mapping  
- session issuance  
- lockout evaluation  
- HTTP endpoints  
- domain invariants  
- persistence logic  

Those belong in the application, domain, or EF Core layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity Infrastructure implementation.  
Whenever Auth0 configuration, provider metadata, or token validation rules evolve, update this section to reflect the current platform architecture.
