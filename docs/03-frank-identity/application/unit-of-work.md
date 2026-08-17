# Frank.Identity.Application — Unit of Work

The Identity Application layer uses a lightweight, identity‑specific Unit of Work pattern to coordinate operations that require consistency across authentication, session management, lockout evaluation, and identity‑provider interactions. Unlike domain‑level units of work, the identity UoW focuses on **identity state**, **session state**, and **provider‑interaction consistency**, not aggregate mutation or transactional domain persistence.

This document describes the Unit of Work subsystem under:

```
docs/03-frank-identity/application
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

The Identity Unit of Work exists to:

- coordinate identity‑related operations that must succeed or fail together  
- ensure consistency across authentication, session issuance, and lockout updates  
- provide a single execution boundary for identity flows  
- isolate identity state changes from domain persistence  
- support audit logging and observability within a unified operation  

Identity UoW is intentionally minimal — identity is not a domain subsystem.

---

## Responsibilities of the Subsystem

### Coordinating Identity Operations  
Identity flows often require multiple steps that must be treated as a single operation.  
See: [Authentication Services](ca://s?q=Explain_identity_authentication_services)

Examples:

- exchanging OIDC authorization codes  
- validating tokens  
- updating lockout counters  
- issuing session tokens  
- writing audit logs  

The Unit of Work ensures these steps execute in a controlled sequence.

---

### Managing Identity State  
Identity UoW manages state relevant to identity flows:

- lockout counters  
- session revocation markers  
- provider‑interaction metadata  
- identity‑specific caches  

This state is not domain state and does not involve aggregates.

---

### Ensuring Consistency  
Identity operations must remain consistent even without domain transactions.

The UoW ensures:

- lockout updates occur only after authentication failure  
- lockout resets occur only after authentication success  
- session issuance occurs only after token validation  
- audit logs reflect the final outcome of the identity flow  

Consistency is enforced through controlled sequencing, not database transactions.

---

### Integrating with Audit Logging  
Audit logging is part of the identity UoW boundary.  
See: [Audit Logging](ca://s?q=Explain_identity_authentication_logging)

Responsibilities:

- emit logs for each identity step  
- correlate logs using observation context  
- ensure logs reflect the final state of the identity operation  

Audit logs are written as part of the UoW lifecycle.

---

### Supporting Environment‑Specific Behavior  
Identity UoW adapts based on environment settings:

- verbose logging in development  
- stricter error semantics in production  
- optional diagnostic metadata in non‑production  

Environment detection is provided by `IEnvironment`.

---

## How the Unit of Work Connects to the Broader Platform

Identity UoW collaborates with:

- **Frank.Identity.Application**  
  - authentication services  
  - session services  
  - lockout services  
  - rate‑limiting evaluators  
  - token/claim services  

- **Frank.Identity.Domain**  
  - minimal domain involvement (only where legacy identity remains)  

- **Frank.Core.Infrastructure**  
  - observations (correlation IDs, causation chains)  
  - environment detection  
  - exception handling  
  - configuration binding  

- **Frank.Core.Api**  
  - middleware that consumes session validation results  

Identity UoW is the execution boundary for identity flows.

---

## Runtime Collaboration Points

Identity UoW interacts with the runtime by:

- coordinating multi‑step identity operations  
- ensuring lockout and session updates occur in the correct order  
- shaping identity error semantics  
- emitting structured audit logs  
- attaching correlation metadata  
- supporting environment‑specific behavior  

It ensures identity flows are predictable, consistent, and observable.

---

## Composition Flow (Identity Flow → Unit of Work → Application Services)

```
Identity Endpoint (API)
    ↓
Identity Unit of Work
        - Validate provider response
        - Exchange authorization code
        - Validate tokens
        - Evaluate lockout
        - Issue session token
        - Write audit logs
    ↓
Identity Application Services
    ↓
Identity Domain (minimal involvement)
```

The Unit of Work ensures identity operations execute as a coherent whole.

---

## What Belongs in This Document

- identity UoW responsibilities  
- how UoW coordinates identity operations  
- how UoW integrates with identity services  
- how UoW collaborates with infrastructure  
- how UoW fits into the vertical slice lifecycle  

This document does **not** include:

- domain UoW patterns  
- transactional persistence logic  
- aggregate mutation rules  
- business‑rule coordination  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity Unit of Work implementation.  
Whenever identity flows, session behavior, or provider integration evolves, update this section to reflect the current platform architecture.
