# Observations (Application Layer)

The **Observations** folder defines the abstractions that make up the Frank
platform’s unified observability model.  
These interfaces describe how correlation IDs, structured contexts, metrics,
trace events, and error‑boundary signals flow through the application.

Implementations live in the **Infrastructure** layer, ensuring consistent,
deterministic, and environment‑aware observability across all vertical slices.

Observability here is **structured**, **context‑driven**, and **immutability‑oriented**:
contexts are immutable in structure but support controlled metadata enrichment
required for subsystem diagnostics.

---

## Purpose

The Observations subsystem exists to:

- correlate logs, traces, and metrics across boundaries  
- provide structured, deterministic context for all observable operations  
- emit trace events with consistent metadata  
- record metrics (counters, gauges, timers) enriched with request context  
- capture unhandled exceptions at error boundaries  
- support distributed tracing and cross‑service correlation  
- ensure infrastructure‑level consistency without polluting business logic  

This subsystem is foundational for reliability, debugging, performance analysis,
and operational visibility.

---

## Components

### IObservationContext

Represents the structured context flowing through all observable operations.

Includes:

- **CorrelationId** — distributed tracing identifier  
- **Channel** — vertical slice or capability  
- **Agent** — subsystem or module  
- **Environment** — hosting environment  
- **Timestamp** — creation time  
- **Metadata** — structured diagnostic enrichment  
- **AddMetadata** — controlled metadata enrichment for observability

Used by all sinks, metrics, and instrumentation.

---

### IRequestObservationContext

Extends `IObservationContext` with request‑level metadata such as:

- **UserId** (optional)

Used for:

- request pipelines  
- authentication flows  
- user‑scoped diagnostics  
- API‑level validation observability (US‑199)

---

### ICorrelationContext

Responsible for creating and propagating correlation identifiers.

Responsibilities:

- generate GUID‑based correlation IDs  
- propagate incoming IDs when present  
- ensure stable correlation across distributed operations

---

### IMetrics

Provides metric emission capabilities:

- **Counters**  
- **Gauges**  
- **Timers**

All metrics may be enriched with request context.

---

### IObservationSink

Emits structured trace events.

Infrastructure implementations integrate with telemetry backends such as:

- OpenTelemetry  
- Application Insights  
- Seq  
- Elastic  
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

- **Immutability-first**  
  Observation contexts never change once created, except for controlled metadata
  enrichment via `AddMetadata`.

- **Correlation-first**  
  Every observable signal carries a correlation ID.

- **Infrastructure-owned**  
  Application code consumes abstractions; infrastructure provides implementations.

- **Structured events**  
  All trace events and metrics include consistent metadata.

- **Separation of concerns**  
  Observability is isolated from business logic.

- **Deterministic behavior**  
  Observability signals are predictable and reproducible.

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

They ensure that every observable operation — from HTTP requests to domain
events — carries unified, structured metadata.

---
