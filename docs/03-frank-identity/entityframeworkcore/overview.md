# Frank.Identity.EntityFrameworkCore — Overview

The **EntityFrameworkCore** subsystem provides the persistence infrastructure for the Frank Identity vertical. It defines how identity domain models are mapped to database tables, how identity data is queried and written, and how schema evolution is managed through migrations. This layer contains **no identity logic**, **no authentication flows**, and **no session or lockout rules**. Its sole responsibility is to persist identity domain state in a clean, predictable, and slice‑aligned manner.

This document describes the EntityFrameworkCore subsystem under:

```
docs/03-frank-identity/entityframeworkcore
```

and maps it back to its implementation in:

```
src/Frank/Identity/EntityFrameworkCore
```

---

## Purpose

The EntityFrameworkCore layer exists to:

- persist identity domain models (Users, Sessions, LockoutState)  
- provide EF Core configurations for identity aggregates and value objects  
- expose readers and writers for vertical slices  
- implement identity‑specific unit‑of‑work boundaries  
- support schema evolution through EF Core migrations  
- integrate identity persistence with platform‑wide infrastructure  

It is the persistence backbone of the Identity subsystem.

---

## Responsibilities of the Subsystem

### [DbContexts](ca://s?q=Explain_identity_dbcontexts)
The DbContext defines the identity persistence model.

Responsibilities:

- expose DbSets for identity aggregates  
- apply entity configurations  
- bind EF Core provider settings  
- integrate with platform logging, environment detection, and migrations  

The DbContext is scoped per request and used by identity readers/writers.

---

### [Entity Configurations](ca://s?q=Explain_identity_entity_configurations)
Configurations map identity domain models to database tables.

Responsibilities:

- configure identity user storage  
- configure session state storage  
- configure lockout state storage  
- map value objects using owned types or converters  
- enforce database‑level constraints aligned with domain invariants  

Configurations ensure persistence matches domain expectations.

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

### [Migrations](ca://s?q=Explain_identity_migrations)
Migrations define schema evolution for identity persistence.

Responsibilities:

- create identity tables  
- evolve schema as domain models change  
- enforce database constraints  
- maintain indexes for identity lookup performance  
- support automated and manual migration workflows  

Migrations ensure identity persistence remains stable and predictable.

---

## How EntityFrameworkCore Connects to the Broader Platform

Identity EF Core collaborates with:

- **Frank.Identity.Application**  
  - application services call readers/writers  
  - UoW coordinates identity persistence  

- **Frank.Identity.Domain**  
  - domain models are persisted via EF configurations  
  - domain invariants influence schema constraints  

- **Frank.Core.Infrastructure**  
  - logging, environment detection, exception handling  
  - database provider configuration  
  - migration tooling  

- **Frank.Core.Api**  
  - middleware relies on persisted session and lockout state  

EF Core is the persistence engine behind identity flows.

---

## Runtime Collaboration Points

EntityFrameworkCore interacts with the runtime by:

- loading identity domain models for authentication  
- persisting session issuance and revocation  
- persisting lockout counters and timestamps  
- supporting rate‑limit evaluation (if persisted)  
- emitting EF‑level logs for observability  
- integrating with platform‑wide migrations  

It ensures identity state is durable, consistent, and observable.

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

EntityFrameworkCore provides the persistence foundation for all identity flows.

---

## What Belongs in This Document

- DbContext responsibilities  
- entity configuration responsibilities  
- value object mapping rules  
- identity readers and writers  
- identity unit‑of‑work boundaries  
- migration responsibilities  
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
