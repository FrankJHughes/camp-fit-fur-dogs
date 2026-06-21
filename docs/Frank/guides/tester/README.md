# Frank Tester Guide

Welcome to the **Frank Tester Guide** — the handbook for testers responsible for validating the correctness, determinism, guardrails, and capabilities of the Frank Framework itself.  
This guide explains how to test Frank’s internal behavior, how to validate its architectural guarantees, and how to ensure that Frank remains stable, predictable, and safe for all downstream products.

If you are testing an application *built with* Frank, see the product’s Tester Guide instead.  
This guide is specifically for testing **Frank as a framework**.

---

# 1. Purpose of This Guide

This guide provides:

- How to test Frank’s capabilities  
- How to validate Frank’s guardrails  
- How to test deterministic behavior  
- How to test hosting, startup, environment, and DI  
- How to test Frank.Testing itself  
- How to structure tests for new capabilities  
- How to ensure Frank remains product‑agnostic  

This guide is **not** about using Frank to test applications.  
That belongs in the **Frank User Testing Guide** under the Testing capability.

This guide is about **testing Frank itself**.

---

# 2. What Frank Testers Are Responsible For

Frank testers ensure that:

- Frank behaves deterministically  
- Frank enforces purity  
- Frank enforces guardrails  
- Frank capabilities behave as documented  
- Frank.Testing behaves deterministically  
- Frank’s hosting and startup engines behave consistently  
- Frank’s DI auto‑registration rules are enforced  
- Frank’s environment abstraction is respected  
- Frank’s validation pipeline behaves correctly  
- Frank’s error boundaries behave consistently  
- Frank’s security headers are always applied  

Frank testers validate the **framework**, not the applications built on it.

---

# 3. Types of Tests in Frank

Frank uses a layered testing strategy:

## **3.1 Unit Tests**
Located in:

```
tests/Frank.<Capability>.Tests/
```

Unit tests validate:

- invariants  
- guardrails  
- deterministic behavior  
- extension points  
- error conditions  
- boundary conditions  

Unit tests must be:

- isolated  
- deterministic  
- fast  
- pure  

---

## **3.2 Integration Tests**
Located in:

```
tests/Frank.Testing.Tests/
```

Integration tests validate:

- HostingEngine behavior  
- StartupEngine behavior  
- DI auto‑registration  
- environment abstraction  
- configuration layering  
- pipeline execution  
- test harness behavior  
- capability interactions  

Integration tests must:

- use MutatedWebApplicationFactory  
- avoid real infrastructure  
- avoid external dependencies  
- validate deterministic startup  

---

## **3.3 Guardrail Tests**
Guardrail tests ensure that Frank rejects invalid configurations.

Examples:

- missing environment variables  
- invalid hosting metadata  
- invalid DI registrations  
- invalid startup modules  
- invalid configuration layering  

Guardrail tests ensure Frank fails **loudly and predictably**.

---

# 4. Testing Frank’s Capabilities

Each capability has its own tester guide:

```
docs/frank/guides/tester/<capability>/README.md
```

Examples:

- `testing/` — how to test Frank.Testing  
- `hosting-engine/` — how to test hosting provider selection  
- `startup-engine/` — how to test startup module execution  
- `environment/` — how to test environment abstraction  
- `configuration/` — how to test configuration layering  
- `validation/` — how to test validators and validation scanning  
- `dispatching/` — how to test the dispatcher pipeline  

Each capability must have:

- unit tests  
- integration tests  
- guardrail tests  

---

# 5. Testing Determinism

Frank testers must ensure that:

- the same inputs always produce the same outputs  
- startup order is deterministic  
- hosting provider selection is deterministic  
- environment resolution is deterministic  
- DI auto‑registration is deterministic  
- configuration layering is deterministic  
- pipeline execution order is deterministic  

Tests must detect:

- nondeterministic behavior  
- race conditions  
- environment leaks  
- static state leaks  
- time‑dependent behavior  

---

# 6. Testing Purity

Frank testers validate that:

- no direct environment variable access exists  
- no static state is used  
- no ambient state is used  
- no global singletons exist  
- all dependencies are injectable  
- all capabilities remain pure  

Tests must fail if purity is violated.

---

# 7. Testing Guardrails

Frank testers validate that guardrails:

- detect invalid configuration  
- detect invalid DI registrations  
- detect invalid hosting metadata  
- detect invalid startup modules  
- detect invalid environment usage  
- detect invalid capability interactions  

Guardrail tests ensure Frank fails **early**, **loudly**, and **predictably**.

---

# 8. Testing Frank.Testing

Frank.Testing is itself a capability and must be tested.

Tests must validate:

- environment mutation  
- configuration mutation  
- DI mutation  
- hosting provider mutation  
- startup module mutation  
- deterministic test hosting  
- correct integration with HostingEngine  
- correct integration with StartupEngine  

Tests must ensure:

- no real environment variables leak  
- no real hosting providers leak  
- no real configuration leaks  
- no nondeterministic behavior occurs  

---

# 9. Adding Tests for New Capabilities

When a new capability is added:

1. Create a tester guide:

   ```
   docs/frank/guides/tester/<capability>/README.md
   ```

2. Add unit tests:

   ```
   tests/Frank.<Capability>.Tests/
   ```

3. Add integration tests:

   ```
   tests/Frank.Testing.Tests/<Capability>/
   ```

4. Add guardrail tests:

   ```
   tests/Frank.<Capability>.Tests/Guardrails/
   ```

5. Validate:

   - determinism  
   - purity  
   - guardrails  
   - capability boundaries  
   - extension points  

---

# 10. Tester Workflow

Frank testers follow a strict workflow:

1. Understand the capability  
2. Understand its invariants  
3. Understand its guardrails  
4. Write unit tests  
5. Write integration tests  
6. Write guardrail tests  
7. Validate determinism  
8. Validate purity  
9. Validate capability boundaries  
10. Validate documentation accuracy  

Every change to Frank must include tests.

---

# 11. What Frank Testers Should *Not* Do

Frank testers should **not**:

- test product‑specific behavior  
- bypass the test harness  
- use real environment variables  
- use real hosting providers  
- use real infrastructure  
- rely on nondeterministic behavior  
- write tests that depend on time  
- write tests that depend on external services  

Frank tests must remain:

- deterministic  
- isolated  
- pure  
- product‑agnostic  

---

# 12. Summary

The Frank Tester Guide is your handbook for:

- validating Frank’s capabilities  
- enforcing determinism  
- enforcing purity  
- enforcing guardrails  
- testing Frank.Testing  
- testing hosting, startup, environment, DI, and pipeline behavior  
- writing unit, integration, and guardrail tests  
- ensuring Frank remains safe for all downstream products  

Frank testers protect the integrity of the framework.  
Your work ensures that every product built on Frank inherits a stable, deterministic, governed foundation.

