# Frank.Identity.Domain — Sessions

The **Sessions** subsystem in the Identity Domain defines the immutable, invariant‑enforced structures that represent identity session state inside the Frank platform. Unlike the Identity Application layer—which issues, validates, refreshes, and revokes sessions—the domain layer provides the **pure session primitives** that application services rely on. These models contain **no protocol logic**, **no middleware behavior**, and **no persistence rules**. They exist solely to ensure that identity session state is always valid, consistent, and safe.

This document describes the Sessions subsystem under:

```
docs/03-frank-identity/domain
```

and maps it back to its implementation in:

```
src/Frank/Identity/Domain
```

---

## Purpose

The Sessions subsystem exists to:

- define the structure of identity session state  
- enforce session invariants at construction time  
- represent issued‑at, expiration, and revocation metadata  
- provide stable primitives for session validation and authorization  
- ensure identity session behavior remains pure and domain‑free  

Domain session models are the foundation for all identity session flows.

---

## Responsibilities of the Subsystem

### Session State Model  
The domain defines the immutable representation of a session.  
See: [Session State](ca://s?q=Explain_identity_session_state)

Responsibilities:

- represent issued‑at timestamp  
- represent expiration timestamp  
- represent maximum lifetime (if configured)  
- represent revocation markers  
- represent identity metadata (IdentityId, ProviderSubject, etc.)  
- enforce invariants (expiration > issued‑at, valid identity metadata)  

Session state is consumed by application‑level session services and middleware.

---

### Session Value Objects  
Session state is composed of smaller identity‑specific value objects.

Examples:

- **SessionId** — unique session identifier  
- **IssuedAt** — validated timestamp  
- **ExpiresAt** — validated expiration timestamp  
- **RevocationState** — immutable representation of revocation  
- **SessionClaims** — normalized identity claims embedded in the session  

Responsibilities:

- enforce invariants at construction  
- prevent invalid session metadata  
- provide safe, immutable primitives for session logic  

These value objects ensure session state is always valid.

---

### Session Invariants  
The domain enforces strict invariants to prevent invalid session state.

Examples:

- expiration must be after issued‑at  
- maximum lifetime must not be exceeded  
- revocation markers must be consistent  
- identity metadata must be present and valid  
- timestamps must be non‑negative and monotonic  

If invariants are violated, domain exceptions are thrown.

---

### Domain Exceptions  
Session‑related domain exceptions ensure predictable failure semantics.  
See: [Domain Exceptions](ca://s?q=Explain_identity_domain_exceptions)

Examples:

- **SessionExpiredException**  
- **SessionInvalidException**  
- **SessionRevokedException**  
- **SessionStateInvalidException**  

These exceptions are consumed by application services and middleware.

---

## How Sessions Connect to the Broader Platform

Identity Domain Sessions collaborate with:

- **Frank.Identity.Application**  
  - session issuance constructs domain session models  
  - session validation reads domain session state  
  - lockout services may reset session state after successful login  

- **Frank.Identity.Api**  
  - API endpoints never construct domain session models directly  
  - API consumes application‑level session results  

- **Frank.Core.Infrastructure**  
  - environment detection influences application behavior, not domain models  
  - observations attach metadata to session domain objects  

- **Frank.Core.Api**  
  - authorization middleware consumes session domain models  

The domain layer provides the stable session primitives used across identity flows.

---

## Runtime Collaboration Points

Domain session models interact with the runtime by:

- enforcing session invariants  
- shaping identity error semantics  
- supporting session issuance and validation  
- supporting revocation flows  
- supporting sliding expiration logic  
- providing immutable identity metadata  

Domain session models ensure identity sessions remain correct and predictable.

---

## Composition Flow (Authentication → Domain Session → Application → API)

```
OIDC Callback (Application)
    ↓
Domain Session Created
        - IssuedAt
        - ExpiresAt
        - IdentityId
        - ProviderSubject
        - Claims
        - RevocationState
    ↓
Session Validation (Application)
    ↓
Identity Middleware
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

The domain layer provides the identity session primitives used throughout the subsystem.

---

## What Belongs in This Document

- session domain model responsibilities  
- session value object definitions  
- session invariants  
- session domain exceptions  
- how domain session models integrate with application services  
- how domain session models support identity flows  

This document does **not** include:

- session issuance logic  
- session validation logic  
- lockout evaluation  
- HTTP endpoints  
- middleware behavior  
- persistence logic  

Those belong in the application or API layers.

---

## Notes

Keep this document grounded in the actual Frank.Identity domain session implementation.  
Whenever session metadata, expiration rules, or identity invariants evolve, update this section to reflect the current platform architecture.
