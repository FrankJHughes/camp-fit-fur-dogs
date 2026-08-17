# Frank.Identity.EntityFrameworkCore — Users Persistence

The **Users Persistence** subsystem defines how identity‑level user information is stored, retrieved, and mapped using Entity Framework Core. While the Identity Application layer handles authentication, claim mapping, and session issuance, this layer provides the **pure infrastructure** required to persist identity user domain models. It contains **no identity logic**, **no claim‑mapping rules**, and **no authentication flows** — only the durable storage mechanics that support the Identity vertical.

This document describes the Users Persistence subsystem under:

```
docs/03-frank-identity/entityframeworkcore
```

and maps it back to its implementation in:

```
src/Frank/Identity/EntityFrameworkCore
```

---

## Purpose

Users Persistence exists to:

- store identity user domain models in a durable, queryable form  
- map identity value objects (ProviderSubject, ProviderIssuer, IdentityId) to database columns  
- support authentication flows through readers and writers  
- enforce database‑level constraints aligned with domain invariants  
- integrate identity user storage with platform‑wide EF Core infrastructure  

It is the persistence foundation for all identity user behavior.

---

## Responsibilities of the Subsystem

### [User Entity Configuration](ca://s?q=Explain_identity_user_entity_configuration)
Configurations define how identity user domain models map to database tables.

Responsibilities:

- map provider subject (`sub`) and issuer (`iss`)  
- map platform identity identifier (`IdentityId`)  
- map normalized identity claims (if persisted)  
- configure required columns and constraints  
- configure indexes for fast lookup by provider subject or identity ID  
- map value objects using owned types or converters  

Configurations ensure persistence matches domain identity invariants.

---

### [User DbSet](ca://s?q=Explain_identity_dbcontext_users)
The DbContext exposes a DbSet for identity users.

Responsibilities:

- provide EF Core access to user entities  
- integrate with identity readers and writers  
- apply configurations via `ApplyConfigurationsFromAssembly`  
- support provider‑specific column types and behaviors  

The DbSet is the entry point for all user persistence operations.

---

### [User Readers](ca://s?q=Explain_identity_user_readers)
Readers provide slice‑specific read access to identity user data.

Responsibilities:

- load identity users by provider subject  
- load identity users by identity ID  
- load identity users for authentication flows  
- return **domain user models**, not EF entities  

Readers contain no identity logic — they only retrieve data.

---

### [User Writers](ca://s?q=Explain_identity_user_writers)
Writers persist identity user domain changes.

Responsibilities:

- create new identity users during first‑time authentication  
- update identity user metadata when claims change  
- ensure domain invariants are validated before persistence  
- operate inside the identity unit‑of‑work boundary  

Writers ensure identity user state is durable and consistent.

---

### [Unit of Work](ca://s?q=Explain_identity_unit_of_work)
The identity UoW coordinates user persistence operations.

Responsibilities:

- commit user creation and updates atomically  
- ensure user persistence aligns with session issuance and lockout updates  
- integrate with EF Core transaction boundaries  
- support audit logging and observability  

The UoW ensures user persistence is consistent and predictable.

---

## How Users Persistence Connects to the Broader Platform

Users Persistence collaborates with:

- **Frank.Identity.Application**  
  - authentication services create or update identity users  
  - session services embed identity user metadata  
  - lockout services evaluate identity users  

- **Frank.Identity.Domain**  
  - domain user models define invariants  
  - value objects map directly to EF owned types  

- **Frank.Core.Infrastructure**  
  - logging, environment detection, exception handling  
  - database provider configuration  
  - migration tooling  

- **Frank.Core.Api**  
  - middleware relies on persisted identity user state for authorization  

Users Persistence is the durable backbone of identity user flows.

---

## Runtime Collaboration Points

Users Persistence interacts with the runtime by:

- loading identity users for authentication  
- persisting identity user creation and updates  
- supporting session issuance and validation  
- supporting lockout evaluation  
- emitting EF‑level logs for observability  
- integrating with platform‑wide migrations  

It ensures identity users remain durable, consistent, and observable.

---

## Composition Flow (Domain → EF Core → Application → API)

```
Domain Identity User Model
    ↓
User Entity Configuration (owned types, converters)
    ↓
IdentityDbContext (DbSet<IdentityUser>)
    ↓
User Readers / Writers
    ↓
Identity Application Services
    ↓
Identity API Endpoints
```

Users Persistence provides the storage foundation for all identity user behavior.

---

## What Belongs in This Document

- user entity configuration  
- user DbSet responsibilities  
- user readers and writers  
- user unit‑of‑work boundaries  
- database constraints and indexing strategy  
- how EF Core integrates with domain identity user models  

This document does **not** include:

- authentication logic  
- claim‑mapping rules  
- session issuance logic  
- lockout evaluation  
- HTTP endpoints  
- middleware behavior  
- domain invariants  

Those belong in the application or domain layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity EF Core user implementation.  
Whenever domain identity models or persistence rules evolve, update this section to reflect the current platform architecture.
