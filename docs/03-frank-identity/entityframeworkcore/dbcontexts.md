# Frank.Identity.EntityFrameworkCore — DbContexts

The **DbContexts** subsystem provides the persistence boundary for the Frank Identity vertical. It defines how identity domain models (Users, Sessions, LockoutState) are stored, retrieved, and mapped using Entity Framework Core. This layer contains **no identity logic**, **no authentication flows**, and **no session rules** — it is strictly infrastructure that supports the Identity Application layer.

This document describes the DbContexts subsystem under:

```
docs/03-frank-identity/entityframeworkcore
```

and maps it back to its implementation in:

```
src/Frank/Identity/EntityFrameworkCore
```

---

## Purpose

The DbContexts subsystem exists to:

- define the EF Core persistence model for identity  
- expose DbSets for identity domain aggregates  
- apply entity configurations and value‑object mappings  
- integrate identity persistence with platform‑wide infrastructure  
- support identity readers, writers, and unit‑of‑work boundaries  

DbContexts are the persistence backbone of the Identity subsystem.

---

## Responsibilities of the Subsystem

### [IdentityDbContext](ca://s?q=Explain_identity_dbcontext)
The central EF Core context for identity persistence.

Responsibilities:

- expose DbSets for identity entities:
  - IdentityUser  
  - SessionState  
  - LockoutState  
- apply configurations via `ApplyConfigurationsFromAssembly`  
- configure provider‑specific behaviors (SQL Server, PostgreSQL, etc.)  
- integrate with platform logging, environment detection, and migrations  

IdentityDbContext is scoped per request and used by identity readers/writers.

---

### [Entity Configuration](ca://s?q=Explain_identity_entity_configurations)
Configurations map domain models to database tables.

Responsibilities:

- configure identity user storage (keys, indexes, constraints)  
- configure session state (timestamps, revocation markers)  
- configure lockout state (failure counters, timestamps)  
- map value objects using owned types or converters  
- enforce database‑level constraints aligned with domain invariants  

Configurations ensure persistence matches domain expectations.

---

### [Value Object Mapping](ca://s?q=Explain_identity_value_object_mapping)
Identity domain value objects are mapped using EF Core owned types.

Examples:

- ProviderSubject  
- ProviderIssuer  
- IdentityId  
- SessionId  
- Lockout counters and timestamps  

Responsibilities:

- ensure value objects remain immutable  
- ensure database representation matches domain invariants  
- prevent invalid identity metadata from being persisted  

Value object mapping keeps persistence aligned with domain purity.

---

### [Readers](ca://s?q=Explain_identity_readers)
Readers provide slice‑specific read access to identity data.

Responsibilities:

- load identity users by provider subject or identity ID  
- load session state by session token or identity ID  
- load lockout state for authentication flows  
- return **domain models**, not EF entities  

Readers contain no identity logic — they only retrieve data.

---

### [Writers](ca://s?q=Explain_identity_writers)
Writers persist identity domain changes.

Responsibilities:

- create or update identity users  
- persist session issuance and revocation  
- persist lockout counters and timestamps  
- ensure domain invariants are validated before persistence  

Writers operate inside the identity unit‑of‑work boundary.

---

### [Unit of Work](ca://s?q=Explain_identity_unit_of_work)
Identity UoW coordinates persistence operations.

Responsibilities:

- commit identity changes atomically  
- ensure lockout updates and session issuance occur together  
- integrate with EF Core transaction boundaries  
- support audit logging and observability  

Identity UoW is infrastructure, not domain logic.

---

## How DbContexts Connect to the Broader Platform

Identity EF Core collaborates with:

- **Frank.Identity.Application**  
  - application services call readers/writers  
  - UoW coordinates identity persistence  

- **Frank.Identity.Domain**  
  - domain models are persisted via EF configurations  
  - domain invariants are enforced before persistence  

- **Frank.Core.Infrastructure**  
  - logging, environment detection, exception handling  
  - database provider configuration  
  - migration tooling  

- **Frank.Core.Api**  
  - middleware relies on persisted session and lockout state  

DbContexts are the persistence engine behind identity flows.

---

## Runtime Collaboration Points

DbContexts interact with the runtime by:

- loading identity domain models for authentication  
- persisting session issuance and revocation  
- persisting lockout counters and timestamps  
- supporting rate‑limit evaluation (if persisted)  
- emitting EF‑level logs for observability  
- integrating with platform‑wide migrations  

DbContexts ensure identity state is durable, consistent, and observable.

---

## Composition Flow (Domain → EF Core → Application → API)

```
Identity Domain Model
    ↓
Entity Configuration (owned types, converters)
    ↓
IdentityDbContext
    ↓
Readers / Writers
    ↓
Identity Application Services
    ↓
Identity API Endpoints
```

DbContexts provide the persistence foundation for all identity flows.

---

## What Belongs in This Document

- DbContext responsibilities  
- entity configuration responsibilities  
- value object mapping rules  
- identity readers and writers  
- identity unit‑of‑work boundaries  
- how EF Core integrates with identity domain and application layers  

This document does **not** include:

- identity logic  
- authentication flows  
- session issuance rules  
- lockout evaluation  
- HTTP endpoints  
- middleware behavior  

Those belong in the application or API layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity EF Core implementation.  
Whenever identity domain models, persistence rules, or configuration patterns evolve, update this section to reflect the current platform architecture.
