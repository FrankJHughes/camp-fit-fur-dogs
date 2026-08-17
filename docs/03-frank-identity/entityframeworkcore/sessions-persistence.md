# Frank.Identity.EntityFrameworkCore — Sessions Persistence

The **Sessions Persistence** subsystem defines how identity session state is stored, retrieved, and mapped using Entity Framework Core. While the Identity Application layer issues, validates, refreshes, and revokes sessions, this layer provides the **pure infrastructure** required to persist session domain models. It contains **no identity logic**, **no authentication flows**, and **no session rules** — only the durable storage mechanics that support the Identity vertical.

This document describes the Sessions Persistence subsystem under:

```
docs/03-frank-identity/entityframeworkcore
```

and maps it back to its implementation in:

```
src/Frank/Identity/EntityFrameworkCore
```

---

## Purpose

Sessions Persistence exists to:

- store identity session domain models in a durable, queryable form  
- map session value objects (IssuedAt, ExpiresAt, IdentityId, SessionId) to database columns  
- support session issuance, validation, and revocation through readers and writers  
- enforce database‑level constraints aligned with domain invariants  
- integrate session storage with platform‑wide EF Core infrastructure  

It is the persistence foundation for all identity session flows.

---

## Responsibilities of the Subsystem

### [Session Entity Configuration](ca://s?q=Explain_identity_session_entity_configuration)
Configurations define how session domain models map to database tables.

Responsibilities:

- map session timestamps (issued‑at, expiration)  
- map revocation markers  
- map identity metadata (IdentityId, ProviderSubject, ProviderIssuer)  
- configure required columns and constraints  
- configure indexes for fast lookup by session token or identity ID  
- map value objects using owned types or converters  

Configurations ensure persistence matches domain session invariants.

---

### [Session DbSet](ca://s?q=Explain_identity_dbcontext_sessions)
The DbContext exposes a DbSet for session state.

Responsibilities:

- provide EF Core access to session entities  
- integrate with identity readers and writers  
- apply configurations via `ApplyConfigurationsFromAssembly`  
- support provider‑specific column types and behaviors  

The DbSet is the entry point for all session persistence operations.

---

### [Session Readers](ca://s?q=Explain_identity_session_readers)
Readers provide slice‑specific read access to session data.

Responsibilities:

- load session state by session token  
- load session state by identity ID  
- load session state for validation and sliding expiration  
- return **domain session models**, not EF entities  

Readers contain no identity logic — they only retrieve data.

---

### [Session Writers](ca://s?q=Explain_identity_session_writers)
Writers persist session domain changes.

Responsibilities:

- create new session state during authentication  
- update expiration timestamps during sliding expiration  
- mark sessions as revoked  
- ensure domain invariants are validated before persistence  

Writers operate inside the identity unit‑of‑work boundary.

---

### [Session Unit of Work](ca://s?q=Explain_identity_unit_of_work)
The identity UoW coordinates session persistence operations.

Responsibilities:

- commit session issuance and lockout updates atomically  
- ensure revocation and expiration updates occur consistently  
- integrate with EF Core transaction boundaries  
- support audit logging and observability  

The UoW ensures session persistence is consistent and predictable.

---

## How Sessions Persistence Connects to the Broader Platform

Sessions Persistence collaborates with:

- **Frank.Identity.Application**  
  - session issuance writes new session state  
  - session validation reads session state  
  - revocation flows update session state  

- **Frank.Identity.Domain**  
  - domain session models define invariants  
  - value objects map directly to EF owned types  

- **Frank.Core.Infrastructure**  
  - logging, environment detection, exception handling  
  - database provider configuration  
  - migration tooling  

- **Frank.Core.Api**  
  - middleware relies on persisted session state for authentication  

Sessions Persistence is the durable backbone of identity session flows.

---

## Runtime Collaboration Points

Sessions Persistence interacts with the runtime by:

- loading session state for validation  
- persisting session issuance and sliding expiration  
- persisting revocation events  
- supporting lockout and authentication flows  
- emitting EF‑level logs for observability  
- integrating with platform‑wide migrations  

It ensures identity sessions remain durable, consistent, and observable.

---

## Composition Flow (Domain → EF Core → Application → API)

```
Domain Session Model
    ↓
Session Entity Configuration (owned types, converters)
    ↓
IdentityDbContext (DbSet<SessionState>)
    ↓
Session Readers / Writers
    ↓
Identity Application Services
    ↓
Identity API Endpoints
```

Sessions Persistence provides the storage foundation for all identity session behavior.

---

## What Belongs in This Document

- session entity configuration  
- session DbSet responsibilities  
- session readers and writers  
- session unit‑of‑work boundaries  
- database constraints and indexing strategy  
- how EF Core integrates with domain session models  

This document does **not** include:

- session issuance logic  
- session validation logic  
- lockout evaluation  
- HTTP endpoints  
- middleware behavior  
- domain invariants  

Those belong in the application or domain layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity EF Core session implementation.  
Whenever domain session models or persistence rules evolve, update this section to reflect the current platform architecture.
