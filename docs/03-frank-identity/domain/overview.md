# Frank.Identity.Domain — Overview

The **Identity Domain** defines the foundational identity concepts used throughout the Frank Identity subsystem. It provides the pure, invariant‑enforced models that authentication, session management, lockout evaluation, and authorization depend on. Unlike the Identity Application layer—which executes identity flows—the domain layer defines **what identity *is*** inside the platform.

This document describes the Identity Domain subsystem under:

```
docs/03-frank-identity/domain
```

and maps it back to its implementation in:

```
src/Frank/Identity/Domain
```

---

## Purpose

The Identity Domain exists to:

- define identity entities and value objects  
- enforce identity invariants at construction time  
- represent identity subjects independently of providers  
- provide stable identity primitives for application services  
- ensure identity logic remains pure and domain‑free  

The domain layer is the stable core of the identity subsystem.

---

## Responsibilities of the Subsystem

### [Identity User Model](ca://s?q=Explain_identity_user_model)
Defines the authenticated identity subject.

Responsibilities:

- represent provider subject (`sub`) and issuer (`iss`)  
- represent mapped platform identity identifier  
- enforce invariants (non‑empty subject, valid issuer, valid identity ID)  
- provide identity metadata for sessions and authorization  

Identity users are **not** domain owners or customer records.

---

### [Identity Value Objects](ca://s?q=Explain_identity_value_objects)
Immutable primitives used throughout identity flows.

Examples:

- ProviderSubject  
- ProviderIssuer  
- IdentityId  
- IdentityClaims  

Responsibilities:

- enforce invariants at construction  
- prevent invalid identity state  
- provide safe, immutable identity primitives  

These objects form the building blocks of identity logic.

---

### [Lockout State](ca://s?q=Explain_identity_lockout_state)
Represents identity‑level lockout information (US‑133).

Responsibilities:

- track failed login attempts  
- track lockout timestamps  
- enforce lockout invariants (non‑negative counters, valid timestamps)  

Lockout state is identity state, not domain state.

---

### [Session State](ca://s?q=Explain_identity_session_state)
Represents identity session metadata.

Responsibilities:

- issued‑at and expiration timestamps  
- revocation markers  
- identity metadata embedded in sessions  
- session invariants (valid expiration windows, non‑tampered state)  

Session state is consumed by session services and middleware.

---

### [Domain Exceptions](ca://s?q=Explain_identity_domain_exceptions)
Identity‑specific error types used to enforce invariants.

Responsibilities:

- represent validation failures  
- represent token/claim errors  
- represent lockout violations  
- represent session failures  
- ensure identity flows fail safely and predictably  

Domain exceptions shape identity error semantics across layers.

---

## How the Identity Domain Connects to the Broader Platform

Identity Domain collaborates with:

- **Frank.Identity.Application**  
  - constructs and validates domain models  
  - uses domain primitives for authentication, sessions, lockout, and authorization  

- **Frank.Identity.Api**  
  - API endpoints consume application‑level identity results  
  - API never constructs domain models directly  

- **Frank.Core.Infrastructure**  
  - environment detection influences application behavior, not domain models  
  - observations attach metadata to identity domain objects  

- **Frank.Core.Api**  
  - authorization middleware consumes identity domain models  

The domain layer is the foundation for all identity flows.

---

## Runtime Collaboration Points

Identity Domain interacts with the runtime by:

- enforcing identity invariants  
- shaping identity error semantics  
- supporting session issuance and validation  
- supporting lockout evaluation  
- supporting provider claim mapping  
- providing immutable identity primitives  

Domain models ensure identity flows remain correct and predictable.

---

## Composition Flow (Provider → Claims → Domain Models → Application → API)

```
Identity Provider
    ↓
Claims Extracted (Application)
    ↓
Identity Domain Models Created
        - IdentityUser
        - ProviderSubject
        - ProviderIssuer
        - IdentityId
        - IdentityClaims
        - LockoutState
        - SessionState
    ↓
Identity Application Services
    ↓
Identity API Endpoints
```

The domain layer provides the identity primitives used throughout the subsystem.

---

## What Belongs in This Document

- identity domain model responsibilities  
- identity value object definitions  
- lockout and session state structures  
- identity domain exceptions  
- how domain models integrate with application services  
- how domain models support identity flows  

This document does **not** include:

- OIDC protocol logic  
- session issuance logic  
- lockout evaluation logic  
- HTTP endpoints  
- middleware behavior  
- domain‑business rules  

Those belong in the application or API layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity domain implementation.  
Whenever identity invariants, claim mapping rules, or session metadata evolve, update this section to reflect the current platform architecture.
