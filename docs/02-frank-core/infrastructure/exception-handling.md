# Frank.Core.Infrastructure — Exception Handling

The exception‑handling subsystem in `Frank.Core.Infrastructure` provides the cross‑cutting mechanisms that convert raw exceptions into structured, observable, and predictable application behavior. It ensures that domain exceptions, application failures, and unexpected infrastructure errors are surfaced consistently across the platform.

This document maps the exception‑handling subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Exceptions
```

---

## Purpose

Exception handling exists to:

- provide a unified strategy for catching and processing exceptions  
- convert domain and application errors into structured results  
- prevent raw exceptions from leaking into API responses  
- ensure consistent logging, correlation, and observability  
- keep exception‑handling logic out of domain and application layers  

It is the safety net that protects the platform from unpredictable runtime failures.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Infrastructure/Exceptions`

- **Documentation folder:**  
  `docs/02-frank-core/infrastructure`

This documentation must remain aligned with the actual exception middleware, pipeline behaviors, and logging integrations.

---

## Responsibilities of the Exception‑Handling Subsystem

### [Domain Exception Mapping](ca://s?q=Frank_Core_Infrastructure_Domain_Exception_Mapping)
Domain exceptions (`DomainException`) are:

- caught by infrastructure pipeline behaviors  
- converted into `Result.Failure` objects  
- logged with domain‑specific metadata  
- surfaced to API as structured problem details  

This ensures domain invariants are communicated clearly without exposing internal logic.

### [Application Exception Handling](ca://s?q=Frank_Core_Infrastructure_Application_Exception_Handling)
Application‑level exceptions (e.g., handler failures) are:

- wrapped in consistent error responses  
- enriched with correlation IDs  
- logged with contextual information  
- prevented from leaking stack traces in production  

This keeps application behavior predictable and safe.

### [Infrastructure Exception Processing](ca://s?q=Frank_Core_Infrastructure_Infrastructure_Exception_Processing)
Infrastructure exceptions (e.g., database connectivity, configuration errors) are:

- captured by global exception middleware  
- mapped to appropriate HTTP status codes  
- logged with severity based on environment  
- optionally surfaced with diagnostic details in development  

This ensures operational failures are observable and actionable.

### [Global Exception Middleware](ca://s?q=Frank_Core_Infrastructure_Global_Exception_Middleware)
The middleware:

- wraps the entire request pipeline  
- catches unhandled exceptions  
- logs them with correlation and environment metadata  
- returns structured error responses  

It is the final guardrail for runtime safety.

### [Pipeline Behaviors](ca://s?q=Frank_Core_Infrastructure_Pipeline_Behaviors)
Command/query pipeline behaviors:

- intercept domain exceptions  
- convert them into `Result<T>` failures  
- prevent exceptions from bubbling into API  
- ensure handlers remain exception‑free  

This keeps vertical slices clean and predictable.

---

## How Exception Handling Connects to the Broader Platform

Exception handling collaborates with:

- **Frank.Core.Domain**  
  Domain exceptions are mapped and logged.

- **Frank.Core.Application**  
  Handlers return structured results instead of throwing.

- **Frank.Core.Api**  
  Middleware converts failures into HTTP responses.

- **Frank.Core.Infrastructure**  
  Logging and observability capture exception metadata.

- **Frank.Core.EntityFrameworkCore**  
  Persistence errors are surfaced through infrastructure handlers.

Exception handling is a cross‑cutting concern that touches every layer.

---

## Runtime Collaboration Points

Exception handling interacts with the runtime by:

- capturing unhandled exceptions  
- mapping domain failures to structured results  
- enriching logs with correlation IDs  
- adjusting verbosity based on environment  
- preventing stack traces from leaking in production  
- surfacing actionable diagnostics in development  

It ensures the platform behaves consistently under failure conditions.

---

## Composition Flow (API → Application → Domain → Infrastructure)

```
API Request
    ↓
Application Handler
    ↓
Domain Logic (may throw DomainException)
    ↓
Pipeline Behavior (maps exception → Result.Failure)
    ↓
API Middleware (maps failure → HTTP response)
    ↓
Client
```

Infrastructure ensures that exceptions never escape the structured flow.

---

## What Belongs in This Document

This page should describe:

- exception‑handling responsibilities  
- how domain exceptions are mapped  
- how application and infrastructure errors are processed  
- how middleware and pipeline behaviors collaborate  
- how exception handling fits into the vertical slice lifecycle  

It should **not** include:

- product‑specific error codes  
- API‑specific problem‑details formats  
- logging configuration details  

Those belong in product or API documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure exception‑handling implementation.  
Whenever pipeline behaviors, middleware, or logging integrations evolve, update this section to reflect the current platform architecture.
