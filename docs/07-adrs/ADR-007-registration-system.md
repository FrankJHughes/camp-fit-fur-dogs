# ADR 007 — Registration System

## Status
Accepted

## Context

The platform requires a reliable, consistent, and secure registration system for new owners.  
Registration is a core workflow that touches multiple subsystems:

- Domain: creation of Owner aggregates  
- Application: commands, validation, and business rules  
- Infrastructure: persistence, email delivery, identity integration  
- API: endpoints for account creation and verification  

A decision record is needed to document how registration is structured, how it integrates  
with vertical slices, and how it interacts with external identity providers (Auth0).

## Decision

Adopt a vertical-slice‑based registration system that:

- Creates Owner aggregates through domain factories  
- Uses commands for registration and queries for verification  
- Integrates with Auth0 for identity and authentication  
- Sends verification and welcome emails through the Email subsystem  
- Persists owners using EF Core and PostgreSQL  
- Ensures all registration logic is isolated within the `Owners` slice  

This keeps registration self‑contained, testable, and aligned with the architecture.

## Consequences

### Positive

- Clear separation — registration logic lives entirely in the Owners slice  
- Strong domain modeling — Owner aggregate enforces invariants  
- Secure — identity handled by Auth0, not custom logic  
- Extensible — easy to add email verification, welcome emails, lockout, etc.  
- Testable — commands, handlers, and domain rules can be tested independently  
- Consistent — follows CQRS and vertical slice patterns  

### Negative

- Requires coordination with external identity provider  
- More moving parts (email, identity, persistence)  
- Must maintain consistency between Auth0 user and domain Owner aggregate  

## Implementation

- `RegisterOwnerCommand` creates the Owner aggregate  
- `RegisterOwnerCommandHandler` validates input and persists the aggregate  
- Auth0 handles identity creation and authentication  
- Email subsystem sends verification and welcome emails  
- Owner repository persists aggregates using EF Core  
- Registration endpoints live under `Api/Endpoints/Owners`  
- Domain events may be raised (e.g., `OwnerRegisteredEvent`)  

## Related

- ADR 001 — Vertical Slice Architecture  
- ADR 002 — CQRS Pattern  
- ADR 003 — EF Core  
- ADR 004 — PostgreSQL  
- ADR 005 — Unit of Work  
- ADR 006 — Immutable Contexts  
- US‑126 — Create Account Page  
- US‑148 — Email Verification  
- US‑145 — Welcome Email  

## Notes

Keep this document grounded in the actual registration implementation and update it  
as the source architecture evolves.
