# Camp Fit Fur Dogs — Guides - Tester  
Product‑specific testing handbook

Welcome to the **Camp Fit Fur Dogs Tester Guide** — the handbook for testers validating the behavior, correctness, and stability of the Camp Fit Fur Dogs product **built on top of the Frank Framework**.

This guide documents **only product‑specific testing rules**.  
All framework‑level testing behavior (Frank.Testing, deterministic hosting, environment mutation, DI mutation, observability sinks, guardrail testing, etc.) is documented in the:

- **Frank Tester Guide**  
- **Frank Developer Guide**  
- **Frank User Guide**

Camp Fit Fur Dogs (CFFD) uses Frank’s testing harness.  
This guide explains how to test *this product* correctly.

---

# 1. Purpose of This Guide

This guide provides:

- Product‑specific test architecture  
- How to test CFFD features using Frank.Testing  
- Product‑specific test utilities and patterns  
- Product‑specific observability expectations  
- How to structure tests for new slices/features  
- What CFFD testers must validate (and what they must not)  

This guide does **not** restate Frank’s testing rules.  
Frank governs:

- deterministic test hosting  
- environment/config mutation  
- DI mutation  
- hosting provider mutation  
- startup module mutation  
- observability test sinks  
- guardrail testing  
- purity/determinism enforcement  

Refer to the Frank Tester Guide for all framework behavior.

---

# 2. Product Test Architecture

Camp Fit Fur Dogs uses a **vertical‑slice test structure**:

```
tests/
  CampFitFurDogs.Tests/
    <Feature>/
      Domain/
      Application/
      Api/
      Integration/
      Observability/
```

### 2.1 Test Types

CFFD tests include:

- **Domain tests** — invariants, rules, value objects  
- **Application tests** — handlers, validators, orchestrations  
- **API tests** — endpoint behavior, request/response shapes  
- **Integration tests** — repository behavior, EF Core mapping  
- **Observability tests** — product‑specific events/metrics  

### 2.2 What CFFD tests *do not* include

- Frank guardrail tests  
- Frank capability tests  
- Frank.Testing tests  
- Frank observability primitive tests  

Those belong in the Frank repository.

---

# 3. Testing CFFD With Frank.Testing

Frank.Testing provides:

- deterministic test hosting  
- environment mutation  
- configuration mutation  
- DI mutation  
- hosting provider mutation  
- startup module mutation  
- observability test sinks  

CFFD tests use:

```csharp
var factory = new MutatedWebApplicationFactory();
var client = factory.CreateClient();
```

### 3.1 Product‑Specific Environment Mutation

Examples:

- `CFFD__Database__ConnectionString`  
- `CFFD__Auth__Authority`  
- `CFFD__Auth__Audience`  

Tests may override these using Frank.Testing mutation APIs.

### 3.2 Product‑Specific Configuration Mutation

Examples:

- enabling/disabling feature flags  
- forcing specific booking rules  
- simulating operational modes  

### 3.3 Product‑Specific DI Mutation

Examples:

- replacing repositories with fakes  
- injecting test doubles for external services  
- overriding product‑specific abstractions  

---

# 4. Testing Product Behavior

## 4.1 Domain Tests

Domain tests validate:

- aggregate invariants  
- domain rules  
- value object correctness  
- domain event emission (product‑specific)  

Domain tests must be:

- pure  
- deterministic  
- isolated  

## 4.2 Application Tests

Application tests validate:

- handler behavior  
- validator behavior  
- repository abstraction usage  
- domain interaction  

Handlers must be tested through:

- commands  
- queries  
- cancellation tokens  
- product‑specific observability events  

## 4.3 API Tests

API tests validate:

- endpoint routing  
- request/response shapes  
- validation behavior  
- authorization behavior  
- correct dispatching  
- correct error shaping  
- product‑specific observability events  

API tests must **never**:

- bypass the dispatcher  
- call handlers directly  
- return domain entities  

## 4.4 Integration Tests

Integration tests validate:

- EF Core mapping  
- repository behavior  
- database interactions  
- transaction boundaries  
- product‑specific configuration behavior  

Integration tests must use:

- in‑memory or ephemeral databases  
- deterministic seeding  
- deterministic cleanup  

---

# 5. Product‑Specific Observability Testing

Frank provides observability primitives.  
CFFD defines **product‑specific events and metrics**.

### 5.1 What to Test

- correct event names  
- correct metric names  
- correct correlation propagation  
- correct event payloads (no secrets/PII)  
- correct emission timing (start/end/failure)  

### 5.2 Example Product Events

- `cffd.bookings.create.started`  
- `cffd.bookings.create.completed`  
- `cffd.bookings.create.failed`  
- `cffd.owners.register.started`  
- `cffd.owners.register.completed`  

### 5.3 Observability Test Rules

- never assert on timestamps  
- never assert on correlation ID values  
- assert on **presence**, not **format**  
- assert on event/metric **names**  
- assert on **payload shape**, not internal structure  

---

# 6. Adding Tests for New Features

When adding a new feature slice:

1. Add domain tests  
2. Add application tests  
3. Add API tests  
4. Add integration tests  
5. Add observability tests  
6. Add test utilities if needed  
7. Update product documentation if required  

### 6.1 Required Test Coverage

Every new feature must include:

- domain invariants  
- handler behavior  
- validator behavior  
- endpoint behavior  
- repository behavior  
- observability events  

---

# 7. What Camp Fit Fur Dogs Testers Should *Not* Do

- Do **not** test Frank itself  
- Do **not** bypass Frank.Testing  
- Do **not** use real environment variables  
- Do **not** use real hosting providers  
- Do **not** use real infrastructure  
- Do **not** rely on nondeterministic behavior  
- Do **not** write tests that depend on time  
- Do **not** assert on correlation ID values  
- Do **not** emit ad‑hoc logs or vendor‑specific metrics  

These break determinism and guardrails.

---

# 8. Summary

The Camp Fit Fur Dogs Tester Guide explains:

- how to test product features  
- how to structure domain, application, API, and integration tests  
- how to use Frank.Testing correctly  
- how to validate product‑specific observability  
- how to maintain deterministic, governed test behavior  

Frank provides the deterministic testing foundation.  
Camp Fit Fur Dogs provides the product logic built on top of it.
