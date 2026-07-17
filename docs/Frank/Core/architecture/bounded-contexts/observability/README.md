# Frank — Architecture — Bounded Context — Observability  
*The cross‑cutting capability that makes the entire system understandable.*

Observability in the Frank platform is a **cross‑cutting capability** that ensures every request, operation, and cross‑service call can be traced, correlated, measured, and diagnosed consistently.  
It provides the plumbing that makes the entire system **understandable**, **debuggable**, and **operationally safe**.

This document explains **how Observability works end‑to‑end**.

---

# 1. The Core Idea

Every request in the system has:

- **one correlation ID**  
- **one trace context**  
- **one observability context**  

These values flow:

- **inbound** (client → API)  
- **internal** (API → domain → infrastructure)  
- **outbound** (API → downstream services)  

Everything else — metrics, trace events, error observation — is built on top of this flow.

Observability is **unified**, **deterministic**, and **request‑scoped**.

---

# 2. Inbound: How Observability Begins

When a request enters the system, the **Inbound Trace Context Middleware** runs first.

It performs four steps:

---

## Step 1 — Extract

It reads:

- `traceparent` (W3C)  
- `tracestate`  
- `X-Correlation-ID`  
- `X-Channel`  
- `X-Agent`  

---

## Step 2 — Normalize

It chooses the authoritative correlation ID:

````text
traceparent.trace-id
    > X-Correlation-ID
    > generated GUID
````

This ensures **stable**, **predictable**, **non‑colliding** correlation.

---

## Step 3 — Populate

It creates a request‑scoped `IObservabilityContext` containing:

- CorrelationId  
- TraceId  
- SpanId  
- Channel  
- Agent  
- Environment  
- Timestamp  
- Metadata  

This context becomes the **single source of truth** for the entire request.

---

## Step 4 — Surface

It writes:

````text
X-Correlation-ID: <value>
````

onto the **response headers**, ensuring the caller sees the same ID the system used internally.

This is the foundation of observability.

---

# 3. Internal: How Observability Flows Through the System

Once the `IObservabilityContext` is established, it flows through the entire system.

Every subsystem reads from it:

- logging  
- metrics  
- trace events  
- error boundaries  
- handlers  
- endpoints  
- repositories  
- domain logic  

No component:

- re‑parses headers  
- generates its own correlation ID  
- stores observability state manually  

This ensures **consistency**, **purity**, and **determinism**.

---

# 4. Outbound: How Observability Leaves the System

When the system makes an outbound HTTP call, the **Outbound Trace Context Handler** runs.

It performs three steps:

---

## Step 1 — Read

It reads the current `IObservabilityContext`.

---

## Step 2 — Attach

It writes:

- `traceparent`  
- `tracestate`  
- `X-Correlation-ID`  
- `X-Channel`  
- `X-Agent`  

onto the outgoing request.

---

## Step 3 — Preserve

If the caller already provided a `traceparent`, it is **not overwritten**.

This ensures **distributed trace continuity** across services.

---

# 5. Metrics: How the System Measures Itself

Metrics are emitted through the `IMetrics` abstraction.

Every metric is automatically enriched with:

- correlation ID  
- environment  
- service name  
- timestamp  

This ensures metrics can be correlated with traces and logs.

Metrics are:

- structured  
- deterministic  
- correlation‑aware  

---

# 6. Trace Events: How the System Explains Itself

Trace events are emitted through `ITraceEvents`.

Each event includes:

- event name  
- category  
- message  
- optional structured data  
- correlation ID  
- trace ID  
- timestamp  

Trace events are the **breadcrumbs** that explain what happened inside the system.

Events are:

- structured  
- typed  
- correlation‑aware  
- slice‑aligned  

---

# 7. Error Boundaries: How the System Observes Failures

When an exception crosses a boundary (middleware, endpoint, handler), the system uses `IErrorBoundaryObserver` to:

- capture the error  
- enrich it with correlation context  
- emit a trace event  
- emit metrics  
- log structured error data  

This ensures failures are **observable**, not silent.

Error boundaries provide:

- consistent error semantics  
- safe logging  
- deterministic classification  

---

# 8. Test Harness: How Observability Is Verified

The Frank test harness includes:

- test endpoints (e.g., `/__test__/correlation`)  
- fake metrics  
- fake trace events  
- fake error observers  

These allow tests to validate:

- correlation propagation  
- traceparent extraction  
- outbound propagation  
- error observation  
- metric emission  
- trace event emission  

Tests run against the **real pipeline**, not mocks.

This ensures **deterministic**, **end‑to‑end** observability validation.

---

# 9. Putting It All Together (The Full Flow)

Here is the complete lifecycle of observability in one sequence:

````text
1. Request arrives
2. Inbound middleware extracts trace context
3. Correlation ID is normalized
4. IObservabilityContext is created
5. Endpoint executes
6. Domain logic executes
7. Metrics and trace events are emitted
8. Outbound HTTP calls propagate trace context
9. Errors are observed and correlated
10. Response is returned with X-Correlation-ID
````

This is the entire observability pipeline.

---

# 10. Why This Matters

This architecture ensures:

- every request is traceable  
- every operation is correlated  
- every error is diagnosable  
- every metric is contextual  
- every service participates in distributed tracing  
- every test can validate observability end‑to‑end  

It makes the system:

- **understandable**  
- **debuggable**  
- **operationally safe**  
- **consistent across environments**  
- **aligned with modern distributed tracing standards**  
