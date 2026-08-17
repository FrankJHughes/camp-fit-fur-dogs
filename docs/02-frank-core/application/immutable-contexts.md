# Frank.Core.Application — Immutable Contexts

The immutable context subsystem in `Frank.Core.Application` provides the platform‑level mechanism for carrying request‑scoped information through the application pipeline in a safe, deterministic, and side‑effect‑free manner. Immutable contexts ensure that application handlers, pipeline behaviors, and domain logic all receive consistent contextual data without allowing mutation or leakage across requests.

This document describes the responsibilities of the immutable context subsystem and maps the documentation folder:

```
docs/02-frank-core/application
```

back to the implementation under:

```
src/Frank/Core
```

---

## Purpose

Immutable contexts exist to:

- provide a stable, read‑only container for request metadata  
- ensure contextual information cannot be mutated by handlers  
- propagate correlation IDs, timestamps, user identity, and environment data  
- unify context handling across API, application, and domain layers  
- prevent accidental cross‑request contamination in multi‑threaded environments  

They form the backbone of Frank’s deterministic request‑processing model.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/Application/Contexts`

- **Documentation folder:**  
  `docs/02-frank-core/application`

This documentation must remain aligned with the actual immutable context types and propagation logic.

---

## Responsibilities of the Immutable Context Subsystem

### [Context Definition](ca://s?q=Frank_Core_Application_ImmutableContext_Definition)
Frank defines immutable context interfaces such as:

- `IRequestContext`  
- `IObservationContext`  
- `IExecutionContext`  

These contexts expose read‑only properties for:

- correlation ID  
- request start time  
- authenticated user ID  
- environment metadata  
- pipeline state  

### [Context Creation](ca://s?q=Frank_Core_Api_Observation_Middleware)
Contexts are created at the API layer, typically inside inbound observation middleware:

```csharp
var context = new RequestContext(
    correlationId,
    DateTimeOffset.UtcNow,
    currentUser);
```

Once created, they are immutable for the lifetime of the request.

### [Context Propagation](ca://s?q=Frank_Core_Application_Pipeline_Behaviors)
Contexts flow through:

- pipeline behaviors  
- command/query dispatchers  
- application handlers  
- domain event dispatchers  

Propagation is explicit and controlled, ensuring deterministic behavior.

### [Context Consumption](ca://s?q=Frank_Core_Application_Handler_Context)
Handlers consume context to:

- access correlation IDs for logging  
- retrieve user identity  
- measure execution duration  
- enforce authorization rules  
- attach metadata to domain events  

Handlers **cannot** modify context values.

---

## How Immutable Contexts Connect to the Broader Platform

Immutable contexts collaborate with multiple Frank subsystems:

- **Frank.Core.Api**  
  Creates and initializes context during inbound middleware.

- **Frank.Identity**  
  Supplies authenticated user information.

- **Frank.Core.Application**  
  Passes context through dispatchers and pipeline behaviors.

- **Frank.Core.Infrastructure**  
  Uses context for logging, tracing, and correlation.

- **Frank.Core.Domain**  
  Domain events may include contextual metadata for observability.

This ensures consistent context handling across the entire vertical slice.

---

## Runtime Collaboration Points

Immutable contexts interact with the runtime in several ways:

- **During inbound middleware** — context is created and attached  
- **During dispatching** — context flows through pipeline behaviors  
- **During handler execution** — context provides identity and correlation  
- **During domain event dispatch** — context metadata is preserved  
- **During logging** — context enriches structured logs  

This keeps request processing deterministic, observable, and safe.

---

## Composition Flow (API → Application → Domain → Persistence)

Immutable contexts participate in the vertical slice flow:

```
API Middleware (context created)
    ↓
ICommandDispatcher / IQueryDispatcher
    ↓
Pipeline Behaviors (context propagated)
    ↓
Application Handler (context consumed)
    ↓
Domain Aggregate (context metadata optional)
    ↓
Persistence Layer (context used for logging)
```

Context flows downward but never mutates.

---

## What Belongs in This Document

This page should describe:

- immutable context responsibilities  
- how contexts are created, propagated, and consumed  
- how contexts collaborate with middleware, identity, and dispatchers  
- how contexts fit into the vertical slice lifecycle  

It should **not** include:

- product‑specific context types  
- mutable context patterns  
- infrastructure‑specific logging implementations  

Those belong in product or infrastructure documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Application immutable context implementation.  
Whenever context creation, propagation, or consumption evolves, update this section to reflect the current platform architecture.

