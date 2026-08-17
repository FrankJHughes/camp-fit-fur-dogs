# Frank.Identity.EntityFrameworkCore — Unit of Work

The **Unit of Work (UoW)** subsystem in the Identity EntityFrameworkCore layer defines how identity persistence operations are coordinated, committed, and observed. While the Identity Application layer orchestrates identity flows (authentication, session issuance, lockout evaluation), the EF Core UoW provides the **transactional boundary** that ensures identity state changes are written atomically and consistently. This layer contains **no identity logic**, **no domain invariants**, and **no authentication rules** — it is strictly infrastructure.

This document describes the Unit of Work subsystem under:

```
docs/03-frank-identity/entityframeworkcore
```

and maps it back to its implementation in:

```
src/Frank/Identity/EntityFrameworkCore
```

---

## Purpose

The EF Core Unit of Work exists to:

- provide a transactional boundary for identity persistence  
- coordinate writes across identity users, sessions, and lockout state  
- ensure identity changes succeed or fail as a single operation  
- integrate EF Core transactions with platform‑wide observability  
- support identity application flows without leaking identity logic  

It is the infrastructure mechanism that keeps identity persistence consistent.

---

## Responsibilities of the Subsystem

### [Transactional Boundary](ca://s?q=Explain_identity_unit_of_work_boundary)
The UoW ensures identity persistence operations are atomic.

Responsibilities:

- wrap identity writes in a single EF Core transaction  
- ensure session issuance and lockout updates commit together  
- ensure revocation and expiration updates commit together  
- prevent partial identity state updates  

Identity logic determines *what* to write; UoW determines *how* it commits.

---

### [Commit Semantics](ca://s?q=Explain_identity_unit_of_work_commit)
Commit behavior ensures consistent persistence.

Responsibilities:

- call `SaveChangesAsync()` once per identity flow  
- propagate EF Core exceptions to the application layer  
- ensure audit logging occurs after successful commit  
- ensure rollback occurs on failure  

Commit semantics guarantee predictable persistence behavior.

---

### [Integration with Readers & Writers](ca://s?q=Explain_identity_readers_writers_integration)
The UoW coordinates identity persistence operations.

Responsibilities:

- ensure writers operate inside the UoW boundary  
- ensure readers participate in the same DbContext scope  
- ensure domain models are validated before persistence  
- ensure EF entities are never exposed outside infrastructure  

Readers and writers rely on UoW for consistency.

---

### [Observability & Logging](ca://s?q=Explain_identity_observability)
The UoW integrates with platform‑wide observability.

Responsibilities:

- emit logs for commit and rollback events  
- attach correlation IDs and causation chains  
- support environment‑specific logging verbosity  
- surface EF Core exceptions with structured metadata  

Observability ensures identity persistence is diagnosable.

---

### [Provider‑Specific Behavior](ca://s?q=Explain_identity_provider_specific_behavior)
The UoW adapts to the configured database provider.

Responsibilities:

- support SQL Server / PostgreSQL / SQLite transaction semantics  
- handle provider‑specific concurrency behaviors  
- ensure consistent commit behavior across environments  

Provider behavior is configured in the DbContext.

---

## How the Unit of Work Connects to the Broader Platform

Identity EF Core UoW collaborates with:

- **Frank.Identity.Application**  
  - application services call UoW to commit identity changes  
  - UoW ensures identity flows persist atomically  

- **Frank.Identity.Domain**  
  - domain models are validated before persistence  
  - domain invariants influence what the UoW commits  

- **Frank.Core.Infrastructure**  
  - logging, environment detection, exception handling  
  - database provider configuration  
  - migration tooling  

- **Frank.Core.Api**  
  - middleware relies on persisted session and lockout state  

The UoW is the persistence engine behind identity flows.

---

## Runtime Collaboration Points

The UoW interacts with the runtime by:

- committing identity state changes  
- rolling back on failure  
- supporting authentication and session flows  
- supporting lockout evaluation  
- emitting EF‑level logs for observability  
- integrating with platform‑wide migrations  

It ensures identity persistence remains durable and predictable.

---

## Composition Flow (Application → Domain → EF Core → Commit)

```
Identity Application Flow
    ↓
Domain Models Validated
    ↓
Identity Writers Persist Changes
    ↓
Unit of Work Commit
        - SaveChangesAsync()
        - Transaction commit
        - Audit logging
    ↓
Identity API Returns Result
```

The UoW ensures identity persistence is consistent across all identity flows.

---

## What Belongs in This Document

- UoW responsibilities  
- transactional boundaries  
- commit semantics  
- integration with readers and writers  
- observability and logging behavior  
- provider‑specific considerations  

This document does **not** include:

- identity logic  
- authentication flows  
- session issuance rules  
- lockout evaluation  
- HTTP endpoints  
- middleware behavior  
- domain invariants  

Those belong in the application or domain layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity EF Core UoW implementation.  
Whenever identity persistence rules or domain models evolve, update this section to reflect the current platform architecture.
