# Observations

The **Observations** folder contains the abstractions that define the platform’s unified observability model. These interfaces describe how correlation IDs, structured context, metrics, trace events, and error‑boundary signals flow through the application. Implementations are provided by the infrastructure layer, ensuring consistent, deterministic, and environment‑aware observability across all vertical slices.

Observability here is **structured**, **immutable**, and **context‑driven**, enabling deep diagnostics without leaking infrastructure concerns into application logic.

---

## Purpose

The Observations subsystem exists to:

- correlate logs, traces, and metrics across boundaries  
- provide immutable, structured context for all observable operations  
- emit trace events with consistent metadata  
- record metrics (counters, gauges, timers) enriched with request context  
- capture unhandled exceptions at error boundaries  
- support distributed tracing and cross‑service correlation  
- ensure infrastructure‑level consistency without polluting business logic  

This subsystem is foundational for reliability, debugging, performance analysis, and operational visibility.

---

## Components

### IObservationContext
Represents the immutable, structured context flowing through all observable operations.

Includes:

- CorrelationId  
- Channel (vertical slice)  
- Agent (module/subsystem)  
- Environment  
- Timestamp  
- Metadata  

Used by all observability sinks and instrumentation.

---

### IRequestObservationContext
Extends `IObservationContext` with request‑level metadata such as:

- UserId (optional)

Used for request pipelines, authentication flows, and user‑scoped diagnostics.

---

### ICorrelationContext
Responsible for creating and propagating correlation identifiers.

Ensures every operation has a stable correlation ID.

---

### IMetrics
Provides metric emission capabilities:

- Counters  
- Gauges  
- Timers  

All metrics may be enriched with request context.

---

### IObservationSink
Emits structured trace events.

Infrastructure implementations integrate with telemetry backends such as:

- OpenTelemetry  
- Application Insights  
- Seq  
- Custom sinks  

---

### IErrorBoundaryObserver
Observes unhandled exceptions at error boundaries.

Used for:

- structured error events  
- diagnostics  
- correlation  
- failure attribution  

Does not suppress or handle exceptions — only observes them.

---

## Design Principles

- **Immutability** — Observation contexts never change once created.  
- **Correlation-first** — Every observable signal carries a correlation ID.  
- **Infrastructure-owned** — Application code consumes observability abstractions but never implements them.  
- **Structured events** — All trace events and metrics include consistent metadata.  
- **Separation of concerns** — Observability is isolated from business logic.  
- **Deterministic behavior** — Observability signals are predictable and reproducible.

---

## How Observations Fit Into the Application

Observations are used throughout:

- middleware pipelines  
- command/query dispatch  
- domain orchestration  
- error boundaries  
- metrics instrumentation  
- trace event emission  
- logging and diagnostics  

They ensure that every observable operation — from HTTP requests to domain events — carries unified, structured metadata.

---
