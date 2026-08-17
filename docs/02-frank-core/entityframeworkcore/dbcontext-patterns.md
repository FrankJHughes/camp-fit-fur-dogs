# Frank.Core.EntityFrameworkCore — DbContext Patterns

The DbContext subsystem in `Frank.Core.EntityFrameworkCore` defines how aggregates, entities, and value objects are tracked, materialized, and persisted. These patterns ensure that domain logic remains pure, persistence remains predictable, and transactional boundaries are enforced consistently across all vertical slices.

This document maps the DbContext patterns under:

```
docs/02-frank-core/entityframeworkcore
```

back to their implementation in:

```
src/Frank/Core/EntityFrameworkCore
```

---

## Purpose

DbContext patterns exist to:

- materialize aggregates without invoking domain logic  
- track changes to aggregates and entities  
- enforce transactional boundaries via the Unit of Work  
- persist value objects and strongly typed identifiers  
- integrate domain event dispatching with persistence  
- keep domain logic isolated from ORM concerns  

These patterns form the backbone of Frank’s persistence model.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/EntityFrameworkCore`

- **Documentation folder:**  
  `docs/02-frank-core/entityframeworkcore`

This documentation must remain aligned with the actual DbContext, Unit of Work, and mapping conventions.

---

## Responsibilities of the DbContext Subsystem

### Aggregate Materialization

EF Core materializes aggregates using:

- private parameterless constructors  
- backing fields  
- owned types for value objects  
- strongly typed ID converters  

No domain logic is executed during materialization. Aggregates are hydrated into a valid state based solely on persisted data.

### Change Tracking

DbContext tracks:

- aggregate root state  
- entity collections  
- owned value objects  
- navigation properties  

Tracking ensures that only mutated aggregates are persisted during the Unit of Work commit.

### Unit of Work Integration

DbContext participates in the Unit of Work:

```
Begin Request
    ↓
Handler Mutates Aggregate
    ↓
DbContext Tracks Changes
    ↓
Domain Events Dispatched
    ↓
Commit Transaction
```

The Unit of Work ensures atomic writes and consistent domain event dispatching.

### Value Object Persistence

Value objects are persisted via:

- owned entity types  
- backing fields  
- custom converters (when needed)  

This preserves immutability while allowing EF Core to store their values.

### Strongly Typed ID Conversion

Strongly typed IDs (e.g., `DogId`, `UserId`) are converted to/from primitives:

```csharp
builder.Property(x => x.Id)
    .HasConversion(
        id => id.Value,
        value => new DogId(value));
```

This keeps domain identity types pure while storing simple primitives in the database.

### Context Lifetime

DbContext is registered with **scoped** lifetime:

- one context per request  
- ensures consistent tracking  
- prevents cross‑request contamination  
- aligns with Unit of Work boundaries  

---

## How DbContext Connects to the Broader Platform

DbContext collaborates with:

- **Frank.Core.Domain** — aggregates define invariants; DbContext persists them  
- **Frank.Core.Application** — handlers mutate aggregates; DbContext tracks changes  
- **Frank.Core.Infrastructure** — observation context logs database operations  
- **Unit of Work** — DbContext commits changes and triggers domain event dispatch  

DbContext is the persistence engine for the vertical slice.

---

## Runtime Collaboration Points

DbContext interacts with the runtime by:

- materializing aggregates via private constructors  
- tracking mutations during handler execution  
- converting strongly typed IDs  
- persisting value objects  
- dispatching domain events before commit  
- enforcing transactional boundaries  

This ensures persistence is deterministic and domain‑safe.

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Application Handler
    ↓
Aggregate Mutations
    ↓
DbContext Tracking
    ↓
Domain Events Dispatched
    ↓
Unit of Work Commit
    ↓
Database
```

DbContext is the final step before persistence.

---

## What Belongs in This Document

This page should describe:

- DbContext responsibilities  
- materialization patterns  
- tracking behavior  
- value object and ID conversion  
- Unit of Work integration  
- how DbContext fits into the vertical slice lifecycle  

It should **not** include:

- product‑specific DbContexts  
- raw SQL migrations  
- infrastructure‑specific database tuning  

Those belong in product or infrastructure documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.EntityFrameworkCore DbContext implementation.  
Whenever mapping conventions, tracking behavior, or Unit of Work integration evolves, update this section to reflect the current platform architecture.
