# ADR 006 — Immutable Contexts

## Status
Accepted

## Context

The application uses EF Core and vertical slice architecture.  
Most slices require deterministic, predictable behavior when interacting with  
the database, especially during testing, command execution, and domain event  
processing. Mutable `DbContext` instances can introduce:

- accidental state leakage between operations  
- nondeterministic behavior in tests  
- unintended tracking of entities  
- coupling between slices through shared context state  

Immutable contexts ensure that each operation (command, query, test, or handler)  
receives a clean, isolated persistence boundary.

## Decision

Adopt immutable EF Core contexts for all vertical slices.

An immutable context is:

- created per operation  
- not reused across handlers  
- configured with deterministic options  
- free of shared mutable state  
- used only within the scope of a single command or query  

This ensures isolation, purity, and predictable behavior across the system.

## Consequences

### Positive

- Deterministic behavior — each operation gets a clean context  
- No accidental cross-slice state sharing  
- Improved test reliability  
- Clear transactional boundaries  
- Reduced risk of unintended entity tracking  
- Aligns with vertical slice independence  

### Negative

- More context instantiations per request  
- Slight performance overhead (negligible for typical workloads)  
- Requires discipline to avoid static or shared contexts  

## Implementation

- Each handler receives its own context instance  
- Context lifetime is scoped per request or per handler  
- No static or singleton `DbContext` instances  
- Tracking is explicitly controlled (AsNoTracking for queries)  
- Context configuration is centralized in the infrastructure layer  
- Tests use deterministic in-memory or PostgreSQL contexts  
- Contexts are created through DI using scoped lifetimes  

## Related

- ADR 003 — EF Core  
- ADR 004 — PostgreSQL  
- ADR 005 — Unit of Work  
- ADR 001 — Vertical Slice Architecture  
- ADR 002 — CQRS Pattern  

## Notes

Keep this document grounded in the actual EF Core and context lifecycle  
implementation and update it as the architecture evolves.
