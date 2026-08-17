# Frank.Core.EntityFrameworkCore — Overview

The `Frank.Core.EntityFrameworkCore` subsystem provides the persistence foundation for all Frank‑based products. It defines how aggregates are materialized, tracked, and committed; how value objects and strongly typed identifiers are mapped; and how transactional boundaries are enforced through the Unit of Work.

This document maps the EntityFrameworkCore subsystem under:

```
docs/02-frank-core/entityframeworkcore
```

back to its implementation in:

```
src/Frank/Core/EntityFrameworkCore
```

---

## Purpose

The EF Core subsystem exists to:

- persist aggregates and entities  
- materialize domain objects without invoking domain logic  
- enforce transactional boundaries via the Unit of Work  
- integrate domain event dispatching with persistence  
- map value objects and strongly typed identifiers  
- keep domain logic pure and free of ORM concerns  

It is the persistence engine for the entire Frank platform.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/EntityFrameworkCore`

- **Documentation folder:**  
  `docs/02-frank-core/entityframeworkcore`

This documentation must remain aligned with the actual EF Core context, Unit of Work, and mapping conventions.

---

## What Belongs Here

### Responsibilities of the Subsystem

- mapping aggregates, entities, and value objects  
- configuring backing fields and owned types  
- converting strongly typed IDs  
- tracking aggregate mutations  
- coordinating domain event dispatch before commit  
- enforcing transactional boundaries  

### Platform Integration

EF Core collaborates with:

- **Frank.Core.Domain** — aggregates define invariants; EF Core persists them  
- **Frank.Core.Application** — handlers mutate aggregates; EF Core tracks changes  
- **Frank.Core.Infrastructure** — observation context logs database operations  
- **Unit of Work** — EF Core commits changes atomically  

### Runtime Collaboration Points

- aggregates materialized via private constructors  
- value objects persisted through owned types  
- strongly typed IDs converted to primitives  
- domain events dispatched before commit  
- DbContext scoped per request  

### Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Application Handler
    ↓
Domain Aggregate (state mutated)
    ↓
EF Core Tracking
    ↓
Domain Events Dispatched
    ↓
Unit of Work Commit
    ↓
Database
```

EF Core is the final step in the vertical slice lifecycle.

---

## Notes

Keep this document grounded in the actual Frank.Core.EntityFrameworkCore implementation.  
Whenever mapping conventions, Unit of Work behavior, or domain event integration evolves, update this section to reflect the current platform architecture.
