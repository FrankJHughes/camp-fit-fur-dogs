# US‑176 — Frank: Unified Test Harness Engine

## Story Grammar

As an **architect**, I must be able to **move the unified test harness into Frank as a core testing engine** so that **all products share the same deterministic, governed, multi‑layer testing infrastructure**.

---

# Intent

The current test harness is not “just tests.”  
It is a **cross‑cutting testing engine** that provides:

- API test host  
- DI override engine  
- Fake authentication engine  
- Fake session engine  
- Fake clock  
- Fake environment  
- Testcontainers orchestration  
- Guardrail engine  
- Frontend harness  
- FormCommand harness  
- State machine harness  

This belongs in Frank, not in the product.

---

# Motivation

- Every product needs the same test harness  
- The harness enforces architecture and purity  
- The harness provides deterministic test environments  
- The harness provides cross‑cutting capabilities  
- The harness reduces boilerplate  
- The harness ensures consistency across products  

---

# Scope

This story introduces a new Frank subsystem:

````text
Frank/Testing/
    ApiTestHost/
    ApplicationTestHost/
    InfrastructureTestHost/
    FrontendTestHost/
    GuardrailEngine/
    TestClock/
    TestIdentity/
    TestDatabase/
    TestEnvironment/
    TestOverrides/
    TestCookies/
    TestProblemDetails/
````

This story includes:

- Migrating the existing test harness into Frank  
- Creating a unified API test host  
- Creating a unified DI override engine  
- Creating a unified fake authentication engine  
- Creating a unified fake session engine  
- Creating a unified fake clock  
- Creating a unified Testcontainers orchestration layer  
- Creating a unified guardrail engine  
- Creating a unified frontend testing harness  
- Creating engine‑level documentation:
  - `TestHarnessOverview.md`
  - `TestHarnessArchitecture.md`
  - `TestHarnessTroubleshooting.md`
  - `TestHarnessReferenceExamples.md`

---

# Acceptance Criteria

- [ ] All test harness capabilities live in Frank  
- [ ] Product test projects reference Frank.Testing  
- [ ] API tests use Frank’s ApiTestHost  
- [ ] Application tests use Frank’s ApplicationTestHost  
- [ ] Infrastructure tests use Frank’s Testcontainers orchestration  
- [ ] Frontend tests use Frank’s FrontendTestHost  
- [ ] Guardrail tests use Frank’s GuardrailEngine  
- [ ] DI override engine works across all layers  
- [ ] Fake authentication works across all layers  
- [ ] Fake session engine works across all layers  
- [ ] Test clock works across all layers  
- [ ] All existing tests pass without modification (except imports)  
- [ ] Architecture tests enforce new boundaries  

---

# Out of Scope

- New guardrail rules  
- New frontend testing patterns  
- New FormCommand features  

---

# Dependencies

- US‑2XX — Frank: Auth Callback Engine Migration  
- test-harness-overview.md (completed)  

---

# Notes

This is a **migration**, not a redesign.  
A future story will formalize the Frank.Testing API surface.

