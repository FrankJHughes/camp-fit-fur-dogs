# Frank — Tester Guide — Observability  
*How to validate Frank’s correlation, events, metrics, and engine integration.*

This guide explains how testers validate the correctness, determinism, and integration behavior of the Frank Observability subsystem across hosting, startup, modules, domain, and infrastructure.

Frank Observability is a **platform‑level capability**.  
Testers ensure that all engines and all consuming applications follow the rules defined by ADR‑0060.

---

# 1. Purpose of Observability Testing

Testers ensure:

- **correlation is deterministic** and correctly propagated  
- **events** are emitted with correct names, payloads, and correlation  
- **metrics** are emitted with correct names and deterministic timing  
- **engines integrate correctly** (hosting, startup, modules, infrastructure)  
- **no forbidden patterns** exist in the codebase  

Observability tests validate **platform correctness**, not product behavior.

---

# 2. Test Harness Requirements

`Frank.Testing` provides:

- deterministic correlation ID generation  
- deterministic event sinks  
- deterministic metric sinks  
- real hosting pipeline execution  
- real startup engine execution  

Testers must **not** mock observability interfaces unless testing the interfaces themselves.

Tests must run against the **real observability pipeline**, not mocks.

---

# 3. Correlation Tests

Tests must verify:

- correlation ID creation is deterministic  
- correlation flows through:
  - HostingEngine  
  - StartupEngine  
  - modules  
  - domain services  
  - infrastructure adapters  
  - outbox handlers  
- correlation ID appears in **all** events and metrics  

Tests must **not**:

- generate correlation IDs manually  
- override correlation IDs  
- mutate observability context  

Correlation must always come from Frank’s correlation provider.

---

# 4. Event Tests

Tests must verify:

- events are emitted using `ITraceEvents`  
- event names follow `slice.module.action`  
- payloads are structured and serializable  
- correlation ID is injected automatically  

Tests must **not**:

- assert against vendor‑specific exporters  
- use ad‑hoc logging  
- assert against raw JSON strings  

Event tests validate:

- correct event name  
- correct payload shape  
- correct correlation ID  
- correct severity (if applicable)  

---

# 5. Metric Tests

Tests must verify:

- metrics are emitted using `IMetrics`  
- metric names follow `slice.module.metric_name`  
- timers use Frank abstractions (never `Stopwatch`)  
- counters increment deterministically  
- gauges update deterministically  

Tests must **not**:

- depend on real time  
- use `Stopwatch`  
- assert against vendor‑specific metric libraries  

Metric tests validate:

- correct metric name  
- correct metric type  
- deterministic values  
- correlation ID (if applicable)  

---

# 6. Engine Integration Tests

Testers must validate:

### **HostingEngine**
- creates the initial `IObservabilityContext`  
- propagates correlation before any middleware runs  
- emits lifecycle events  

### **StartupEngine**
- receives and propagates correlation  
- emits module load/unload events  
- emits metrics for module initialization  

### **Modules**
- accept and propagate correlation  
- emit structured events  
- emit metrics for module‑level operations  

### **Infrastructure Adapters**
- propagate correlation to external systems  
- emit events for external calls, retries, failures  
- emit metrics for latency and success/failure  

Integration tests validate **end‑to‑end observability behavior**.

---

# 7. Forbidden Testing Practices

Tests must **reject**:

- manual correlation ID creation  
- Stopwatch usage  
- vendor‑specific assertions  
- mocking Frank observability interfaces  
- asserting against real exporters  
- asserting against non‑deterministic timestamps  
- bypassing the Frank test harness  

These violate:

- ADR‑0060  
- determinism guarantees  
- platform purity  

---

# 8. Tester Responsibilities Summary

Testers must:

- validate determinism  
- validate naming conventions  
- validate correlation propagation  
- validate event correctness  
- validate metric correctness  
- validate engine integration  
- ensure no forbidden patterns exist  

Frank Observability defines the rules;  
**testers ensure they are followed.**
