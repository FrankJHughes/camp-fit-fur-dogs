# Identity API — Observations Middleware

The **Observations** middleware provides unified, structured telemetry for all HTTP
requests flowing through the API surface.  
Although it resides in the Identity API assembly, it is **not limited to identity
endpoints**.  
It lives here because it depends on Identity‑specific abstractions such as
`ICurrentUser`, correlation propagation, and user‑resolution pipelines.

This middleware is part of the shared infrastructure layer and is intended to
instrument **any API surface hosted within this assembly**, not just identity
routes.

It implements the observability guarantees defined in **US‑183 — Improve
Observability**, ensuring that every request is:

- Correlated  
- Timed  
- Traced  
- Counted  
- Error‑observed  
- Enriched with contextual metadata (path, method, environment, user ID when available)

---

## Folder Structure

```
Observations/
├── ObservationInstrumentationMiddleware.cs
└── ApplicationBuilderExtensions.cs
```

---

## ObservationInstrumentationMiddleware

Provides full‑stack request instrumentation for the API pipeline.

### Responsibilities

- **Correlation**
  - Propagates incoming `X-Correlation-ID` or generates a new one  
  - Writes correlation ID to the response  

- **User Context**
  - Resolves `ICurrentUser`  
  - Includes user ID when authenticated  
  - Never exposes tokens, claims, or provider metadata  

- **Metrics**
  - Measures request duration  
  - Increments request counters  
  - Increments error counters  

- **Tracing**
  - Emits `http.request.begin`  
  - Emits `http.request.complete`  
  - Emits `http.request.error`  

- **Error Observation**
  - Reports exceptions to `IErrorBoundaryObserver`  
  - Emits structured error traces  

### Why It Lives in the Identity API

- It requires **Identity’s user‑resolution abstraction (`ICurrentUser`)**  
- It participates in **Identity’s correlation and observation pipeline**  
- It is **shared infrastructure**, but Identity is the only subsystem that provides the required dependencies  
- It is designed to instrument **all API traffic**, not only identity endpoints  

---

## ApplicationBuilderExtensions

Registers the observability middleware into the ASP.NET Core pipeline.

### Responsibilities

- Adds `ObservationInstrumentationMiddleware`  
- Ensures observability is applied uniformly across the entire API surface  
- Keeps registration isolated from endpoint logic  

### Contract

```csharp
app.UseFrankIdentityApiMiddlewareObservations();
```

### Notes

- Runs early to capture the full request lifecycle  
- Correlation ID is available to all downstream components  
- Works for *any* endpoint hosted in this assembly  

---

## How Observations Fit Into the API Architecture

```
[ Client Request ]
       ↓
[ Observations Middleware ]
       ↓
[ Authentication ]
       ↓
[ Authorization ]
       ↓
[ Endpoint Execution ]
       ↓
[ Application Pipelines ]
```

This ensures:

- Every request is traceable end‑to‑end  
- Every error is observable and structured  
- Every metric is consistent and environment‑aware  
- Observability is unified across all API surfaces hosted in this assembly  

---

## Summary

The Observations middleware provides:

- Correlation propagation  
- Structured tracing  
- Unified metrics  
- Error observation  
- Context‑rich telemetry  

It is **shared infrastructure**, hosted in the Identity API assembly only because
Identity provides the abstractions it depends on.

---
