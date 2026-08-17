# Frank.Core.Infrastructure — Observations

The **Observations** subsystem provides unified, structured, and environment‑aware logging and tracing across the entire Frank platform. It ensures that every vertical slice—API, application, domain, and persistence—emits consistent, correlated, and machine‑processable diagnostic information.

This document maps the Observations subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Observations
```

---

## Purpose

The Observations subsystem exists to:

- provide structured logging and tracing  
- attach correlation and causation metadata to every operation  
- unify diagnostic output across all layers  
- support environment‑specific verbosity and formatting  
- ensure domain events, commands, queries, and exceptions are observable  
- keep logging concerns out of domain and application logic  

It is the backbone of runtime visibility for the Frank platform.

---

## Responsibilities of the Subsystem

### Structured Logging

Observations enforce structured, semantic logging:

- logs are emitted as key/value pairs  
- domain values (IDs, names, timestamps) are included  
- logs are machine‑readable and compatible with modern platforms (Seq, ELK, Render logs, etc.)  
- no string‑concatenated or unstructured logs  

This ensures logs can be queried, filtered, and correlated reliably.

### Correlation & Causation Metadata

Every request and operation receives:

- **Correlation ID** — ties together logs across layers  
- **Causation ID** — identifies the triggering event or command  
- **Operation Name** — identifies the vertical slice or handler  

This metadata flows through:

- API middleware  
- application handlers  
- domain event dispatch  
- persistence operations  

### Environment‑Aware Behavior

Observations adjust based on environment:

- **Development** — verbose logs, debug metadata  
- **Staging** — structured logs with moderate verbosity  
- **Production** — strict formatting, minimal noise, high signal  

Environment detection is provided by the `IEnvironment` service.

### Domain Event Logging

When domain events are raised:

- Observations capture event type, aggregate ID, and timestamp  
- events are logged before dispatch  
- handlers log completion and failures  
- correlation metadata ties events back to the originating request  

### Exception Logging

Infrastructure exception handling integrates with Observations:

- domain exceptions include domain metadata  
- application exceptions include handler metadata  
- infrastructure exceptions include environment metadata  
- all exceptions include correlation IDs  

This ensures failures are diagnosable and traceable.

---

## How Observations Connect to the Broader Platform

Observations collaborate with:

- **Frank.Core.Api**  
  Middleware attaches correlation IDs and logs request lifecycle.

- **Frank.Core.Application**  
  Pipeline behaviors log command/query execution, timing, and failures.

- **Frank.Core.Domain**  
  Domain events and aggregate operations emit structured logs.

- **Frank.Core.EntityFrameworkCore**  
  Persistence operations (queries, commits, failures) are logged.

- **Frank.Core.Infrastructure**  
  Environment detection shapes logging verbosity and formatting.

Observations unify diagnostics across all layers.

---

## Runtime Collaboration Points

Observations interact with the runtime by:

- capturing request start/end  
- logging command and query execution  
- recording domain event emission and dispatch  
- logging persistence operations  
- capturing exceptions with correlation metadata  
- adjusting verbosity based on environment  

This ensures the platform is fully observable during development, staging, and production.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Request
    ↓
Observation Middleware (correlation ID)
    ↓
Application Handler (structured logs)
    ↓
Domain Logic (event logs)
    ↓
Unit of Work Commit (persistence logs)
    ↓
API Response (completion logs)
```

Observations provide a complete trace of every vertical slice.

---

## What Belongs in This Document

This page should describe:

- structured logging responsibilities  
- correlation/causation metadata  
- environment‑aware logging behavior  
- how Observations integrate with domain events and exceptions  
- how Observations fit into the vertical slice lifecycle  

It should **not** include:

- product‑specific log formats  
- external logging platform configuration  
- deployment‑specific log routing  

Those belong in product or deployment documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure Observations implementation.  
Whenever logging patterns, correlation strategies, or environment‑specific behavior evolve, update this section to reflect the current platform architecture.
