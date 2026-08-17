# Frank.Identity.Infrastructure — OIDC Settings

This document describes the `/docs/03-frank-identity/infrastructure` area and maps it back to the implementation under `/src/Frank/Identity/Infrastructure`.

## Purpose

The **OIDC Settings** subsystem provides the configuration binding, validation, and environment‑aware runtime behavior required for OpenID Connect authentication. While the Identity Application layer performs authentication flows, claim mapping, and session issuance, the Infrastructure layer ensures that OIDC provider settings are correctly loaded, validated, and supplied to the rest of the Identity subsystem.

This subsystem contains **no identity logic**, **no domain invariants**, and **no persistence rules** — it is strictly operational support for configuring and consuming OIDC providers.

---

## Source Alignment

- Primary implementation area: `/src/Frank/Identity/Infrastructure`
- Current folder: `/docs/03-frank-identity/infrastructure`

OIDC Settings are implemented as part of Identity Infrastructure and consumed by Identity Application and API layers.

---

## Responsibilities of the Subsystem

### [Configuration Binding](ca://s?q=Explain_identity_configuration_binding)
OIDC settings are bound from configuration at startup.

Responsibilities:

- bind issuer URL  
- bind client ID and client secret  
- bind audience and scopes  
- bind metadata and JWKS endpoints  
- bind callback and logout URLs  
- validate configuration shape and required fields  

Configuration binding ensures OIDC behavior is environment‑correct.

---

### [Configuration Validation](ca://s?q=Explain_identity_configuration_validation)
Infrastructure validates OIDC settings before the application starts.

Responsibilities:

- ensure issuer is a valid HTTPS URL  
- ensure client ID and secret are present  
- ensure audience is non‑empty  
- ensure metadata endpoint is reachable (optional health check)  
- ensure settings match provider expectations  

Validation prevents runtime authentication failures.

---

### [Environment‑Aware Behavior](ca://s?q=Explain_identity_environment_detection)
OIDC Settings adapt to the hosting environment.

Responsibilities:

- enable verbose diagnostics in development  
- enforce strict validation in production  
- support mock OIDC providers for local development  
- toggle metadata caching based on environment  

Environment detection influences configuration behavior, not domain models.

---

### [Provider Metadata Integration](ca://s?q=Explain_identity_provider_metadata)
OIDC Settings supply metadata endpoints used by the provider integration subsystem.

Responsibilities:

- expose `.well-known/openid-configuration` URL  
- expose JWKS endpoint  
- support metadata refresh intervals  
- support provider‑specific overrides  

Metadata integration ensures the platform trusts the provider correctly.

---

### [Token Validation Configuration](ca://s?q=Explain_identity_token_validation)
OIDC Settings supply the parameters used by token validation infrastructure.

Responsibilities:

- issuer validation  
- audience validation  
- signature validation (JWKS)  
- clock skew tolerance  
- required claims (sub, iss, exp, iat)  

Token validation rules are runtime configuration, not domain logic.

---

### [Observability & Logging](ca://s?q=Explain_identity_observability)
OIDC Settings emit structured logs for configuration and provider interactions.

Responsibilities:

- log configuration binding  
- log validation failures  
- log provider metadata retrieval  
- attach correlation and causation metadata  
- integrate with platform‑wide observability (US‑183)  

Observability ensures configuration issues are diagnosable.

---

## How OIDC Settings Connect to the Broader Platform

OIDC Settings collaborate with:

- **Frank.Identity.Application**  
  - application services consume validated OIDC settings  
  - token validation and provider integration rely on these settings  

- **Frank.Identity.Domain**  
  - domain models remain pure; settings never mutate domain state  

- **Frank.Identity.EntityFrameworkCore**  
  - persistence stores identity users and sessions after OIDC authentication  

- **Frank.Core.Infrastructure**  
  - provides configuration system, logging, environment detection  
  - supplies HTTP client factory for metadata/JWKS retrieval  

- **Frank.Core.Api**  
  - middleware uses OIDC settings for authentication and authorization flows  

OIDC Settings are the configuration backbone of external authentication.

---

## Runtime Collaboration Points

OIDC Settings interact with the runtime by:

- binding provider configuration  
- validating configuration at startup  
- supplying metadata endpoints  
- supplying token validation parameters  
- emitting structured logs  
- supporting environment‑specific behavior  
- surfacing configuration failures to observability  

They ensure OIDC authentication is secure, predictable, and diagnosable.

---

## Composition Flow (Configuration → Infrastructure → Application → Domain → API)

```
appsettings.json / environment variables
    ↓
OIDC Settings Bound (Infrastructure)
    ↓
OIDC Settings Validated (Infrastructure)
    ↓
Token Validation & Provider Integration (Application)
    ↓
Identity Domain Models Created
    ↓
Session Issuance (Application)
    ↓
Identity API Endpoints
```

OIDC Settings provide the configuration foundation for all external authentication flows.

---

## What Belongs in This Document

- OIDC configuration binding  
- configuration validation  
- environment‑aware behavior  
- provider metadata integration  
- token validation configuration  
- observability and logging  

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
Whenever OIDC configuration, provider metadata, or token validation rules evolve, update this section to reflect the current platform architecture.
