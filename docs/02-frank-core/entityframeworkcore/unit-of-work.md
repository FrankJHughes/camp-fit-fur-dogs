# Frank.Core.EntityFrameworkCore — Unit of Work

The Unit of Work subsystem in `Frank.Core.EntityFrameworkCore` coordinates transactional persistence for all Frank‑based products. It ensures that aggregate mutations, domain event dispatching, and database writes occur inside a single, atomic operation. This keeps the domain model consistent and prevents partial updates from leaking into the system.

This document maps the Unit of Work subsystem under:

```
docs/02-frank-core/entityframeworkcore
```

back to its implementation in:

```
src/Frank/Core/EntityFrameworkCore
```

---

## Purpose

The Unit of Work exists to:

- ensure all aggregate changes are committed atomically  
- coordinate domain event dispatching before persistence  
- track aggregate mutations through the DbContext  
- provide a consistent transactional boundary for each request  
- prevent partial writes or inconsistent domain state  
- keep domain logic pure and free of persistence concerns  

It is the transactional backbone of the vertical slice architecture.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core/EntityFrameworkCore/UnitOfWork`

- **Documentation folder:**  
  `docs/02-frank-core/entityframeworkcore`

This documentation must remain aligned with the actual Unit of Work implementation and EF Core integration.

---

## Responsibilities of the Unit of Work

### Transactional Boundary

The Unit of Work wraps all domain mutations inside a single transaction:

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

If any step fails, the entire operation is rolled back.

### Domain Event Dispatching

Before committing changes, the Unit of Work:

1. collects domain events from all tracked aggregates  
2. dispatches them through the application event dispatcher  
3. ensures handlers run inside the same transaction  

This guarantees that domain events reflect persisted state.

### Change Tracking

The Unit of Work relies on EF Core’s DbContext to track:

- aggregate roots  
- entities  
- owned value objects  
- navigation properties  

Only mutated aggregates are written to the database.

### Commit Semantics

A typical Unit of Work exposes:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}
```

Responsibilities include:

- flushing tracked changes  
- dispatching domain events  
- committing the transaction  
- returning the number of affected rows  

### Isolation From Domain Logic

The Unit of Work:

- does not enforce domain invariants  
- does not perform validation  
- does not contain business rules  

Its sole responsibility is persistence and event coordination.

---

## How the Unit of Work Connects to the Broader Platform

The Unit of Work collaborates with:

- **Frank.Core.Domain**  
  Aggregates raise domain events; UoW dispatches them.

- **Frank.Core.Application**  
  Handlers invoke UoW to commit changes.

- **Frank.Core.Infrastructure**  
  Observation context logs commit operations.

- **DbContext**  
  UoW delegates tracking and persistence to EF Core.

This ensures consistent behavior across all vertical slices.

---

## Runtime Collaboration Points

The Unit of Work interacts with the runtime by:

- coordinating domain event dispatch  
- enforcing atomic writes  
- preventing partial updates  
- ensuring consistent logging and correlation  
- integrating with scoped DbContext lifetime  

It is the final gatekeeper before persistence.

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
Unit of Work (dispatch events + commit)
    ↓
Database
```

The Unit of Work ensures that domain changes and event propagation occur together.

---

## What Belongs in This Document

This page should describe:

- Unit of Work responsibilities  
- how transactions are coordinated  
- how domain events are dispatched  
- how DbContext integrates with UoW  
- how UoW fits into the vertical slice lifecycle  

It should **not** include:

- product‑specific transaction logic  
- infrastructure‑specific database tuning  
- raw SQL or migration details  

Those belong in product or infrastructure documentation.

---

## Notes

Keep this document grounded in the actual Frank.Core.EntityFrameworkCore Unit of Work implementation.  
Whenever commit semantics, event dispatching, or DbContext integration evolve, update this section to reflect the current platform architecture.
