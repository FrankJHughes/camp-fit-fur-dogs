# Frank.Core.Application — Overview

The `Frank.Core.Application` subsystem defines the orchestration primitives used by all products built on the Frank platform. It provides the command/query dispatching model, pipeline behaviors, result types, and application‑level contracts that sit between the API layer and the domain layer.

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

- coordinate execution of commands and queries  
- enforce cross‑cutting behaviors (validation, logging, authorization)  
- provide a consistent dispatching model across all products  
- separate orchestration from domain logic  
- ensure handlers remain pure and deterministic  

This subsystem keeps the architecture readable, testable, and predictable for future contributors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Application`

- **Documentation folder:**  
  `docs/02-frank-core/application`

This documentation must remain aligned with the actual dispatcher, pipeline, and result implementations.

---

## What Belongs Here

### [Application Responsibilities](ca://s?q=Frank_Core_Application_Responsibilities)
- command and query dispatching  
- pipeline behaviors (validation, logging, authorization)  
- result modeling (`Result<T>`, error types, success/failure flows)  
- handler contracts (`ICommandHandler`, `IQueryHandler`)  
- immutable request context propagation  

### [Platform Integration](ca://s?q=Frank_Core_Application_Platform_Integration)
How the application layer connects to:

- **Frank.Core.Api** — endpoints call dispatchers  
- **Frank.Identity** — identity flows into handlers via context  
- **Frank.Core.Domain** — handlers invoke domain logic  
- **Frank.Core.EntityFrameworkCore** — handlers commit via Unit of Work  

### [Runtime Collaboration Points](ca://s?q=Frank_Core_Application_Runtime_Collaboration)
- pipeline execution order  
- validation before domain invocation  
- logging and correlation via observation context  
- exception mapping into `Result<T>`  
- transactional boundaries enforced by infrastructure  

### [Composition Flow](ca://s?q=Frank_Core_Application_Composition_Flow)
How application logic fits into the vertical slice:

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
Whenever dispatching, pipeline behaviors, or result modeling evolve, update this section to reflect the current platform architecture.

