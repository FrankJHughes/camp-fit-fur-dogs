# Frank.Testing — Testing Strategy

This document describes the `/docs/04-testing` area and maps it back to the implementation under `/src/Frank/Testing`.

## Purpose

The **Testing Strategy** defines how the platform verifies correctness, stability, and security across the full vertical — API → Application → Domain → EF Core → Infrastructure. It ensures tests are realistic, deterministic, and aligned with production behavior while remaining isolated and repeatable.

This strategy guides contributors on *how* to test, *what* to test, and *where* each type of test belongs.

---

## Source alignment

- Primary implementation area: `/src/Frank/Testing`
- Current folder: `/docs/04-testing`

The strategy applies to all testing subsystems: Contexts, Endpoints, Factories, Integration, and Mutated WebApp Context.

---

## What belongs here

- the responsibilities of the testing strategy  
- how testing connects to the broader platform  
- runtime and infrastructure collaboration points  
- how tests exercise the full vertical from API to persistence  

---

## Strategic Principles

### 1. **Test the System, Not the Framework**
Identity and platform features should be tested through realistic flows, not mocked abstractions.

- Prefer full HTTP pipeline tests over controller‑only tests  
- Prefer EF Core test databases over mocked repositories  
- Prefer real middleware execution over bypassing the pipeline  

This ensures tests validate *actual behavior*, not assumptions.

---

### 2. **Use Mutated WebApp Contexts for Realism**
The test harness provides controlled mutation of the runtime environment.

- override DI services  
- inject fake providers  
- modify configuration  
- alter middleware ordering  
- simulate environment flags (Development/Production)

This allows tests to simulate real‑world conditions without touching production code.

---

### 3. **Factories Provide Deterministic Object Construction**
Factories ensure domain models, EF Core entities, and test data remain valid and expressive.

- domain factories respect invariants  
- EF Core factories match actual mappings  
- test data builders reflect real identity flows  

Factories eliminate boilerplate and prevent invalid test objects.

---

### 4. **Integration Tests Validate Vertical Behavior**
Integration tests exercise Application + Infrastructure + Domain + EF Core together.

- authentication flows  
- session lifecycle  
- user persistence  
- provider metadata handling  
- unit‑of‑work behavior  

Integration tests ensure cross‑layer correctness.

---

### 5. **Endpoint Tests Validate Full HTTP Behavior**
Endpoint tests run through the entire pipeline:

```
Request → Middleware → Endpoint → Application → Domain → EF Core → Response
```

They validate:

- authentication  
- authorization  
- session validation  
- security headers  
- CORS  
- rate limiting  
- error semantics  

Endpoint tests ensure the platform behaves correctly under real request conditions.

---

### 6. **Avoid Over‑Mocking**
Mocks are used sparingly and only when necessary.

Appropriate mocking:

- external providers (Auth0, JWKS)  
- time/clock services  
- logging sinks  
- environment detection  

Inappropriate mocking:

- domain models  
- EF Core persistence  
- middleware  
- application orchestration  

The strategy favors realism over isolation.

---

### 7. **Tests Must Be Deterministic**
Tests must produce identical results across environments.

- fixed clocks  
- seeded databases  
- stable provider metadata  
- deterministic configuration  
- isolated test hosts  

Determinism ensures reliability and prevents flaky tests.

---

## How the strategy connects to the broader platform

Testing collaborates with:

- **Frank.Identity.Api** — middleware, routing, endpoint handlers  
- **Frank.Identity.Application** — authentication/session orchestration  
- **Frank.Identity.Domain** — invariants surfaced through test flows  
- **Frank.Identity.EntityFrameworkCore** — persistence tested through real or in‑memory databases  
- **Frank.Identity.Infrastructure** — provider integration, configuration binding, logging  
- **Frank Test Harness (US‑176)** — deterministic setup/teardown, DI overrides, fake providers  

The strategy ensures every layer is validated in realistic conditions.

---

## Runtime and infrastructure collaboration points

Testing interacts with the runtime by:

- constructing mutated test hosts  
- injecting fake infrastructure services  
- mutating configuration and environment flags  
- altering middleware pipelines  
- simulating authenticated/unauthenticated requests  
- capturing logs and correlation IDs (US‑183)  
- using test databases with seeded identity state  

This ensures the platform is validated under both normal and adverse conditions.

---

## Composition flow (Strategy → Subsystems → Tests → Platform)

```
Testing Strategy
    ↓
Testing Subsystems (Contexts, Endpoints, Factories)
    ↓
Tests (Unit, Integration, Endpoint)
    ↓
Platform (API → Application → Domain → EF Core → Infrastructure)
```

The strategy guides how all testing layers work together.

---

## Notes

Keep this document grounded in the actual Frank.Testing implementation.  
As new testing utilities, identity features, or runtime behaviors are added, update this strategy to reflect the evolving architecture.
