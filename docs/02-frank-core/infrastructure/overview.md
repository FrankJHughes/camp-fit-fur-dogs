# Frank.Core.Infrastructure — Infrastructure Layer

The **Frank.Core infrastructure layer** provides foundational runtime services that support every vertical slice in the platform. These services are cross‑cutting, environment‑aware, and designed to keep domain and application layers pure, testable, and free from technical concerns.

This document maps the infrastructure layer under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure
```

---

## Key Modules

### Observations

Provides unified request tracing, correlation, and structured logging.

- **SystemObservationContext** — global observation context shared across the runtime  
- **RequestObservationContext** — per‑request correlation and diagnostic metadata  
- **CorrelationContext** — trace IDs, causation chains, and operation identifiers  
- **ErrorBoundaryObserver** — captures and logs unhandled exceptions with correlation metadata  

Observations ensure every request, command, query, domain event, and persistence operation is fully traceable.

---

### Exception Handlers

Manages exception‑to‑HTTP response mapping and structured error output.

- **ExceptionHandlerRegistry** — central registry of exception handlers  
- **ExceptionOptions** — configuration for exception‑handling behavior  
- **Domain Exception Mapping** — transforms domain exceptions into user‑friendly HTTP responses  

Exception handling ensures consistent, predictable error behavior across all layers.

---

### Clock

Provides time abstractions for deterministic, testable time‑dependent logic.

- **SystemClock** — production implementation of `IClock` using `DateTime.UtcNow`  
- **TestClock** (in test harness) — allows manual time advancement for expiration logic, scheduling, and time‑based domain rules  

Clock abstraction eliminates reliance on system time and makes tests fully deterministic.

---

### Environment Variables

Provides type‑safe access to environment configuration.

- **SystemEnvironment** — implementation of `IEnvironment`  
- encapsulates OS environment variable access  
- supports environment‑specific behavior (Development, Testing, Production, Staging, RenderPrPreview)  
- easy to stub or override in tests  

Environment abstraction ensures runtime behavior is consistent and environment‑aware.

---

## Registration

All infrastructure services are registered via the platform bootstrap:

```csharp
services.AddFrankCoreApiPlatform(configuration);
```

This registers:

- Observations infrastructure  
- Exception handler registry  
- System clock  
- Environment variable access  
- Logging and middleware components  

The infrastructure layer becomes available to all vertical slices through dependency injection.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure implementation.  
Whenever logging, exception handling, environment detection, or clocking evolves, update this section to reflect the current platform architecture.
