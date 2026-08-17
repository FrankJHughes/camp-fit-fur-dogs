# Frank.Core.Infrastructure — Service Registration

The service‑registration subsystem defines how all infrastructure services are wired into the dependency‑injection container. It ensures that observations, exception handling, environment detection, clocking, and other cross‑cutting services are consistently available throughout the platform.

This document maps the service‑registration subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure
```

---

## Purpose

Service registration exists to:

- provide a single, predictable entry point for wiring infrastructure services  
- ensure consistent DI configuration across all Frank‑based products  
- keep Program.cs clean and declarative  
- centralize platform‑level middleware, logging, and runtime services  
- guarantee that infrastructure services are available to all vertical slices  

It is the bootstrap layer that activates the entire infrastructure subsystem.

---

## Responsibilities of the Subsystem

### Infrastructure Bootstrapping

The registration module:

- configures observation contexts  
- registers exception‑handling components  
- wires up clock and environment abstractions  
- configures logging and middleware  
- exposes a unified extension method for platform setup  

This ensures infrastructure is initialized consistently across all applications.

### Dependency Injection Configuration

Infrastructure services are registered with appropriate lifetimes:

- **Singletons** for global contexts (e.g., `SystemObservationContext`)  
- **Scoped** for per‑request services (e.g., `RequestObservationContext`)  
- **Transient** for lightweight helpers  

This keeps runtime behavior predictable and efficient.

### Middleware Registration

The registration process wires up:

- exception‑handling middleware  
- observation middleware  
- correlation ID propagation  
- logging enrichers  

These components form the backbone of request processing.

### Environment‑Aware Configuration

Service registration adapts based on environment:

- production logging vs. development logging  
- debug endpoints enabled only in non‑production  
- stricter exception handling in production  

Environment detection is provided by the `IEnvironment` service.

---

## How Service Registration Connects to the Broader Platform

Service registration collaborates with:

- **Frank.Core.Api**  
  Middleware and platform setup are activated here.

- **Frank.Core.Application**  
  Handlers rely on infrastructure services (clock, environment, observations).

- **Frank.Core.Domain**  
  Domain events and exceptions flow through infrastructure pipelines.

- **Frank.Core.EntityFrameworkCore**  
  Persistence services are registered alongside infrastructure components.

Service registration is the glue that binds all layers together.

---

## Registration Entry Point

All infrastructure services are registered via the platform bootstrap:

```csharp
services.AddFrankCoreApiPlatform(configuration);
```

This registers:

- Observations infrastructure  
- Exception handler registry  
- System clock (`IClock`)  
- Environment variable access (`IEnvironment`)  
- Logging configuration  
- Middleware components  
- Correlation and tracing services  

This single call ensures the entire infrastructure layer is active.

---

## Runtime Collaboration Points

Service registration interacts with the runtime by:

- configuring middleware pipelines  
- enabling structured logging and correlation  
- providing environment‑specific behavior  
- injecting clock and environment services into handlers  
- wiring exception handling into the request lifecycle  

It ensures the platform behaves consistently across all environments.

---

## Composition Flow (API → Application → Domain → Infrastructure)

```
Program.cs
    ↓
AddFrankCoreApiPlatform(configuration)
    ↓
Infrastructure Services Registered
    ↓
Middleware Pipeline Activated
    ↓
Application Handlers Receive Services
    ↓
Domain Logic Executes with Infrastructure Support
```

Service registration is the first step in the vertical slice lifecycle.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure registration implementation.  
Whenever new infrastructure modules are added or registration patterns evolve, update this section to reflect the current platform architecture.
