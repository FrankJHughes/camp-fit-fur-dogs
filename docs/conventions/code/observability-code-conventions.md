# CampFitFurDogs - Observability - Code Conventions  
*How CampFitFurDogs code must use the Frank Observability platform.*

These conventions define how CampFitFurDogs integrates with Frank’s observability primitives:

- `ITraceEvents` (structured events)  
- `IMetrics` (counters, gauges, timers)  
- correlation propagation  
- immutable observability context  

These conventions apply to **all backend code** across Api, Application, Domain, and Infrastructure.

---

# 1. Event Naming

CampFitFurDogs must follow the Frank event naming pattern:

````text
slice.module.action
````

Examples:

- `customer.registration.created`  
- `infra.email.sent`  
- `api.owner.login_failed`  

Event names must be:

- lowercase  
- dot‑separated  
- stable  
- slice‑aligned  

---

# 2. Metric Naming

CampFitFurDogs must follow the Frank metric naming pattern:

````text
slice.module.metric_name
````

Examples:

- `api.login.duration_ms`  
- `infra.outbox.retry_count`  
- `domain.customer.created_count`  

Metric names must be:

- lowercase  
- dot‑separated  
- stable  
- slice‑aligned  

---

# 3. Event Emission Rules

CampFitFurDogs must:

- emit events using `ITraceEvents`  
- use **structured payloads** (no embedded JSON strings)  
- rely on Frank for correlation ID injection  
- include the following fields:
  - slice  
  - module  
  - action  
  - severity  
  - payload (structured object)  

CampFitFurDogs must **not**:

- use ad‑hoc logging  
- use `Console.WriteLine`  
- use vendor‑specific logging APIs  
- emit unstructured strings  

Events must be:

- structured  
- typed  
- deterministic  
- correlation‑aware  

---

# 4. Metric Emission Rules

CampFitFurDogs must:

- use `IMetrics` for all metrics  
- use Frank timers (never `Stopwatch`)  
- emit:
  - counters  
  - gauges  
  - timers  

CampFitFurDogs must **not**:

- use `Stopwatch`  
- use vendor‑specific metric libraries  
- compute durations manually  

Metrics must be:

- deterministic  
- structured  
- slice‑aligned  

---

# 5. Correlation Rules

CampFitFurDogs must:

- accept correlation ID from Frank  
- propagate correlation ID through:
  - domain services  
  - infrastructure calls  
  - outbox handlers  
- never generate correlation IDs manually  

Correlation IDs must be:

- stable for the request  
- automatically injected  
- automatically propagated  

CampFitFurDogs must **not**:

- create new correlation IDs  
- override correlation IDs  
- mutate correlation context  

---

# 6. Forbidden Code Practices

The following practices are **forbidden**:

- manual correlation ID creation  
- manual timing (`Stopwatch`, `DateTime.UtcNow` deltas)  
- ad‑hoc logging  
- vendor‑specific logging APIs  
- mutable observability context  
- embedding JSON inside strings  
- emitting events without structured payloads  

These practices violate:

- purity rules  
- observability invariants  
- deterministic behavior guarantees  

---

# Summary

CampFitFurDogs must use Frank’s observability primitives:

- `ITraceEvents` for structured events  
- `IMetrics` for counters, gauges, and timers  
- Frank‑managed correlation IDs  
- immutable observability context  

CampFitFurDogs must **never**:

- log manually  
- time manually  
- generate correlation IDs  
- use vendor APIs  
- emit unstructured events  

These conventions ensure:

- deterministic observability  
- consistent event and metric naming  
- safe cross‑service correlation  
- clean operational behavior  
- alignment with Frank’s observability platform  
