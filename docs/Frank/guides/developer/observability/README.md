# Frank Observability Developer Guide

Frank Observability is a platform-level capability providing unified correlation, structured events, and vendor-agnostic metrics across all Frank engines and all applications built on Frank. This guide explains how developers integrate with and use the observability subsystem.

---

## 1. Purpose of Frank Observability

Frank Observability ensures:

- End-to-end correlation across hosting, startup, modules, domain, and infrastructure
- Structured, consistent event emission
- Vendor-agnostic metrics with deterministic timing
- Deterministic behavior for testing
- A unified context model (`IObservabilityContext`) that flows through the entire platform

Developers consume these capabilities; they do not extend or redefine them.

---

## 2. Core Abstractions

### `IObservabilityContext`
Immutable execution context containing:
- Correlation ID  
- Slice  
- Module  
- Environment  
- Timestamp  
- Metadata  

Developers must **never** mutate or recreate this context.

### `ICorrelationContext`
Responsible for:
- Creating correlation IDs  
- Propagating IDs across async boundaries  
- Reading/writing correlation headers  

Developers must **never** generate correlation IDs manually.

### `ITraceEvents`
Structured event emitter with:
- Event name  
- Category  
- Severity  
- Payload  
- Automatic correlation injection  

### `IMetrics`
Vendor-agnostic metrics primitives:
- Counters  
- Gauges  
- Timers  
- Duration measurement  

---

## 3. Integration Points for Developers

### 3.1 API Handlers
Developers must:
- Accept the `IObservabilityContext` from the hosting pipeline
- Emit events for request start/end, validation, domain calls, and failures
- Emit metrics for handler duration

### 3.2 Domain Services
Developers must:
- Propagate the observability context
- Emit events for domain actions
- Emit metrics for domain operation duration

### 3.3 Infrastructure Adapters
Developers must:
- Propagate correlation IDs to external systems
- Emit events for external calls, retries, and failures
- Emit metrics for latency and success/failure

### 3.4 Outbox Handlers
Developers must:
- Emit events for outbox processing
- Emit metrics for handler duration and retry counts
- Propagate correlation IDs from domain → outbox → handler

---

## 4. Event Naming Rules

Event names must follow:

````  
slice.module.action  
````

Examples:
- `api.owner.login_failed`
- `infra.email.sent`
- `domain.customer.created`

---

## 5. Metric Naming Rules

Metric names must follow:

````  
slice.module.metric_name  
````

Examples:
- `api.login.duration_ms`
- `infra.outbox.retry_count`

---

## 6. Forbidden Developer Practices

- Manual correlation ID creation  
- Stopwatch timing  
- Ad-hoc logging  
- Vendor-specific logging/metrics APIs  
- Mutable observability context  

---

## 7. Developer Responsibilities Summary

Developers must:
- Use Frank abstractions exclusively  
- Follow naming conventions  
- Propagate context correctly  
- Emit structured events and metrics  
- Avoid forbidden patterns  

Frank Observability provides the platform; developers integrate with it.
