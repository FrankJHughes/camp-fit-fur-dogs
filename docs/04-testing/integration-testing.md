# Frank.Identity.Testing — Integration Testing

This document describes the `/docs/04-testing` area and maps it back to the implementation under `/src/Frank/Testing`.

## Purpose

The **Integration Testing** subsystem verifies how Identity components behave when exercised together through realistic runtime conditions. Unlike unit tests (which isolate domain logic) or endpoint tests (which validate full HTTP flows), integration tests focus on **cross‑layer behavior**: Application + Infrastructure + EF Core working as a cohesive vertical.

Integration tests ensure that identity flows behave correctly when real persistence, configuration, provider metadata, and runtime services are involved.

This subsystem contains **no production logic**. It exists solely to validate correctness, stability, and cross‑layer interaction.

---

## Source alignment

- Primary implementation area: `/src/Frank/Testing`
- Current folder: `/docs/04-testing`

Integration tests run through the Frank Test Harness (US‑176) and exercise Identity’s Application, Infrastructure, and EF Core layers.

---

## What belongs here

- responsibilities of integration tests  
- how integration tests connect to the broader platform  
- runtime and infrastructure collaboration points  
- how integration tests validate API → Application → Domain → EF Core flows  

---

## Responsibilities of the subsystem

### [Authentication Flow Integration Tests](ca://s?q=Explain_identity_authentication_integration_tests)
Validate the full authentication pipeline without HTTP routing.

Responsibilities:

- simulate OIDC callback payloads  
- validate token processing and claim normalization  
- verify domain user creation on first login  
- verify session issuance  
- verify audit logging (US‑183)  
- verify provider metadata retrieval (Auth0/OIDC)  

These tests ensure authentication logic works across Application + Infrastructure + Domain.

---

### [Session Lifecycle Integration Tests](ca://s?q=Explain_identity_session_lifecycle_tests)
Validate session creation, validation, expiration, and revocation.

Responsibilities:

- verify session issuance persists correctly  
- verify session validation uses EF Core state  
- verify expiration rules behave consistently  
- verify revocation updates persistence  
- verify session invariants surface correctly  

These tests ensure session behavior is stable across Domain + EF Core + Infrastructure.

---

### [User Persistence Integration Tests](ca://s?q=Explain_identity_user_persistence_tests)
Validate user creation, lookup, and update flows.

Responsibilities:

- verify user creation persists correctly  
- verify lookup by provider subject and identity ID  
- verify claim updates propagate to persistence  
- verify domain invariants are enforced before commit  
- verify EF Core mappings match domain models  

These tests ensure user behavior is consistent across Domain + EF Core.

---

### [Infrastructure Integration Tests](ca://s?q=Explain_identity_infrastructure_integration_tests)
Validate runtime services used by Identity flows.

Responsibilities:

- verify OIDC settings binding and validation  
- verify provider metadata retrieval  
- verify JWKS key refresh behavior  
- verify current user resolution (non‑HTTP context)  
- verify environment‑specific behavior  

These tests ensure Infrastructure behaves correctly when consumed by Application.

---

### [Unit of Work Integration Tests](ca://s?q=Explain_identity_uow_integration_tests)
Validate atomic persistence behavior.

Responsibilities:

- verify session + user updates commit together  
- verify rollback behavior on failure  
- verify EF Core exceptions surface correctly  
- verify audit logging occurs after commit  

These tests ensure persistence is consistent and predictable.

---

## How the subsystem connects to the broader platform

Integration tests collaborate with:

- **Frank.Identity.Application**  
  - authentication/session orchestration  
  - OIDC callback processing  
  - unit‑of‑work coordination  

- **Frank.Identity.Domain**  
  - domain invariants validated before persistence  
  - domain exceptions surfaced through integration flows  

- **Frank.Identity.EntityFrameworkCore**  
  - real persistence tested through test databases  
  - entity mappings validated  
  - migrations exercised  

- **Frank.Identity.Infrastructure**  
  - provider integration  
  - configuration binding  
  - logging and observability  
  - current user resolution  

- **Frank Test Harness (US‑176)**  
  - deterministic setup/teardown  
  - DI overrides  
  - fake provider metadata  
  - test database provisioning  

Integration tests validate the vertical behavior of Identity without requiring HTTP endpoints.

---

## Runtime and infrastructure collaboration points

Integration tests interact with the runtime by:

- constructing test hosts with real DI wiring  
- injecting fake OIDC providers and JWKS keys  
- binding configuration from test settings  
- using EF Core test databases  
- capturing logs and correlation IDs (US‑183)  
- simulating authenticated and unauthenticated flows  
- verifying environment‑specific behavior  

They ensure Identity behaves consistently across environments and runtime configurations.

---

## Composition flow (Test → Application → Infrastructure → Domain → EF Core)

```
Integration Test
    ↓
Application Services
    ↓
Infrastructure Services (OIDC, Logging, Settings)
    ↓
Domain Models (User, Session)
    ↓
EF Core Persistence
    ↓
Result Verified by Test
```

Integration tests validate the vertical behavior of Identity end‑to‑end without HTTP routing.

---

## Notes

Keep this document grounded in the actual Frank.Identity test suite.  
As new identity features or infrastructure components are added, update this section to reflect new integration testing requirements.
