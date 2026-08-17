# ADR 005 — Unit of Work

## Status
Accepted

## Context

The application uses EF Core as its persistence layer.  
While EF Core implicitly manages transactions through `SaveChangesAsync`,  
complex vertical slices often require explicit coordination of multiple  
repository operations, domain events, and transactional guarantees.

A Unit of Work pattern provides a consistent abstraction for committing  
changes, ensuring atomicity, and integrating domain event dispatching.

## Decision

Adopt a Unit of Work abstraction to coordinate persistence operations  
within each vertical slice. The Unit of Work encapsulates:

- Transaction boundaries  
- Aggregate persistence  
- Domain event collection and dispatch  
- Consistent commit semantics  

This ensures that all write operations follow a predictable, testable,  
and observable workflow.

## Consequences

### Positive

- Atomic commits — multiple operations succeed or fail together  
- Clear transactional boundaries  
- Centralized domain event dispatching  
- Consistent write workflow across vertical slices  
- Improved testability through mockable interfaces  
- Reduced accidental partial writes  

### Negative

- Additional abstraction layer  
- Requires discipline to avoid bypassing the Unit of Work  
- Slightly more boilerplate in infrastructure  

## Implementation

- `IUnitOfWork` exposes `CommitAsync()` and `RollbackAsync()`  
- EF Core `DbContext` is wrapped by the Unit of Work implementation  
- Repositories register changes with the Unit of Work  
- Domain events are collected during aggregate operations  
- After a successful commit, domain events are dispatched  
- Each vertical slice uses its own repository + Unit of Work combination  

## Related

- ADR 003 — EF Core  
- ADR 004 — PostgreSQL  
- ADR 006 — Immutable Contexts  
- ADR 001 — Vertical Slice Architecture  
- ADR 002 — CQRS Pattern  

## Notes

Keep this document grounded in the actual Unit of Work implementation  
and update it as the source architecture evolves.
