# Infrastructure

The **Infrastructure** layer provides concrete, environment‑aware implementations
of abstractions defined in `Frank.Core.Application`.  
Its purpose is to supply the runtime behaviors that support the platform’s
cross‑cutting concerns: time, environment access, exception handling,
observability, and dependency injection wiring.

Infrastructure components are intentionally thin, deterministic, and free of
business logic. They exist to bridge the application abstractions with the
actual runtime environment.

---

## Folder Structure

```
Infrastructure
├── AssemblyMarker.cs
├── Clock
├── EnvironmentVariables
├── ExceptionHandlers
├── Observations
├── ServiceCollectionExtensions.cs
└── Frank.Core.Infrastructure.csproj
```

Each subfolder represents a self‑contained infrastructure subsystem.

---

## AssemblyMarker

`AssemblyMarker` is a zero‑logic type used to reference the
`Frank.Core.Infrastructure` assembly during reflection‑based discovery.

This avoids brittle assumptions about assembly names and provides a stable anchor
for orchestrators and registration pipelines.

---

## Clock

Provides infrastructure‑level time services.

### Components

- **SystemClock**  
  Concrete implementation of `IClock` using `DateTimeOffset.UtcNow`.

- **ServiceCollectionExtensions**  
  Registers the clock into DI.

### Purpose

- Ensures deterministic, testable time access.
- Centralizes all system time retrieval.

---

## EnvironmentVariables

Provides access to environment variables and hosting environment metadata.

### Components

- **SystemEnvironment**  
  Concrete implementation of `IEnvironmentVariables`.

- **ServiceCollectionExtensions**  
  Registers environment services into DI.

### Purpose

- Unified access to environment configuration.
- Avoids direct calls to `Environment.GetEnvironmentVariable`.

---

## ExceptionHandlers

Implements the exception‑handling subsystem.

### Components

- **ExceptionHandlerRegistry**  
  Orders and resolves `IExceptionHandler` implementations.

- **ExceptionOptions**  
  Controls diagnostic verbosity and logging behavior.

- **ServiceCollectionExtensions**  
  Discovers and registers exception handlers using the unified orchestrator.

### Purpose

- Deterministic exception resolution.
- Slice‑controlled handler discovery.
- Environment‑aware error output.

---

## Observations

Implements the unified observability subsystem.

### Components

- **ObservationContextBase**  
  Base metadata model for all trace events.

- **RequestObservationContext**  
  Authenticated request‑scope context.

- **DefaultRequestObservationContext**  
  Fallback context for unauthenticated or background requests.

- **SystemObservationContext**  
  Context for background jobs and system‑initiated operations.

- **CorrelationContext**  
  Generates and propagates correlation IDs.

- **ErrorBoundaryObserver**  
  Emits structured error events.

- **Metrics**  
  Counter, gauge, and timer primitives (currently no‑op).

- **ObservationSink**  
  Vendor‑specific trace emission boundary (currently no‑op).

- **ServiceCollectionExtensions**  
  Registers all observability components and context factories.

### Purpose

- Unified tracing, logging, and metric metadata.
- Consistent correlation across distributed operations.
- Structured diagnostics for both request and system flows.

---

## Root ServiceCollectionExtensions

The root `ServiceCollectionExtensions.cs` file provides top‑level DI wiring for
Infrastructure components that do not belong to a specific subsystem.

This ensures Infrastructure can be added to the application with a single,
cohesive registration call.

---

## Design Principles

- **Abstractions in Application, implementations in Infrastructure**  
  Infrastructure never defines business logic.

- **Explicit registration**  
  All components are registered through clear DI extension methods.

- **Reflection‑safe assembly markers**  
  No hard‑coded assembly names.

- **Environment‑aware behavior**  
  Infrastructure adapts to Development, Staging, Production.

- **Unified cross‑cutting concerns**  
  Time, environment, exceptions, and observability share consistent patterns.

---

## How Infrastructure Fits Into the Architecture

Infrastructure is the runtime backbone of the platform.  
It provides the concrete behaviors that slices and application services rely on:

- time and environment access  
- exception handling and error boundaries  
- request/system observability  
- correlation propagation  
- DI discovery and registration  

Slices remain pure and testable; Infrastructure supplies the real-world
implementations.

---

## Typical Usage

Registering Infrastructure:

```csharp
services.AddFrankCoreInfrastructure();
```

Using the clock:

```csharp
var now = _clock.UtcNow;
```

Creating a system observation context:

```csharp
var context = _systemContextFactory("system", "scheduler");
```

Resolving an exception handler:

```csharp
var handler = _registry.Resolve(exception);
```

---

## Notes

- Infrastructure is intentionally minimal and deterministic.
- All abstractions live in `Frank.Core.Application`.
- Observability and exception handling are designed for future backend integration.
- Assembly markers ensure safe reflection and discovery.

---
