# Frank Testing — Tester Guide

This guide explains how to **test the Frank Testing capability itself** — including the deterministic test harness, mutation model, hosting behavior, startup behavior, and guardrails.  
It is intended for **Frank testers**, whose responsibility is to validate the correctness, determinism, and purity of Frank.Testing.

If you are writing tests *using* Frank.Testing in an application, see the **Frank User Testing Guide** instead.  
If you are extending Frank.Testing, see the **Frank Developer Testing Guide**.

---

# 1. Purpose of the Testing Capability (Tester Perspective)

Frank.Testing provides a deterministic test harness that ensures:

- no real environment variables are used  
- no real hosting providers are used  
- no real configuration sources are loaded  
- no real startup modules leak into tests  
- no nondeterministic behavior occurs  
- all mutations are applied in a strict, predictable order  

As a tester, your job is to **prove** that these guarantees hold.

---

# 2. What Frank Testers Validate

Frank testers validate:

## **2.1 Determinism**
- same mutations → same application state  
- same inputs → same outputs  
- no nondeterministic ordering  
- no time‑dependent behavior  

## **2.2 Purity**
- no static state  
- no ambient state  
- no direct environment variable access  
- no real configuration access  

## **2.3 Guardrails**
- invalid test setups fail loudly  
- invalid DI mutations fail  
- invalid hosting mutations fail  
- invalid startup mutations fail  
- invalid configuration mutations fail  

## **2.4 Capability Boundaries**
- testing capability does not leak into other capabilities  
- hosting behavior remains isolated  
- startup behavior remains isolated  

## **2.5 Product‑Agnosticism**
Frank.Testing must not depend on:

- product code  
- product configuration  
- product hosting providers  
- product environment variables  

---

# 3. Test Types Required for Frank.Testing

Frank testers must write three categories of tests:

## **3.1 Unit Tests**
Validate:

- mutation primitives  
- guardrail enforcement  
- deterministic ordering  
- error conditions  
- boundary conditions  

Unit tests must be:

- pure  
- isolated  
- fast  
- deterministic  

---

## **3.2 Integration Tests**
Validate:

- HostingEngine behavior under mutation  
- StartupEngine behavior under mutation  
- DI mutation behavior  
- environment mutation behavior  
- configuration mutation behavior  
- pipeline behavior under test hosting  

Integration tests must:

- use `MutatedWebApplicationFactory`  
- avoid real infrastructure  
- avoid real environment variables  
- avoid real configuration sources  

---

## **3.3 Guardrail Tests**
Validate that Frank.Testing **rejects** invalid setups:

- accessing real environment variables  
- loading real configuration  
- selecting real hosting providers  
- mutating DI after startup  
- mutating environment after startup  
- mutating configuration after startup  
- registering invalid services  
- using nondeterministic startup modules  

Guardrail tests ensure Frank fails **early**, **loudly**, and **predictably**.

---

# 4. Testing the Mutation Model

Frank testers must validate the **strict mutation order**:

1. Environment mutations  
2. Configuration mutations  
3. Hosting provider mutations  
4. Startup module mutations  
5. DI mutations  

Tests must ensure:

- order cannot be changed  
- order cannot be bypassed  
- order cannot be influenced by product code  
- order is deterministic across runs  

---

# 5. Testing MutatedWebApplicationFactory

Testers must validate:

## **5.1 Deterministic Hosting**
- same mutations → same hosting provider  
- no real hosting providers leak  
- hosting metadata is respected  

## **5.2 Deterministic Startup**
- startup modules execute in deterministic order  
- startup modules cannot access real environment  
- startup modules cannot load real configuration  

## **5.3 Deterministic DI**
- DI mutations override services predictably  
- DI mutations cannot occur after startup  
- DI mutations cannot introduce nondeterminism  

## **5.4 Deterministic Environment**
- environment mutations override values predictably  
- real environment variables are never read  
- environment cannot be mutated after startup  

## **5.5 Deterministic Configuration**
- configuration mutations override values predictably  
- real configuration sources are never loaded  
- configuration cannot be mutated after startup  

---

# 6. Required Test Coverage Areas

Frank testers must ensure coverage for:

## **6.1 Happy Path**
- valid mutations  
- valid hosting provider selection  
- valid startup module execution  
- valid DI overrides  
- valid environment overrides  
- valid configuration overrides  

## **6.2 Failure Path**
- invalid mutations  
- invalid hosting provider selection  
- invalid startup modules  
- invalid DI overrides  
- invalid environment access  
- invalid configuration access  

## **6.3 Edge Cases**
- empty mutation sets  
- conflicting mutations  
- duplicate mutations  
- mutation ordering conflicts  
- mutation of unregistered services  
- mutation of sealed services  

---

# 7. Anti‑Patterns (Tests Must Reject These)

Frank testers must ensure tests **fail** when:

- real environment variables are accessed  
- real configuration is loaded  
- real hosting providers are used  
- static state is introduced  
- nondeterministic behavior is introduced  
- mutation order is violated  
- guardrails are bypassed  
- product logic leaks into Frank.Testing  

These are **hard failures**, not warnings.

---

# 8. Adding Tests for New Testing Features

When Frank developers add new testing features, testers must:

1. Read the developer guide for the capability  
2. Identify invariants  
3. Identify guardrails  
4. Write unit tests  
5. Write integration tests  
6. Write guardrail tests  
7. Validate determinism  
8. Validate purity  
9. Validate capability boundaries  
10. Validate documentation accuracy  

No feature is complete until testers approve it.

---

# 9. Summary

The Frank Testing capability must remain:

- deterministic  
- pure  
- isolated  
- governed  
- product‑agnostic  

Frank testers ensure:

- mutation ordering is correct  
- guardrails are enforced  
- hosting and startup behavior are deterministic  
- DI, environment, and configuration mutations behave predictably  
- no nondeterminism is introduced  
- no real environment or configuration leaks  
- no product logic contaminates the framework  

Your work ensures that every product built on Frank inherits a stable, deterministic, testable foundation.

