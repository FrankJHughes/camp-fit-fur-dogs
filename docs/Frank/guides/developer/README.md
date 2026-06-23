# Frank Developer Guide  
Authoritative handbook for contributors to the Frank Framework

Welcome to the **Frank Developer Guide** — the authoritative handbook for developers who build, extend, or maintain the Frank Framework.  
This guide explains how Frank works internally, how to contribute safely, and how to extend the framework while preserving its architectural guarantees.

Frank is a **high‑discipline, capability‑oriented application framework** designed for deterministic behavior, strict purity, predictable hosting, and fully governed startup.  
As a Frank developer, you are responsible for maintaining these guarantees.

---

# 1. Purpose of This Guide

This guide provides:

- A high‑level understanding of Frank’s architecture  
- Rules and expectations for Frank contributors  
- How to extend Frank safely  
- How to maintain Frank’s purity and determinism  
- How to work with Frank’s capabilities  
- How to write tests for Frank itself  
- How to reason about Frank’s guardrails  
- How to integrate and extend **Frank Observability** (NEW)  

This guide is **not** about using Frank in a product.  
That belongs in the **Frank User Guide**.

This guide is about **building Frank itself**.

---

# 2. Frank’s Architectural Principles

Frank is built on a set of strict architectural principles:

## **2.1 Determinism**
Frank must behave the same way every time:

- deterministic startup  
- deterministic hosting provider selection  
- deterministic environment resolution  
- deterministic DI auto‑registration  
- deterministic pipeline execution  
- deterministic observability output (NEW)  

No randomness. No implicit behavior. No hidden state.

## **2.2 Purity**
Frank enforces purity at multiple levels:

- no direct environment variable access  
- no direct static access  
- no ambient state  
- no global singletons  
- no hidden dependencies  
- no ad‑hoc logging or metrics (NEW)  

Everything must be explicit, injectable, and testable.

## **2.3 Capability‑Oriented Design**
Frank is composed of **capabilities**, each with:

- a clear purpose  
- a clear boundary  
- a clear extension model  
- a clear set of invariants  
- a clear set of guardrails  
- a clear observability surface (NEW)  

Capabilities must remain isolated and composable.

## **2.4 Guardrails**
Frank enforces correctness through:

- startup validation  
- hosting provider validation  
- DI auto‑registration validation  
- environment validation  
- configuration validation  
- pipeline validation  
- observability validation (NEW)  

Guardrails must never be bypassed.

## **2.5 Product‑Agnosticism**
Frank must never contain:

- product logic  
- product configuration  
- product hosting assumptions  
- product‑specific DI  
- product‑specific environment variables  
- product‑specific observability events (NEW)  

Frank is a framework — not an application.

---

# 3. Frank’s High‑Level Architecture

Frank is composed of several major subsystems:

- **HostingEngine** — selects and configures hosting providers  
- **StartupEngine** — discovers and executes startup modules  
- **Environment Abstraction** — replaces environment variables with a pure interface  
- **Configuration System** — deterministic configuration layering  
- **DI Auto‑Registration Engine** — attribute‑driven DI registration  
- **Dispatcher Pipeline** — request pipeline with deterministic ordering  
- **Validation Scanner** — discovers validators and enforces validation rules  
- **Security Headers Middleware** — enforces standard security headers  
- **Error Boundary Middleware** — ensures safe error handling  
- **Observability Primitives** — structured events, metrics, correlation (NEW)  
- **Test Harness (Frank.Testing)** — deterministic test hosting and mutation  

Each subsystem is documented in its own capability guide:

```
docs/frank/guides/developer/<capability>/README.md
```

---

# 4. Developer Responsibilities

As a Frank developer, you must:

- Preserve determinism  
- Preserve purity  
- Preserve guardrails  
- Preserve capability boundaries  
- Preserve product‑agnosticism  
- Maintain backward compatibility when possible  
- Add tests for every change  
- Document every new capability  
- Emit structured, correlated observability events for all capability boundaries (NEW)  

Frank is a **governed framework** — not a free‑for‑all.

---

# 5. Extending Frank

When adding or modifying a capability, you must:

1. Understand the capability’s invariants  
2. Preserve its extension model  
3. Maintain its guardrails  
4. Add developer documentation  
5. Add user documentation (if applicable)  
6. Add tests in `Frank.*.Tests`  
7. Ensure no product‑specific logic leaks into Frank  
8. Ensure observability is integrated correctly (NEW)  

Every capability must remain:

- isolated  
- deterministic  
- composable  
- testable  
- observable (NEW)  

---

# 6. Observability in Frank (NEW)

Observability is a **Frank capability**, not a governance domain.  
It must be implemented consistently across all Frank subsystems.

Frank provides:

- `IObservabilityContext` — immutable correlation context  
- `ITraceEvents` — structured event emission  
- `IMetrics` — deterministic metrics  
- Correlation propagation middleware  
- Test sinks for deterministic observability testing  

### Observability Rules for Frank Developers

- No ad‑hoc logging  
- No vendor‑specific logging or metrics  
- No Stopwatch or real‑time timers  
- All events must follow `slice.module.action` naming  
- All metrics must follow `slice.module.metric_name` naming  
- No secrets, tokens, or PII in observability payloads  
- All capability boundaries must emit events  
- All external calls must emit events and metrics  
- All errors must emit structured error events  

Observability is required for:

- HostingEngine lifecycle  
- StartupEngine lifecycle  
- DI auto‑registration  
- Configuration loading  
- Pipeline execution  
- Error boundaries  
- Test harness execution  

---

# 7. Testing Frank

Frank has two test layers:

## **7.1 Unit Tests**
Located in:

```
tests/Frank.<Capability>.Tests/
```

These validate:

- invariants  
- guardrails  
- deterministic behavior  
- extension points  
- error conditions  
- observability emission (NEW)  

## **7.2 Integration Tests**
Located in:

```
tests/Frank.Testing.Tests/
```

These validate:

- HostingEngine behavior  
- StartupEngine behavior  
- DI auto‑registration  
- environment abstraction  
- configuration layering  
- pipeline execution  
- test harness behavior  
- observability propagation and determinism (NEW)  

Every capability must have both unit and integration coverage.

---

# 8. Working With Capabilities

Each capability has:

- a purpose  
- a boundary  
- invariants  
- extension points  
- a developer guide  
- a tester guide (if applicable)  
- a user guide (if applicable)  
- an observability surface (NEW)  

Capability guides live here:

```
docs/frank/guides/developer/<capability>/README.md
```

Examples:

- `hosting-engine/`  
- `startup-engine/`  
- `environment/`  
- `configuration/`  
- `observability/` (NEW)  
- `testing/`  

---

# 9. Adding a New Capability

To add a new capability:

1. Create a new folder under:

   ```
   docs/frank/guides/developer/<capability>/
   ```

2. Add a `README.md` describing:

   - purpose  
   - architecture  
   - invariants  
   - extension points  
   - guardrails  
   - observability surface (NEW)  
   - tests required  

3. Implement the capability in:

   ```
   src/Frank.<Capability>/
   ```

4. Add tests in:

   ```
   tests/Frank.<Capability>.Tests/
   ```

5. Add user‑facing documentation (if applicable):

   ```
   docs/frank/guides/user/<capability>/README.md
   ```

6. Add tester‑facing documentation (if applicable):

   ```
   docs/frank/guides/tester/<capability>/README.md
   ```

---

# 10. Developer Workflow

Frank uses a strict workflow:

1. **Design the capability**  
2. **Document the capability**  
3. **Implement the capability**  
4. **Test the capability**  
5. **Validate guardrails**  
6. **Validate observability** (NEW)  
7. **Submit a PR with:**
   - code  
   - tests  
   - documentation  

No capability is complete without documentation, tests, and observability.

---

# 11. Contributing to Frank

Contributions must:

- follow Frank conventions  
- follow Frank governance  
- include tests  
- include documentation  
- preserve architectural principles  
- preserve observability conventions (NEW)  

PRs that violate guardrails will be rejected.

---

# 12. Where to Go Next

Explore the capability guides:

```
docs/frank/guides/developer/<capability>/README.md
```

Or read the Frank User Guide:

```
docs/frank/guides/user/README.md
```

Or read the Frank Tester Guide:

```
docs/frank/guides/tester/README.md
```

Or explore **[Observability Conventions](ca://s?q=Open_observability_conventions)** (NEW).

---

# 13. Summary

The Frank Developer Guide is your handbook for:

- understanding Frank’s architecture  
- contributing safely  
- extending capabilities  
- maintaining guardrails  
- writing tests for Frank  
- documenting new features  
- integrating observability correctly (NEW)  

Frank is a governed, deterministic, capability‑oriented framework.  
As a developer, you are responsible for preserving its integrity.
