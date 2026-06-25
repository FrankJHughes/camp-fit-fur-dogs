# Frank — Conventions — Platform Architecture — Observability  
*Observability as a first‑class platform capability.*

Observability in Frank is a **platform‑level primitive**.  
All applications built on Frank — including CampFitFurDogs — must integrate with the observability subsystem **exactly** as defined here.

Frank provides the **unified, deterministic, cross‑cutting observability foundation** for:

- hosting  
- startup  
- modules  
- infrastructure  
- application pipelines  
- test harness  

Observability is **not** an application feature.  
It is a **platform responsibility**.

---

# 1. Architectural Purpose

Frank Observability provides:

- **end‑to‑end correlation** across all execution paths  
- **structured event emission**  
- **vendor‑agnostic metrics**  
- **deterministic behavior** for testing  
- a **unified context model** for hosting, startup, modules, and infrastructure  

Observability ensures:

- every request is traceable  
- every operation is correlated  
- every failure is diagnosable  
- every metric is contextual  
- every environment behaves consistently  

---

# 2. Core Abstractions

Frank defines four canonical observability interfaces.

---

## IObservabilityContext

Immutable execution context containing:

- Correlation ID  
- Slice  
- Module  
- Environment  
- Timestamp  
- Metadata  

This context flows through:

- HostingEngine  
- StartupEngine  
- Modules  
- Infrastructure  
- Test harness  
- All consuming applications  

It is the **single source of truth** for observability.

---

## ICorrelationContext

Responsible for:

- creating correlation IDs  
- propagating them across async boundaries  
- reading/writing correlation headers  
- ensuring deterministic correlation in tests  

Applications must **never** generate correlation IDs manually.

---

## ITraceEvents

Structured event emitter with:

- event name  
- category  
- severity  
- payload  
- automatic correlation injection  

Events must be:

- structured  
- typed  
- deterministic  
- correlation‑aware  

---

## IMetrics

Vendor‑agnostic metrics primitives:

- counters  
- gauges  
- timers  
- duration measurement  

Metrics must be:

- deterministic  
- structured  
- correlation‑aware  

---

# 3. Hosting Engine Integration

`Frank.Hosting` must:

- create the initial `IObservabilityContext`  
- attach it to the request pipeline  
- ensure correlation IDs exist **before any middleware runs**  
- propagate context into:
  - Startup modules  
  - Hosting modules  
  - Background jobs  
  - Infrastructure adapters  

No module may create its own observability context.

HostingEngine is the **root of observability**.

---

# 4. Startup Engine Integration

`Frank.Startup` must:

- accept `IObservabilityContext` during module initialization  
- emit structured events for module load/unload  
- emit metrics for module initialization duration  
- propagate correlation IDs into module constructors  

StartupEngine is the **lifecycle observability layer**.

---

# 5. Module Integration

All Frank modules must:

- accept `IObservabilityContext` as an input dependency  
- emit structured events for module‑level actions  
- emit metrics for module‑level operations  
- propagate correlation IDs to downstream components  

Modules must **never**:

- mutate observability context  
- generate correlation IDs  
- bypass HostingEngine  

---

# 6. Infrastructure Integration

Frank infrastructure components must:

- propagate correlation IDs to external systems  
- emit structured events for:
  - external calls  
  - retries  
  - failures  
  - latency  
- emit metrics for:
  - duration  
  - success/failure counts  

Infrastructure must **never**:

- use vendor‑specific logging  
- use vendor‑specific metrics  
- generate correlation IDs  

---

# 7. Determinism Requirements

Frank Observability must be:

- deterministic in tests  
- deterministic in module initialization  
- deterministic in hosting pipelines  

This is required for:

- reproducible test harness behavior  
- stable CI  
- predictable module behavior  

Determinism is a **platform guarantee**.

---

# 8. Forbidden Architectural Practices

The following practices are **forbidden** in Frank:

- manual correlation ID generation  
- mutable observability context  
- Stopwatch timing  
- ad‑hoc logging for structured events  
- bypassing HostingEngine for context creation  
- vendor‑specific logging or metrics  
- embedding JSON inside strings  
- non‑deterministic timing or ID generation  

These violate:

- ADR‑0060  
- platform purity  
- determinism guarantees  

---

# Summary

Frank Observability is:

- unified  
- deterministic  
- structured  
- correlation‑aware  
- platform‑level  
- product‑agnostic  

It provides the **observability backbone** for all applications built on Frank.

All consuming products — including CampFitFurDogs — must integrate with it **exactly** as defined here.
