# Frank.Identity.Testing — Factories

This document describes the `/docs/04-testing` area and maps it back to the implementation under `/src/Frank/Testing`.

## Purpose

The **Factories** subsystem provides deterministic, reusable builders for constructing domain models, application commands, EF Core entities, and test‑host components used throughout the Identity test suite. Factories ensure tests remain expressive, stable, and free from boilerplate by centralizing object creation logic.

Factories contain **no production logic**. They exist solely to support testing by generating valid, realistic objects that reflect the behavior of the Identity vertical.

---

## Source alignment

- Primary implementation area: `/src/Frank/Testing`
- Current folder: `/docs/04-testing`

Factories are used across unit, integration, functional, and endpoint tests.

---

## What belongs here

- responsibilities of factory classes  
- how factories connect to the broader testing platform  
- runtime and infrastructure collaboration points  
- how factories support API → Application → Domain → EF Core testing flows  

---

## Responsibilities of the subsystem

### [Domain Model Factories](ca://s?q=Explain_identity_domain_factories)
Factories generate valid domain objects for testing.

Responsibilities:

- create `User` aggregates with realistic provider metadata  
- create `Session` aggregates with valid timestamps and tokens  
- generate value objects (IdentityId, ProviderSubject, SessionId)  
- ensure domain invariants are respected  
- support edge‑case generation (expired sessions, revoked sessions, invalid claims)  

Domain factories allow tests to focus on behavior rather than object construction.

---

### [Application Command & Query Factories](ca://s?q=Explain_identity_application_factories)
Factories generate commands and queries used by the Application layer.

Responsibilities:

- create commands for session issuance  
- create commands for user creation/update  
- create queries for session lookup  
- generate realistic OIDC callback payloads  
- support invalid or malformed command generation for negative tests  

Application factories ensure orchestration tests remain expressive and concise.

---

### [EF Core Entity Factories](ca://s?q=Explain_identity_efcore_factories)
Factories generate EF Core entities for persistence tests.

Responsibilities:

- create user entities with mapped value objects  
- create session entities with correct expiration and revocation fields  
- generate entities for lockout, rate limiting, or security features  
- support seeding test databases  
- ensure entities match actual EF Core configurations  

EF Core factories ensure persistence tests remain stable and predictable.

---

### [Test Host & Middleware Factories](ca://s?q=Explain_identity_test_host_factories)
Factories generate test hosts and middleware pipelines.

Responsibilities:

- create test hosts using the Frank Test Harness (US‑176)  
- configure DI overrides (including opt‑out behavior from US‑185)  
- inject fake OIDC providers  
- inject fake clock, logging, and configuration services  
- generate authenticated and unauthenticated request contexts  

Test host factories enable full end‑to‑end testing of Identity endpoints.

---

### [Request & Response Factories](ca://s?q=Explain_identity_request_factories)
Factories generate HTTP requests and expected responses.

Responsibilities:

- create authenticated requests with valid session tokens  
- create unauthenticated requests for negative tests  
- generate OIDC callback requests  
- generate expected responses for authorization failures  
- support correlation ID and observability metadata (US‑183)  

Request factories ensure endpoint tests remain readable and expressive.

---

## How factories connect to the broader platform

Factories collaborate with:

- **Frank.Identity.Domain**  
  - generate domain models for unit tests  
  - ensure invariants are respected  

- **Frank.Identity.Application**  
  - generate commands, queries, and callback payloads  
  - support orchestration tests  

- **Frank.Identity.EntityFrameworkCore**  
  - generate entities for persistence tests  
  - seed test databases  

- **Frank.Identity.Infrastructure**  
  - generate fake provider metadata  
  - generate fake current user contexts  
  - support configuration and logging overrides  

- **Frank.Identity.Api**  
  - generate HTTP requests for endpoint tests  
  - simulate authentication and authorization flows  

Factories are the glue that allow tests to exercise the entire vertical without boilerplate.

---

## Runtime and infrastructure collaboration points

Factories interact with the runtime by:

- constructing test hosts with real middleware  
- injecting fake infrastructure services  
- generating realistic OIDC metadata and JWKS keys  
- producing valid session tokens for authenticated tests  
- generating invalid tokens for negative tests  
- supporting environment‑specific behavior (dev vs. prod)  

Factories ensure tests can simulate real runtime conditions without external dependencies.

---

## Composition flow (Factory → Test → Host → Middleware → Handler → Persistence)

```
Factory Creates Test Objects
    ↓
Test Uses Factory Output
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

Factories support the entire testing pipeline end‑to‑end.

---

## Notes

Keep this document grounded in the actual Frank.Identity test suite.  
As new identity features or test harness capabilities are added, update this section to reflect new testing requirements.
