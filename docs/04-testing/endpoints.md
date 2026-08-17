# Frank.Identity.Testing — Endpoints

This document describes the `/docs/04-testing` area and maps it back to the implementation under `/src/Frank/Testing`.

## Purpose

The **Endpoints Testing** subsystem verifies the behavior of Identity API endpoints under realistic conditions. It ensures authentication, authorization, OIDC callback handling, session validation, and security middleware behave correctly when exercised through HTTP. Endpoint tests validate the full request pipeline — routing, middleware, handlers, infrastructure, and persistence — and provide regression protection for the Identity vertical.

This subsystem contains **no production logic**. It exists solely to validate correctness, safety, and stability.

---

## Source alignment

- Primary implementation area: `/src/Frank/Testing`
- Current folder: `/docs/04-testing`

Endpoint tests run through the Frank Test Harness (US‑176) and exercise the Identity API layer end‑to‑end.

---

## What belongs here

- responsibilities of endpoint‑level tests  
- how endpoint tests connect to the broader platform  
- runtime and infrastructure collaboration points  
- how the test harness composes API → Application → Domain → EF Core  

---

## Responsibilities of the subsystem

### [Authentication Endpoint Tests](ca://s?q=Explain_identity_authentication_endpoint_tests)
Tests validate the OIDC login and callback endpoints.

Responsibilities:

- verify login endpoint redirects to Auth0/OIDC provider  
- verify callback endpoint processes tokens correctly  
- verify invalid tokens produce correct HTTP semantics  
- verify audit logging occurs (US‑183)  
- verify no identity provider tokens are persisted (US‑110)  

Authentication endpoint tests ensure the login flow is safe and predictable.

---

### [Session Endpoint Tests](ca://s?q=Explain_identity_session_endpoint_tests)
Tests validate session issuance, validation, and revocation.

Responsibilities:

- verify session token is issued after successful login  
- verify session token is validated on protected endpoints  
- verify expired tokens return `401 Unauthorized`  
- verify revocation endpoint invalidates session state  
- verify session middleware remains pure (US‑111)  

Session endpoint tests ensure stable, secure session behavior.

---

### [Authorization Endpoint Tests](ca://s?q=Explain_identity_authorization_endpoint_tests)
Tests validate access control on protected endpoints.

Responsibilities:

- verify protected endpoints reject unauthenticated requests  
- verify authenticated requests succeed with correct identity  
- verify authorization policies enforce role‑based rules  
- verify `ICurrentUser` is populated correctly  
- verify unauthorized requests return `403 Forbidden`  

Authorization endpoint tests ensure correct enforcement of access rules.

---

### [Security Middleware Tests](ca://s?q=Explain_identity_security_middleware_tests)
Tests validate cross‑cutting security behaviors.

Responsibilities:

- verify security headers (US‑134)  
- verify CORS policy enforcement (US‑135)  
- verify rate limiting activates correctly (US‑132)  
- verify lockout behavior is enforced (US‑133)  
- verify structured observability logs (US‑183)  

Security middleware tests ensure the platform is hardened and predictable.

---

### [Error Semantics Tests](ca://s?q=Explain_identity_error_semantics_tests)
Tests validate consistent error behavior across endpoints.

Responsibilities:

- verify invalid login produces identical responses for wrong email, wrong password, or locked account (US‑133)  
- verify invalid tokens produce generic error messages  
- verify malformed requests return correct HTTP status codes  
- verify no sensitive information leaks in error responses  

Error semantics tests ensure safe, blame‑free error handling.

---

## How the subsystem connects to the broader platform

Endpoint tests collaborate with:

- **Frank.Identity.Api** — middleware, routing, endpoint handlers  
- **Frank.Identity.Application** — authentication/session orchestration  
- **Frank.Identity.Domain** — domain invariants surfaced through endpoints  
- **Frank.Identity.EntityFrameworkCore** — persistence tested through real or test DB  
- **Frank.Identity.Infrastructure** — provider integration, configuration, logging  
- **Frank Test Harness (US‑176)** — deterministic setup/teardown, DI overrides, fake infrastructure  

Endpoint tests validate the entire vertical, not isolated components.

---

## Runtime and infrastructure collaboration points

Endpoint tests interact with the runtime by:

- issuing real HTTP requests against the test host  
- validating middleware ordering and purity  
- verifying DI wiring (including opt‑out behavior from US‑185)  
- capturing logs and correlation IDs (US‑183)  
- simulating authenticated and unauthenticated requests  
- verifying persistence through EF Core test contexts  

They ensure Identity endpoints behave consistently across environments.

---

## Composition flow (Test → Host → Middleware → Handler → Persistence)

```
Test Request
    ↓
Frank Test Harness (US‑176)
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

Endpoint tests validate the full request pipeline end‑to‑end.

---

## Notes

Keep this document grounded in the actual Frank.Identity test suite.  
As new endpoints or middleware are added, update this section to reflect new testing requirements.
