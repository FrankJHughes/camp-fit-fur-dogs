# ADR‑0060 — Observation Abstractions

## Status  
Accepted

## Context  
The platform lacked a unified, vendor‑agnostic observation model. Prior to this decision:

- Correlation IDs were inconsistently generated and propagated.
- Structured events were emitted ad‑hoc with no shared schema.
- Metrics were scattered across libraries with no common abstraction.
- Hosting modules could not reliably attach observation metadata.
- Tests could not assert correlation propagation or event/metric behavior in a deterministic way.

As the system grows (multi‑tenant hosting, distributed modules, background workers, and external integrations), observation becomes a cross‑cutting requirement. We need a consistent, composable, testable abstraction layer that works across:

- API requests  
- Hosting engine modules  
- Domain pipelines  
- Background jobs  
- Infrastructure components  
- Test harnesses  

This ADR defines that layer.

---

## Decision  
We introduce a unified set of **Observation** abstractions under:

- `Frank.Abstractions.Observations`
- `Frank.Infrastructure.Observations`

These abstractions provide correlation, structured events, metrics, and context propagation across all execution paths.

### 1. IObservationContext  
A structured context object representing the current execution scope. It includes:

- CorrelationId  
- Channel  
- Agent  
- Environment  
- Timestamp  
- Metadata  

This object flows through the hosting pipeline and is available to all components.

### 2. IRequestObservationContext  
A request‑scope context that extends `IObservationContext` with:

- `UserId` (nullable)  
- Request metadata (path, method, etc.)

### 3. SystemObservationContext  
A system‑scope context used globally across the application for:

- Startup  
- Discovery  
- Background jobs  
- Domain pipelines  
- Context builders  

It is created by:

````csharp
SystemObservationContext.Create(channel, agent, environment, metadata)
````

### 4. ObservationContextBase  
A shared base class for all Observation contexts, providing:

- unified metadata model  
- canonical timestamp behavior  
- environment propagation  
- correlation propagation  

### 5. ICorrelationContext  
Responsible for:

- creating correlation IDs  
- propagating them across async boundaries  
- reading/writing them from HTTP headers  
- ensuring deterministic correlation behavior in tests  

### 6. IObservationSink  
A structured event emitter with:

- event name  
- category  
- severity  
- payload  
- automatic correlation injection  

This replaces ad‑hoc logging with a consistent, structured event model.

### 7. IMetrics  
A vendor‑agnostic metrics abstraction supporting:

- counters  
- gauges  
- timers  
- duration measurement  

This allows metrics to be emitted consistently regardless of backend (Prometheus, OTEL, etc.).

### 8. Middleware Integration  
Two middleware components integrate Observation into the HTTP pipeline:

- `InboundObservationContextMiddleware`  
- `ObservationInstrumentationMiddleware`  

These handle:

- correlation extraction  
- context creation  
- event emission  
- metric emission  
- error boundary observation  

### 9. Outbound Integration  
Outbound HTTP calls propagate Observation context via:

- `OutboundObservationContextHandler`

This ensures correlation continuity across external calls.

---

## Consequences  

### Positive  
- Unified Observation model across the entire platform.  
- Deterministic correlation propagation, enabling reliable testing.  
- Structured events with consistent schema.  
- Vendor‑agnostic metrics, enabling backend flexibility.  
- Improved debuggability and traceability across modules.  
- Cleaner hosting engine integration.  
- Better test harness support, including correlation tests and event/metric assertions.  
- SystemObservationContext provides global system‑scope observability.

### Negative  
- Requires updates to existing modules to adopt the new abstractions.  
- Introduces a new cross‑cutting dependency that must be respected across layers.  
- Requires documentation updates (architecture, conventions, story references).

### Neutral  
- Does not dictate any specific observation backend (OTEL, Prometheus, etc.).  
- Does not replace logging; it complements it with structured events and metrics.

---

## Summary  
This ADR establishes a unified Observation subsystem across:

- Frank.Abstractions.Observations  
- Frank.Infrastructure.Observations  

It defines:

- `IObservationContext`  
- `IRequestObservationContext`  
- `SystemObservationContext`  
- `ObservationContextBase`  
- `IObservationSink`  
- `ICorrelationContext`  
- `IMetrics`  

And integrates Observation into:

- inbound middleware  
- outbound handlers  
- hosting engine  
- startup engine  
- domain pipelines  
- background jobs  
- test harnesses  

This creates a consistent, deterministic, testable foundation for correlation, structured events, and metrics across the entire platform.

