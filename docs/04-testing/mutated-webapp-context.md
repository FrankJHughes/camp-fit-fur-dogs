# Frank.Identity.Testing — Mutated WebApp Context

This document describes the `/docs/04-testing` area and maps it back to the implementation under `/src/Frank/Testing`.

## Purpose

The **Mutated WebApp Context** subsystem provides controlled, test‑only mutations of the Identity web application’s runtime environment. It allows tests to override configuration, replace services, inject fake infrastructure, and alter middleware behavior without modifying production code. This enables deep, realistic testing of Identity flows under varied runtime conditions.

Mutated contexts are essential for verifying how Identity behaves when the hosting environment, DI container, or middleware pipeline is intentionally altered.

---

## Source alignment

- Primary implementation area: `/src/Frank/Testing`
- Current folder: `/docs/04-testing`

Mutated WebApp Context is part of the Frank Test Harness (US‑176) and is used across integration, functional, and endpoint tests.

---

## What belongs here

- responsibilities of mutated context utilities  
- how mutated contexts connect to the broader platform  
- runtime and infrastructure collaboration points  
- how mutated contexts support API → Application → Domain → EF Core testing flows  

---

## Responsibilities of the subsystem

### [Service Replacement](ca://s?q=Explain_identity_service_replacement)
Mutated contexts allow tests to replace DI services.

Responsibilities:

- override OIDC provider integration with fakes  
- replace Auth0 metadata/JWKS retrieval with deterministic test versions  
- inject fake clocks for session expiration tests  
- inject fake logging and observability sinks (US‑183)  
- override configuration providers for environment‑specific tests  

Service replacement enables deterministic testing of runtime behavior.

---

### [Configuration Mutation](ca://s?q=Explain_identity_configuration_mutation)
Mutated contexts allow tests to alter configuration at runtime.

Responsibilities:

- override OIDC settings (issuer, audience, JWKS URL)  
- simulate invalid or missing configuration  
- toggle environment flags (Development, Production)  
- inject test‑specific configuration sections  
- simulate misconfigured providers  

Configuration mutation ensures Identity behaves correctly under misconfiguration.

---

### [Middleware Pipeline Mutation](ca://s?q=Explain_identity_middleware_mutation)
Mutated contexts allow tests to alter the middleware pipeline.

Responsibilities:

- insert test middleware before authentication  
- remove or replace security middleware (US‑134, US‑135, US‑132)  
- simulate broken or reordered middleware  
- inject request‑scoped identity overrides  
- simulate missing session validation  

Middleware mutation enables testing of pipeline ordering and resilience.

---

### [Request Context Mutation](ca://s?q=Explain_identity_request_context_mutation)
Mutated contexts allow tests to alter the HTTP request environment.

Responsibilities:

- inject fake authenticated users  
- inject invalid or expired session tokens  
- simulate missing headers or malformed requests  
- override correlation IDs and observability metadata  
- simulate cross‑origin requests for CORS testing  

Request mutation enables realistic endpoint testing without external dependencies.

---

### [Database Context Mutation](ca://s?q=Explain_identity_database_mutation)
Mutated contexts allow tests to alter EF Core behavior.

Responsibilities:

- use in‑memory or ephemeral test databases  
- seed identity users and sessions  
- simulate database failures  
- override EF Core provider behavior  
- test migrations in isolation  

Database mutation ensures persistence behavior is validated under varied conditions.

---

## How the subsystem connects to the broader platform

Mutated WebApp Context collaborates with:

- **Frank.Identity.Api**  
  - tests middleware ordering, endpoint behavior, and request resolution  

- **Frank.Identity.Application**  
  - tests authentication/session orchestration under mutated conditions  

- **Frank.Identity.Domain**  
  - tests domain invariants surfaced through mutated flows  

- **Frank.Identity.EntityFrameworkCore**  
  - tests persistence behavior with mutated database contexts  

- **Frank.Identity.Infrastructure**  
  - tests provider integration, configuration binding, logging, and environment detection  

- **Frank Test Harness (US‑176)**  
  - provides the foundation for building mutated contexts  
  - supports DI overrides, fake providers, and custom pipelines  

Mutated contexts allow the entire vertical to be tested under controlled, non‑production conditions.

---

## Runtime and infrastructure collaboration points

Mutated contexts interact with the runtime by:

- constructing test hosts with modified DI containers  
- injecting fake infrastructure services  
- mutating configuration and environment flags  
- altering middleware pipelines  
- simulating authenticated/unauthenticated request contexts  
- capturing logs and correlation IDs (US‑183)  
- using test databases with seeded identity state  

They ensure Identity behaves correctly even when the runtime environment is intentionally altered.

---

## Composition flow (Mutated Context → Test Host → Middleware → Handler → Persistence)

```
Mutated WebApp Context
    ↓
Test Host (Frank Test Harness)
    ↓
Identity Middleware (Auth, Session, Security)
    ↓
Endpoint Handler
    ↓
Application Services
    ↓
Domain Models
    ↓
EF Core Persistence
    ↓
Response Verified by Test
```

Mutated contexts validate the robustness and resilience of Identity under non‑standard runtime conditions.

---

## Notes

Keep this document grounded in the actual Frank.Identity test suite.  
As new identity features or runtime behaviors are added, update this section to reflect new mutation capabilities.
