# Observations

The **Observations** subsystem provides unified, structured, deterministic observability across the Frank platform.  
It defines how correlation IDs, request/system contexts, metrics, trace events, and error boundaries are captured, enriched, and emitted.

This folder contains the **infrastructure‑level implementations** that power:

- request tracing  
- structured logging  
- metrics  
- error reporting  
- background/system diagnostics  

All components here implement abstractions defined in  
**`Frank.Core.Application.Abstractions.Observations`**.

---

## Components

### ObservationContextBase

`ObservationContextBase` is the foundational type for all observability contexts.

#### Responsibilities

- Defines the unified metadata model used by trace events.
- Provides correlation ID, channel, agent, environment, timestamp, and metadata.
- Implements `IObservationContext`, including controlled metadata enrichment via `AddMetadata`.
- Serves as the base class for request‑scope and system‑scope contexts.

All other contexts extend this type.

---

### RequestObservationContext

Represents an authenticated request‑scope observation context.

#### Responsibilities

- Includes `UserId` when available.
- Automatically enriches metadata with `"user.id"`.
- Captures correlation ID, channel, agent, environment, timestamp.
- Used for all authenticated API requests.
- Created via a DI factory.

---

### FallbackRequestObservationContext

Fallback request‑scope context used when no user identity is available.

#### Responsibilities

- Used for unauthenticated requests, startup paths, tests, and background flows.
- Generates a new correlation ID.
- Provides `"none"` defaults for channel and agent.
- Ensures every request has a valid observation context.

This replaces the older `DefaultRequestObservationContext`.

---

### SystemObservationContext

Represents system‑scope observability for background tasks, scheduled jobs, startup/shutdown events, and infrastructure workflows.

#### Responsibilities

- No user identity.
- Generates correlation IDs automatically.
- Captures environment and timestamp.
- Used for non‑request operations.
- Created via a DI factory.

---

### CorrelationContext

Provides correlation ID generation and propagation.

#### Responsibilities

- Generates GUID‑based correlation IDs (`"N"` format).
- Propagates incoming IDs when present.
- Ensures consistent correlation across distributed operations.

Used by request and system contexts.

---

### ErrorBoundaryObserver

Observes boundary‑level errors and emits structured error events.

#### Responsibilities

- Converts exceptions into structured payloads.
- Emits `"system.error"` events via `IObservationSink`.
- Includes message, stack trace, source, and exception type.
- Used by exception boundaries and middleware.

---

### Metrics

Infrastructure implementation of `IMetrics`.

#### Responsibilities

- Defines counter (`Increment`), gauge (`Gauge`), and timer (`Timer`) primitives.
- Currently a no‑op placeholder.
- Establishes the contract surface for future metrics backends.

Timer returns a no‑op disposable until real timing is implemented.

---

### ObservationSink

Infrastructure implementation of `IObservationSink`.

#### Responsibilities

- Emits structured trace events into the observability backend.
- Currently a no‑op placeholder.
- Defines the vendor‑specific emission boundary (OpenTelemetry, Application Insights, Elastic, etc.).

All observability flows through this sink.

---

### ServiceCollectionExtensions

Registers all observability components into DI.

#### Responsibilities

- Registers sinks, metrics, correlation context, and error observers.
- Registers clock (required by context constructors).
- Provides factory for `SystemObservationContext`.
- Provides scoped `IRequestObservationContext` with fallback behavior.
- Ensures every request and system operation has a valid observation context.

This is the entry point for wiring up the Observations subsystem.

---

## Design Principles

- **Unified metadata model**  
  All trace events share a consistent structure.

- **Correlation-first**  
  Every request and system operation has a correlation ID.

- **Controlled enrichment**  
  Contexts are immutable except for structured metadata added via `AddMetadata`.

- **Separation of concerns**  
  Application defines abstractions; infrastructure provides implementations.

- **Structured diagnostics**  
  Observability data is emitted as structured objects, not strings.

- **Extensibility**  
  Metrics and sinks are pluggable; future backends can be added without changing slices.

---

## How This Folder Fits Into the Architecture

The Observations subsystem supports:

- API request tracing  
- background job diagnostics  
- structured error reporting  
- metrics emission  
- unified logging and telemetry  
- distributed correlation across services  

Vertical slices emit trace events; this folder provides the infrastructure that captures, enriches, and forwards them.

---

## Typical Usage

Creating a request context:

```csharp
var context = RequestObservationContext.Create(
    userId,
    correlationId,
    channel: "http",
    agent: "frontend",
    environment,
    clock
);
