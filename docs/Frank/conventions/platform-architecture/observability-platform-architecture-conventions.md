# Frank Observability Architecture Conventions

Observability is a first‑class Frank platform capability.  
All applications built on Frank must integrate with the observability subsystem exactly as defined here.

---

## 1. Architectural Purpose

Frank Observability provides:

- End‑to‑end correlation across all execution paths
- Structured event emission
- Vendor‑agnostic metrics
- Deterministic behavior for testing
- A unified context model for hosting, startup, modules, and infrastructure

Observability is a **platform primitive**, not an application feature.

---

## 2. Core Abstractions

Frank defines four canonical observability interfaces:

### IObservabilityContext
Immutable execution context containing:

- Correlation ID  
- Slice  
- Module  
- Environment  
- Timestamp  
- Metadata  

This context flows through all Frank engines and all consuming applications.

### ICorrelationContext
Responsible for:

- Creating correlation IDs  
- Propagating them across async boundaries  
- Reading/writing correlation headers  
- Ensuring deterministic correlation in tests  

### ITraceEvents
Structured event emitter with:

- Event name  
- Category  
- Severity  
- Payload  
- Automatic correlation injection  

### IMetrics
Vendor‑agnostic metrics primitives:

- Counters  
- Gauges  
- Timers  
- Duration measurement  

---

## 3. Hosting Engine Integration

Frank.Hosting must:

- Create the initial `IObservabilityContext`
- Attach it to the request pipeline
- Ensure correlation IDs exist before any middleware runs
- Propagate context into:
  - Startup modules  
  - Hosting modules  
  - Background jobs  
  - Infrastructure adapters  

No module may create its own observability context.

---

## 4. Startup Engine Integration

Frank.Startup must:

- Accept `IObservabilityContext` during module initialization
- Emit structured events for module load/unload
- Emit metrics for module initialization duration
- Propagate correlation IDs into module constructors

---

## 5. Module Integration

All Frank modules must:

- Accept `IObservabilityContext` as an input dependency
- Emit structured events for module‑level actions
- Emit metrics for module‑level operations
- Propagate correlation IDs to downstream components

---

## 6. Infrastructure Integration

Frank infrastructure components must:

- Propagate correlation IDs to external systems
- Emit structured events for:
  - External calls  
  - Retries  
  - Failures  
  - Latency  
- Emit metrics for:
  - Duration  
  - Success/failure counts  

---

## 7. Determinism Requirements

Frank Observability must be:

- Deterministic in tests  
- Deterministic in module initialization  
- Deterministic in hosting pipelines  

This is required for reproducible test harness behavior.

---

## 8. Forbidden Architectural Practices

- Manual correlation ID generation  
- Mutable observability context  
- Stopwatch timing  
- Ad‑hoc logging for structured events  
- Bypassing the hosting engine for context creation  
