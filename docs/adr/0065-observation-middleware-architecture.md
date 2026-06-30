# ADR‑0065 — Observation Middleware Architecture

## Status  
Accepted  
Depends on: ADR‑0060 (Observation Abstractions), ADR‑0064 (Identity Resolution v2)

## Context  
After introducing the Observation abstractions (ADR‑0060) and Identity Resolution v2 (ADR‑0064), the platform still lacked a **canonical way** to:

- attach Observation context to inbound HTTP requests,
- propagate correlation and identity into the Observation model,
- emit structured events and metrics around the HTTP pipeline,
- surface errors to the Observation subsystem,
- ensure all of this was **consistent**, **testable**, and **layer‑pure**.

Previously, observation‑like behavior was:

- scattered across middleware and endpoints,
- mixed with logging and error handling,
- inconsistent in how correlation and identity were handled,
- difficult to assert in tests.

A dedicated **Observation pipeline middleware architecture** was required.

---

## Decision  

We define a **two‑stage Observation middleware pipeline** in `Frank.Infrastructure.Observations`:

1. **InboundObservationContextMiddleware** — builds and stores request‑scope Observation context.
2. **ObservationInstrumentationMiddleware** — emits events and metrics around the HTTP pipeline and surfaces errors to the Observation subsystem.

### 1. InboundObservationContextMiddleware

Responsibilities:

- Extract correlation ID from:
  - `traceparent` (W3C TraceContext)  
  - `X-Correlation-ID`  
  - `HttpContext.TraceIdentifier` (fallback)
- Propagate correlation via `ICorrelationContext`.
- Resolve user identity via `ICurrentUser`.
- Build a `RequestObservationContext` with:
  - `UserId`  
  - `CorrelationId`  
  - `Channel = "http"`  
  - `Agent = "pipeline"`  
  - `Environment` (via `IHostEnvironment`)  
  - metadata (`path`, `method`, etc.)
- Store the context in `HttpContext.Items[nameof(IRequestObservationContext)]`.

This middleware **does not** emit events or metrics; it only prepares context.

### 2. ObservationInstrumentationMiddleware

Responsibilities:

- Read or derive correlation ID and set `X-Correlation-ID` on the response.
- Build a `RequestObservationContext` (or reuse the inbound one).
- Use `IMetrics` to:
  - time the request (`http.request.duration`),
  - increment counters (`http.request.count`, `http.request.errors`).
- Use `IObservationSink` to emit structured events:
  - `http.request.begin`  
  - `http.request.complete`  
  - `http.request.error`
- Use `IErrorBoundaryObserver` to surface exceptions to the Observation subsystem.

This middleware wraps the entire pipeline:

- emits a **begin** event before calling `_next`,
- emits a **complete** event on success,
- emits an **error** event and notifies `IErrorBoundaryObserver` on failure.

### 3. Ordering and Integration

The Observation middleware is ordered:

1. `InboundObservationContextMiddleware`
2. `ObservationInstrumentationMiddleware`
3. Remaining pipeline (auth, routing, endpoints, etc.)

StartupEngine (ADR‑0062) wires these in the API startup module.

### 4. Testability

Tests can:

- construct `DefaultHttpContext`,
- inject `ICorrelationContext`, `ICurrentUser`, `IMetrics`, `IObservationSink`, `IErrorBoundaryObserver`,
- assert:
  - correlation extraction and propagation,
  - `RequestObservationContext` contents,
  - emitted events,
  - emitted metrics,
  - error observation behavior.

No Testcontainers are required.  
No fake endpoints are required.

---

## Consequences  

### Positive  
- Canonical, consistent Observation pipeline for HTTP requests.  
- Deterministic correlation and identity propagation into Observation.  
- Structured events and metrics around the HTTP pipeline.  
- Clean integration with Identity Resolution v2 and Observation abstractions.  
- Strong testability using `DefaultHttpContext` and DI.  
- Clear separation between context creation and instrumentation.

### Negative  
- Middleware ordering must be maintained carefully.  
- Contributors must understand the two‑stage Observation pipeline.  

### Neutral  
- Does not dictate any specific metrics or tracing backend.  
- Complements logging; does not replace it.

---

## Summary  
This ADR defines the **Observation pipeline middleware architecture**:

- `InboundObservationContextMiddleware` builds request‑scope Observation context.
- `ObservationInstrumentationMiddleware` emits events and metrics and surfaces errors.

Together, they provide a consistent, testable, layer‑pure Observation pipeline for HTTP requests, fully aligned with ADR‑0060 and ADR‑0064.

