# Observations Middleware

The **Observations Middleware** subsystem provides unified inbound and outbound
observability for the Frank.Core API.  
It ensures that every incoming request receives a fully populated
`IRequestObservationContext`, and that every outbound HTTP request propagates
correlation identifiers, metadata, and W3C TraceContext headers.

This subsystem forms the backbone of distributed tracing, correlation, and
diagnostic visibility across vertical slices and external service boundaries.

---

## Files

```
Observations/
├── InboundObservationContextMiddleware.cs
├── OutboundObservationContextHandler.cs
├── ServiceCollectionExtensions.cs
└── ApplicationBuilderExtensions.cs
```

---

## InboundObservationContextMiddleware

`InboundObservationContextMiddleware` constructs an `IRequestObservationContext`
for each incoming HTTP request and attaches it to `HttpContext.Items`.

### Responsibilities

- Extracts correlation identifiers from:
  - `traceparent` (W3C TraceContext)
  - `X-Correlation-ID`
  - ASP.NET Core `TraceIdentifier`
- Propagates correlation IDs via `ICorrelationContext`.
- Resolves the current user (if authenticated).
- Captures environment and clock metadata.
- Records request metadata (path, method).
- Makes the observation context available to all downstream components.

### Why this matters

Inbound observation context ensures:

- consistent correlation across slices  
- unified metadata for logging and diagnostics  
- seamless integration with outbound tracing  
- predictable observability behavior across environments  

---

## OutboundObservationContextHandler

`OutboundObservationContextHandler` enriches outbound HTTP requests with
correlation and tracing headers derived from the inbound observation context.

### Responsibilities

- Adds correlation headers:
  - `X-Correlation-ID`
  - `X-Channel`
  - `X-Agent`
- Generates W3C `traceparent` headers when missing.
- Normalizes correlation IDs into valid 32‑character trace IDs.
- Generates random span IDs for distributed tracing.
- Ensures outbound calls participate in the same trace as inbound requests.

### Why this matters

Outbound propagation ensures:

- cross-service trace continuity  
- consistent correlation across HTTP boundaries  
- compatibility with OpenTelemetry, Application Insights, Jaeger, Zipkin, etc.  
- predictable behavior in distributed systems  

---

## ServiceCollectionExtensions

`ServiceCollectionExtensions` registers all observation-related services.

### Responsibilities

- Registers a wildcard `HttpClient("*")` that automatically applies
  `OutboundObservationContextHandler`.
- Registers `OutboundObservationContextHandler` as transient so it can resolve
  the current `IRequestObservationContext`.
- Adds `IHttpContextAccessor` to support inbound context creation.

### Usage

```csharp
services.AddFrankCoreApiObservations();
```

This ensures both inbound and outbound observability are active.

---

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` registers the inbound observation middleware in
the ASP.NET Core pipeline.

### Responsibilities

- Adds `InboundObservationContextMiddleware` to the pipeline.
- Ensures every request receives an observation context before reaching slices.

### Usage

```csharp
app.UseFrankCoreApiMiddlewareObservations();
```

This should be placed **early in the pipeline**, typically before routing.

---

## How Observations Fit Into the Architecture

Observations middleware is part of the API’s cross-cutting diagnostics layer.  
It provides:

- unified correlation  
- distributed tracing  
- consistent metadata propagation  
- slice-friendly observability  
- compatibility with external tracing systems  

Inbound and outbound components work together:

1. **Inbound middleware** creates the observation context.  
2. **Slices and services** consume the context.  
3. **Outbound handler** propagates correlation and trace headers.  

This ensures full trace continuity across the entire request lifecycle.

---

## Typical Flow

1. **Request enters API**  
2. Inbound middleware extracts correlation → builds observation context  
3. Context stored in `HttpContext.Items`  
4. Slices and services use `IRequestObservationContext`  
5. Outbound HTTP calls include correlation + traceparent  
6. External services participate in same distributed trace  
7. Response returns with full observability metadata

---

## Design Principles

- **Unified context**  
  One observation context per request.

- **Deterministic propagation**  
  Correlation and trace IDs flow inbound → outbound.

- **Distributed tracing ready**  
  W3C TraceContext compliant.

- **Slice-friendly**  
  No slice needs to manage correlation manually.

- **Minimal overhead**  
  Lightweight middleware and handlers.

---

## Notes

- Requires `IRequestObservationContext` to be registered in DI.
- Works seamlessly with any logging or telemetry provider.
- Safe for all environments, including production.
- Outbound propagation applies to all `HttpClient("*")` calls.

---
