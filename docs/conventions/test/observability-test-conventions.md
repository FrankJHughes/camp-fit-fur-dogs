# CampFitFurDogs — Observability Test Conventions  
*How CampFitFurDogs must test its usage of the Frank Observability platform.*

These conventions define how CampFitFurDogs verifies correct usage of:

- `ITraceEvents` (structured events)  
- `IMetrics` (counters, gauges, timers)  
- correlation propagation  
- deterministic observability behavior  

Tests must ensure that observability is **correct**, **deterministic**, and **aligned with Frank’s observability primitives**.

---

# 1. Test Harness Usage

CampFitFurDogs tests must:

- use the **Frank test harness**  
- use deterministic **event sinks** and **metric sinks**  
- use deterministic **correlation IDs**  
- avoid mocking observability interfaces  

Tests must **not**:

- mock `ITraceEvents`  
- mock `IMetrics`  
- rely on vendor‑specific exporters  
- rely on real infrastructure  

The Frank test harness provides:

- deterministic correlation context  
- deterministic event/metric capture  
- deterministic time abstractions  

---

# 2. Correlation Tests

Tests must verify that the **correlation ID propagates end‑to‑end** through:

- API handlers  
- Application handlers  
- domain services  
- infrastructure adapters  
- outbox handlers  

Tests must assert that correlation IDs appear in:

- emitted events  
- emitted metrics  

Tests must **not**:

- generate correlation IDs manually  
- override correlation IDs  
- mutate correlation context  

Correlation must always come from Frank’s correlation provider.

---

# 3. Event Tests

Tests must verify:

- events are emitted using `ITraceEvents`  
- event names follow naming conventions (`slice.module.action`)  
- payloads are **structured**, not embedded JSON  
- correlation ID is included automatically  

Tests must **not**:

- assert against vendor‑specific exporters  
- use ad‑hoc logging  
- assert against raw strings containing JSON  

Event tests must validate:

- correct event name  
- correct payload shape  
- correct correlation ID  
- correct severity (if applicable)  

---

# 4. Metric Tests

Tests must verify:

- metrics are emitted using `IMetrics`  
- metric names follow conventions (`slice.module.metric_name`)  
- timers use Frank abstractions (never `Stopwatch`)  
- counters increment deterministically  
- gauges update deterministically  

Tests must **not**:

- use `Stopwatch`  
- depend on real time  
- assert against vendor‑specific metric libraries  

Metric tests must validate:

- correct metric name  
- correct metric type  
- correct correlation ID (if applicable)  
- deterministic values  

---

# 5. Outbox Tests

Tests must verify:

- outbox handlers emit events  
- outbox handlers emit metrics  
- correlation ID flows from:
  - domain →  
  - outbox record →  
  - outbox handler →  
  - emitted events/metrics  

Outbox tests must ensure:

- no correlation ID is regenerated  
- no correlation ID is lost  
- event/metric emission is deterministic  

---

# 6. Forbidden Testing Practices

The following practices are **forbidden**:

- manual correlation ID creation  
- Stopwatch usage  
- vendor‑specific assertions  
- mocking Frank observability interfaces  
- asserting against raw log strings  
- asserting against non‑deterministic timestamps  
- bypassing the Frank test harness  

These practices violate:

- determinism  
- purity  
- observability invariants  
- Frank’s testing model  

---

# Summary

CampFitFurDogs observability tests must:

- use Frank’s deterministic test harness  
- verify correlation propagation  
- verify structured event emission  
- verify metric emission using Frank abstractions  
- validate naming conventions  
- validate deterministic behavior  
- avoid all forbidden patterns  

These conventions ensure:

- reliable observability  
- deterministic CI behavior  
- safe operational diagnostics  
- alignment with Frank’s observability platform  
