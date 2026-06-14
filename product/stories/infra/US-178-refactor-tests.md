# US‑178 — Refactor Tests

## Story Grammar  
As a **developer**, I must **refactor the existing test suite to align with the stabilized harness** so that **tests are maintainable, consistent, and follow the new conventions**.

## Intent  
After stabilization, the test suite must be updated to use the correct patterns.

## Acceptance Criteria  
- All tests use the unified test host  
- No legacy helpers remain  
- No duplicated patterns  
- No brittle async patterns  
- No direct environment manipulation  
- All tests pass reliably  

---

# US‑179 — Implement Authenticated User Service

## Story Grammar  
As an **architect**, I must **implement a unified AuthenticatedUserService** so that **all layers share a consistent identity model**.

## Intent  
Identity is currently scattered across middleware, callbacks, and test fakes. This story centralizes it.

## Acceptance Criteria  
- Single source of truth for authenticated user  
- Works in API runtime  
- Works in test harness  
- Works with fake identity  
- Works with session model  
- No direct HttpContext access outside API layer  

---

# US‑180 — Migrate Hosting Engine to Frank

## Story Grammar  
As an **architect**, I must **migrate the Hosting Engine into Frank** so that **hosting orchestration becomes a governed, reusable capability across products**.

## Intent  
The Hosting Engine is deterministic, cross‑cutting, and enforces invariants — it belongs in Frank.

## Acceptance Criteria  
- HostingEngine lives in Frank  
- Product only defines modules  
- DI auto‑registration updated  
- Guardrail tests updated  
- No product references to old hosting code  

---

# US‑181 — Migrate Startup Engine to Frank

## Story Grammar  
As an **architect**, I must **migrate the Startup Engine into Frank** so that **startup orchestration is governed, reusable, and consistent across products**.

## Intent  
StartupEngine is a pure orchestration engine and should be centralized.

## Acceptance Criteria  
- StartupEngine lives in Frank  
- Product only defines startup modules  
- DI auto‑registration updated  
- Guardrail tests updated  
- No product references to old startup code  
