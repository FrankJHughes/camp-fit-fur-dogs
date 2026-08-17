# Frank.Identity.Infrastructure — Overview

This document describes the `/docs/03-frank-identity/infrastructure` area and maps it back to the implementation under `/src/Frank/Identity/Infrastructure`.

## Purpose

The **Infrastructure** subsystem provides the runtime, hosting, and operational services required by the Identity vertical. While the Identity Domain layer defines pure identity models and invariants, and the Identity Application layer implements authentication, session issuance, claim mapping, and lockout evaluation, the Infrastructure layer supplies the **runtime mechanics** that make those flows possible in a real environment.

Infrastructure contains **no identity logic**, **no domain rules**, and **no persistence behavior**. It is strictly responsible for configuration, environment detection, provider integration, logging, and cross‑cutting runtime services.

---

## Source Alignment

- Primary implementation area: `/src/Frank/Identity/Infrastructure`
- Current folder: `/docs/03-frank-identity/infrastructure`

Infrastructure is consumed by Identity Application, Domain, EF Core, and API layers.

---

## Responsibilities of the Subsystem

### [Configuration Binding](ca://s?q=Explain_identity_configuration_binding)
Infrastructure binds identity‑specific configuration sections.

Responsibilities:

- bind OIDC provider settings  
- bind Auth0 settings (if configured)  
- bind session lifetime and expiration settings  
- bind lockout thresholds and durations  
- validate configuration at startup  

Configuration binding ensures identity behavior is environment‑correct.

---

### [Environment Detection](ca://s?q=Explain_identity_environment_detection)
Identity behavior varies by environment.

Responsibilities:

- detect development vs. production  
- enable verbose logging in development  
- enforce strict validation in production  
- toggle diagnostic metadata for non‑production environments  

Environment detection influences Application behavior, not Domain models.

---

### [Provider Integration](ca://s?q=Explain_identity_provider_integration)
Infrastructure integrates external identity providers into the runtime.

Responsibilities:

- configure OIDC provider endpoints  
- configure Auth0 metadata and JWKS retrieval  
- configure token validation parameters  
- support multiple providers if enabled  

Provider integration is runtime configuration, not domain logic.

---

### [Current User Resolution](ca://s?q=Explain_identity_context_access)
Infrastructure exposes the authenticated identity to the platform.

Responsibilities:

- resolve identity from validated session state  
- map claims to domain identity user  
- integrate with ASP.NET Core’s `ClaimsPrincipal`  
- ensure consistent identity context across API and Application layers  

Current User resolution is a runtime service, not identity logic.

---

### [Audit Logging](ca://s?q=Explain_identity_structured_logging)
Infrastructure provides structured logging for identity operations.

Responsibilities:

- log authentication attempts  
- log session issuance, validation, and revocation  
- log lockout evaluations  
- log provider metadata retrieval  
- attach correlation and causation metadata  
- integrate with platform‑wide observability (US‑183)  

Audit Logging ensures identity flows are diagnosable.

---

### [Runtime Services](ca://s?q=Explain_identity_runtime_services)
Infrastructure exposes cross‑cutting services used by identity flows.

Examples:

- clock abstraction  
- ID generation  
- HTTP client factory for provider metadata  
- caching (if identity uses distributed cache for lockout)  

Runtime services support identity flows without leaking domain logic.

---

## How Infrastructure Connects to the Broader Platform

Infrastructure collaborates with:

- **Frank.Identity.Application**  
  - provides configuration, logging, environment detection  
  - supplies provider integration and runtime services  

- **Frank.Identity.Domain**  
  - domain models remain pure; infrastructure never mutates them  

- **Frank.Identity.EntityFrameworkCore**  
  - provides DbContext configuration, connection strings, migrations  
  - supplies EF Core logging and provider‑specific behaviors  

- **Frank.Core.Infrastructure**  
  - hosting engine  
  - module loader  
  - structured logging  
  - configuration system  

- **Frank.Core.Api**  
  - middleware relies on infrastructure services (session validation, logging)  

Infrastructure is the glue between identity logic and the runtime environment.

---

## Runtime Collaboration Points

Infrastructure interacts with the runtime by:

- binding identity configuration  
- wiring identity services into DI  
- retrieving provider metadata  
- validating tokens  
- resolving the current user  
- emitting structured logs  
- supporting environment‑specific behavior  
- integrating with platform‑wide observability  

It ensures identity operations are secure, predictable, and diagnosable.

---

## Composition Flow (Infrastructure → Application → Domain → EF Core → API)

```
OIDC / Auth0 Settings Bound (Infrastructure)
    ↓
Provider Metadata & Token Validation Configured (Infrastructure)
    ↓
Authentication & Session Issuance (Application)
    ↓
Identity Domain Models Created
    ↓
Persistence (EF Core)
    ↓
API Middleware & Endpoints
```

Infrastructure provides the runtime foundation for all identity flows.

---

## What Belongs in This Document

- configuration binding  
- environment detection  
- provider integration  
- current user resolution  
- audit logging  
- runtime services  
- observability and logging behavior  

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
Whenever provider behavior, configuration rules, or runtime services evolve, update this section to reflect the current platform architecture.
