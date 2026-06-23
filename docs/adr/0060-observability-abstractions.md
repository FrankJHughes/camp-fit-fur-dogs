# ADR-0060 — Observability Abstractions

## Status  
Accepted

## Context  
The platform lacked a unified, vendor‑agnostic observability model. Prior to this decision:

- Correlation IDs were inconsistently generated and propagated.
- Structured events were emitted ad‑hoc with no shared schema.
- Metrics were scattered across libraries with no common abstraction.
- Hosting modules could not reliably attach observability metadata.
- Tests could not assert correlation propagation or event/metric behavior in a deterministic way.

As the system grows (multi‑tenant hosting, distributed modules, background workers, and external integrations), observability becomes a cross‑cutting requirement. We need a consistent, composable, testable abstraction layer that works across:

- API requests  
- Hosting engine modules  
- Domain events  
- Background jobs  
- Infrastructure components  
- Test harnesses  

This ADR defines that layer.

## Decision  
We introduce four unified observability interfaces:

### IObservabilityContext  
A structured context object representing the current execution scope. It includes:

- Correlation ID  
- Slice  
- Module  
- Environment  
- Timestamp  
- Arbitrary metadata  

This object flows through the hosting pipeline and is available to all components.

### ICorrelationContext  
Responsible for:

- Creating correlation IDs  
- Propagating them across async boundaries  
- Reading/writing them from HTTP headers  
- Ensuring deterministic correlation behavior in tests  

This guarantees consistent correlation semantics across the entire platform.

### ITraceEvents  
A structured event emitter with:

- Event name  
- Category  
- Severity  
- Payload  
- Automatic correlation injection  

This replaces ad‑hoc logging with a consistent, structured event model.

### IMetrics  
A vendor‑agnostic metrics abstraction supporting:

- Counters  
- Gauges  
- Timers  
- Duration measurement  

This allows metrics to be emitted consistently regardless of backend (Prometheus, OTEL, etc.).

### Integration  
These abstractions are integrated into:

- The hosting engine  
- The API middleware pipeline  
- The test harness (via ObservabilityTestBase)  
- Domain and infrastructure layers  

This ensures observability is consistent across all execution paths.

## Consequences  

### Positive  
- Unified observability model across the entire platform.  
- Deterministic correlation propagation, enabling reliable testing.  
- Structured events with consistent schema.  
- Vendor‑agnostic metrics, enabling backend flexibility.  
- Improved debuggability and traceability across modules.  
- Cleaner hosting engine integration.  
- Better test harness support, including correlation tests and event/metric assertions.

### Negative  
- Requires updates to existing modules to adopt the new abstractions.  
- Introduces a new cross‑cutting dependency that must be respected across layers.  
- Requires documentation updates (architecture, conventions, story references).

### Neutral  
- Does not dictate any specific observability backend (OTEL, Prometheus, etc.).  
- Does not replace logging; it complements it with structured events and metrics.
