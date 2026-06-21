# Frank Testing — Developer Guide

This guide explains the internal architecture, invariants, extension points, and guardrails of the **Frank Testing capability**.  
It is intended for **Frank developers** — contributors who maintain or extend Frank.Testing, the test harness, or the deterministic hosting model.

If you are writing tests *using* Frank.Testing in an application, see the **Frank User Testing Guide** instead.  
If you are validating Frank’s behavior, see the **Frank Tester Testing Guide**.

---

# 1. Purpose of the Testing Capability

Frank.Testing provides a **deterministic, mutation‑driven test harness** that allows applications to be tested without:

- real environment variables  
- real hosting providers  
- real configuration sources  
- real startup modules  
- real DI containers  

The testing capability ensures:

- deterministic test hosting  
- deterministic environment mutation  
- deterministic configuration mutation  
- deterministic DI mutation  
- deterministic hosting provider mutation  
- deterministic startup module mutation  

Frank developers maintain these guarantees.

---

# 2. Architectural Overview

Frank.Testing is built around three core components:

## **2.1 MutatedWebApplicationFactory**
A deterministic test host that allows mutation of:

- environment  
- configuration  
- DI container  
- hosting provider  
- startup modules  

It wraps the real HostingEngine and StartupEngine but replaces all external inputs with controlled, test‑safe abstractions.

## **2.2 Mutation Model**
Mutations are applied in a strict order:

1. Environment mutations  
2. Configuration mutations  
3. Hosting provider mutations  
4. Startup module mutations  
5. DI mutations  

This order is **invariant** and must never change.

## **2.3 Test Harness Primitives**
These include:

- `EnvironmentMutation`
- `ConfigurationMutation`
- `ServiceMutation`
- `HostingProviderMutation`
- `StartupModuleMutation`

Each primitive is:

- pure  
- deterministic  
- isolated  
- composable  

---

# 3. Invariants (Must Never Be Broken)

Frank developers must preserve the following invariants:

### **3.1 Deterministic Startup**
The same mutations must always produce the same application state.

### **3.2 No Real Environment Access**
Tests must never read real environment variables.

### **3.3 No Real Hosting Providers**
Tests must never use real hosting providers.

### **3.4 No Real Configuration Sources**
Tests must never load:

- appsettings.json  
- secrets.json  
- environment variables  
- command‑line args  

### **3.5 No Static State**
The test harness must remain pure.

### **3.6 No Product Logic**
Frank.Testing must remain product‑agnostic.

---

# 4. Extension Points

Frank developers may extend the testing capability by adding:

## **4.1 New Mutation Types**
Examples:

- `LoggingMutation`
- `FeatureFlagMutation`
- `TimeProviderMutation`

Rules:

- must be deterministic  
- must be pure  
- must not access real environment  
- must not introduce nondeterminism  

## **4.2 New Hosting Providers (Test‑Only)**
These must:

- be deterministic  
- not depend on external systems  
- not require configuration  

## **4.3 New Startup Module Overrides**
These allow test‑specific startup behavior.

---

# 5. Guardrails

Frank.Testing enforces guardrails to prevent invalid test setups.

## **5.1 Environment Guardrails**
- accessing real environment variables throws  
- mutating environment after startup throws  

## **5.2 Configuration Guardrails**
- loading real configuration sources throws  
- overriding configuration after startup throws  

## **5.3 DI Guardrails**
- replacing services after startup throws  
- registering open generics incorrectly throws  

## **5.4 Hosting Guardrails**
- selecting a real hosting provider throws  
- selecting multiple hosting providers throws  

## **5.5 Startup Guardrails**
- startup modules must be deterministic  
- startup modules must not depend on real environment  

---

# 6. Testing Frank.Testing (Developer Responsibilities)

Frank developers must write tests that validate:

- mutation ordering  
- deterministic behavior  
- guardrail enforcement  
- hosting provider selection  
- startup module execution  
- DI mutation correctness  
- environment purity  
- configuration purity  

Tests live in:

```
tests/Frank.Testing.Tests/
```

---

# 7. Adding New Features to Frank.Testing

When adding a new feature:

1. Define the capability extension  
2. Document it here (developer guide)  
3. Document it in the user guide (if applicable)  
4. Document it in the tester guide  
5. Add unit tests  
6. Add integration tests  
7. Add guardrail tests  
8. Validate determinism  
9. Validate purity  
10. Validate capability boundaries  

No feature is complete without documentation and tests.

---

# 8. Anti‑Patterns (Never Allowed)

Frank developers must **never**:

- introduce nondeterminism  
- access real environment variables  
- access real configuration  
- use static state  
- use real hosting providers  
- bypass the mutation model  
- bypass guardrails  
- introduce product‑specific logic  

These violate Frank’s core guarantees.

---

# 9. Summary

The Frank Testing capability provides:

- deterministic test hosting  
- deterministic mutation of environment, configuration, DI, hosting, and startup  
- strict purity  
- strict guardrails  
- a unified test harness  

As a Frank developer, you are responsible for:

- preserving determinism  
- preserving purity  
- preserving guardrails  
- maintaining capability boundaries  
- documenting new features  
- writing tests for every change  

Frank.Testing is foundational to the reliability of every product built on Frank.

