# Frank — Developer Guide — Observability  
*How developers integrate with Frank’s unified observability subsystem.*

Frank Observability is a **platform‑level capability** providing unified correlation, structured events, and vendor‑agnostic metrics across all Frank engines and all applications built on Frank.  
Developers **consume** these capabilities — they do **not** extend or redefine them.

This guide explains how developers integrate with and use the observability subsystem.

---

# 1. Purpose of Frank Observability

Frank Observability ensures:

- **end‑to‑end correlation** across hosting, startup, modules, domain, and infrastructure  
- **structured, consistent event emission**  
- **vendor‑agnostic metrics** with deterministic timing  
- **deterministic behavior** for testing  
- a unified context model (`IObservabilityContext`) that flows through the entire platform  

Observability is a **platform primitive**, not an application feature.

---

# 2. Core Abstractions

## `IObservabilityContext`

Immutable execution context containing:

- Correlation ID  
- Slice  
- Module  
- Environment  
- Timestamp  
- Metadata  

Developers must **never** mutate or recreate this context.

---

## `ICorrelationContext`

Responsible for:

- creating correlation IDs  
- propagating IDs across async boundaries  
- reading/writing correlation headers  

Developers must **never** generate correlation IDs manually.

---

## `ITraceEvents`

Structured event emitter with:

- event name  
- category  
- severity  
- payload  
- automatic correlation injection  

Events must be:

- structured  
- typed  
- correlation‑aware  
- deterministic  

---

## `IMetrics`

Vendor‑agnostic metrics primitives:

- counters  
- gauges  
- timers  
- duration measurement  

Metrics must be:

- deterministic  
- correlation‑aware  
- structured  

---

# 3. Integration Points for Developers

Frank Observability flows through **all layers**.  
Developers integrate with it — they do not modify it.

---

## 3.1 API Handlers

Developers must:

- accept the `IObservabilityContext` from the hosting pipeline  
- emit events for:
  - request start  
  - request end  
  - validation  
  - domain calls  
  - failures  
- emit metrics for handler duration  

API handlers must **not**:

- generate correlation IDs  
- use ad‑hoc logging  
- use Stopwatch  

---

## 3.2 Domain Services

Developers must:

- propagate the observability context  
- emit events for domain actions  
- emit metrics for domain operation duration  

Domain services must **not**:

- mutate observability context  
- embed JSON in event payloads  

---

## 3.3 Infrastructure Adapters

Developers must:

- propagate correlation IDs to external systems  
- emit events for:
  - external calls  
  - retries  
  - failures  
- emit metrics for:
  - latency  
  - success/failure counts  

Infrastructure must **not**:

- use vendor‑specific logging  
- use vendor‑specific metrics  

---

## 3.4 Outbox Handlers

Developers must:

- emit events for outbox processing  
- emit metrics for:
  - handler duration  
  - retry counts  
- propagate correlation IDs from:
  - domain →  
  - outbox record →  
  - outbox handler  

Outbox handlers must **not**:

- generate new correlation IDs  
- drop correlation IDs  

---

# 4. Event Naming Rules

Event names must follow:

````text
slice.module.action
````

Examples:

- `api.owner.login_failed`  
- `infra.email.sent`  
- `domain.customer.created`  

Event names must be:

- lowercase  
- dot‑separated  
- stable  
- slice‑aligned  

---

# 5. Metric Naming Rules

Metric names must follow:

````text
slice.module.metric_name
````

Examples:

- `api.login.duration_ms`  
- `infra.outbox.retry_count`  

Metric names must be:

- lowercase  
- dot‑separated  
- stable  
- slice‑aligned  

---

# 6. Forbidden Developer Practices

Developers must **never**:

- generate correlation IDs manually  
- use Stopwatch timing  
- use ad‑hoc logging  
- use vendor‑specific logging/metrics APIs  
- mutate observability context  
- embed JSON inside strings  
- bypass Frank observability primitives  

These violate:

- ADR‑0060  
- determinism guarantees  
- platform purity  

---

# 7. Developer Responsibilities Summary

Developers must:

- use Frank abstractions exclusively  
- follow event and metric naming conventions  
- propagate context correctly  
- emit structured events and metrics  
- avoid all forbidden patterns  

Frank Observability provides the platform;  
**developers integrate with it — they do not redefine it.**
