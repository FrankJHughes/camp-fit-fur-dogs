# Frank.Core.Application — Overview

The `Frank.Core.Application` subsystem defines the orchestration layer that sits between the API surface and the domain model. It provides the dispatching model, pipeline behaviors, immutable contexts, domain event propagation, and result primitives that all Frank‑based products rely on.

This document describes the responsibilities of the application subsystem and maps the documentation folder:

```
docs/02-frank-core/application
```

back to the implementation under:

```
src/Frank/Core
```

---

## Purpose

The application layer exists to:

- coordinate command and query execution  
- enforce cross‑cutting behaviors (validation, logging, authorization)  
- propagate immutable request context through the vertical slice  
- dispatch domain events raised by aggregates  
- unify error handling through `Result<T>`  
- keep domain logic pure and free of orchestration concerns  

This subsystem ensures that the architecture remains readable, predictable, and maintainable for future contributors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Application`

- **Documentation folder:**  
  `docs/02-frank-core/application`

This documentation must remain aligned with the actual dispatcher, pipeline, context, and event implementations.

---

## What Belongs Here

### [Subsystem Responsibilities](ca://s?q=Frank_Core_Application_Responsibilities)
This section should describe:

- command and query dispatching  
- pipeline behaviors and execution order  
- immutable request/observation contexts  
- domain event propagation  
- result modeling (`Result<T>`, error types)  
- handler contracts (`ICommandHandler`, `IQueryHandler`)  

### [Platform Integration](ca://s?q=Frank_Core_Application_Platform_Integration)
How the application layer connects to:

- **Frank.Core.Api** — endpoints call dispatchers  
- **Frank.Identity** — identity flows into handlers via context  
- **Frank.Core.Domain** — aggregates raise domain events  
- **Frank.Core.EntityFrameworkCore** — handlers commit via Unit of Work  

### [Runtime Collaboration Points](ca://s?q=Frank_Core_Application_Runtime_Collaboration)
- pipeline execution  
- validation before domain invocation  
- logging and correlation via observation context  
- exception mapping into `Result<T>`  
- transactional boundaries enforced by infrastructure  

### [Composition Flow](ca://s?q=Frank_Core_Application_Composition_Flow)
How the application layer fits into the vertical slice:

```
API Endpoint
    ↓
ICommandDispatcher / IQueryDispatcher
    ↓
Pipeline Behaviors
    ↓
Application Handler
    ↓
Domain Aggregate
    ↓
Infrastructure Persistence
```

The application layer is the orchestration engine for the entire slice.

---

## Notes

Keep this document grounded in the actual Frank.Core.Application implementation.  
Whenever dispatching, pipeline behaviors, contexts, or domain event flows evolve, update this section to reflect the current platform architecture.

