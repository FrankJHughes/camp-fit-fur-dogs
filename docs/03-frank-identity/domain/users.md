# Frank.Identity.Domain — Users

The **Users** subsystem in the Identity Domain defines the pure, invariant‑enforced representation of identity subjects inside the Frank platform. Unlike the Identity Application layer—which executes authentication, claim mapping, session issuance, and lockout evaluation—the domain layer provides the **immutable identity primitives** that all identity flows rely on. These models contain **no OIDC protocol logic**, **no session behavior**, **no middleware concerns**, and **no customer‑domain fields**. They exist solely to ensure identity subjects are always valid, consistent, and safe.

This document describes the Users subsystem under:

```
docs/03-frank-identity/domain
```

and maps it back to its implementation in:

```
src/Frank/Identity/Domain
```

---

## Purpose

The Users subsystem exists to:

- define the identity‑level representation of authenticated subjects  
- enforce invariants around provider identity and platform identity  
- provide stable primitives for authentication, sessions, and authorization  
- ensure identity user data remains pure and domain‑free  
- support identity flows without leaking customer or business concepts  

Identity Domain Users are the foundation for all identity operations.

---

## Responsibilities of the Subsystem

### Identity User Model  
The domain defines the immutable identity user representation.  
See: [Identity User Model](ca://s?q=Explain_identity_user_model)

Responsibilities:

- represent provider subject (`sub`)  
- represent provider issuer (`iss`)  
- represent mapped platform identity identifier (`IdentityId`)  
- store normalized identity claims  
- enforce invariants (non‑empty subject, valid issuer, valid identity ID)  

Identity users are **not** domain owners, staff, or admins — they are authentication subjects.

---

### Identity Value Objects  
Identity users are composed of smaller, validated primitives.  
See: [Identity Value Objects](ca://s?q=Explain_identity_value_objects)

Examples:

- **ProviderSubject** — validated provider subject identifier  
- **ProviderIssuer** — validated issuer URL  
- **IdentityId** — platform identity identifier  
- **IdentityClaims** — normalized claim set  

Responsibilities:

- enforce invariants at construction  
- prevent invalid identity metadata  
- provide safe, immutable identity primitives  

These value objects ensure identity users are always valid.

---

### Identity Claims  
Claims represent the normalized identity attributes extracted from tokens.

Responsibilities:

- store required identity attributes (email, subject, issuer, etc.)  
- normalize provider‑specific claim formats  
- enforce presence of mandatory claims  
- provide identity metadata for sessions and authorization  

Claims are identity‑level metadata, not domain‑level profile data.

---

### Domain Exceptions  
Identity user construction may throw domain exceptions.  
See: [Domain Exceptions](ca://s?q=Explain_identity_domain_exceptions)

Examples:

- **InvalidProviderSubjectException**  
- **InvalidIssuerException**  
- **InvalidIdentityIdException**  
- **InvalidClaimSetException**  

These exceptions ensure identity user state is always correct.

---

## How Users Connect to the Broader Platform

Identity Domain Users collaborate with:

- **Frank.Identity.Application**  
  - authentication services construct identity users from claims  
  - session services embed identity users into session state  
  - lockout services evaluate identity users during login flows  

- **Frank.Identity.Api**  
  - API endpoints never construct domain identity users directly  
  - API consumes application‑level identity results  

- **Frank.Core.Infrastructure**  
  - environment detection influences application behavior, not domain models  
  - observations attach metadata to identity domain objects  

- **Frank.Core.Api**  
  - authorization middleware consumes identity domain users  

Identity users are the bridge between identity providers and the platform.

---

## Runtime Collaboration Points

Identity Domain Users interact with the runtime by:

- enforcing identity invariants  
- shaping identity error semantics  
- supporting authentication flows  
- supporting session issuance and validation  
- supporting lockout evaluation  
- providing identity metadata for authorization  
- ensuring identity subjects remain immutable and predictable  

Identity users ensure identity flows remain secure and consistent.

---

## Composition Flow (Provider → Claims → Domain User → Application → API)

```
Identity Provider
    ↓
Claims Extracted (Application)
    ↓
Identity Domain User Created
        - ProviderSubject
        - ProviderIssuer
        - IdentityId
        - IdentityClaims
    ↓
Session Issuance (Application)
    ↓
Identity Middleware
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

Identity users are the identity payload that flows through the platform.

---

## What Belongs in This Document

- identity user domain model responsibilities  
- identity value object definitions  
- identity claim structures  
- identity domain exceptions  
- how domain identity users integrate with application services  
- how domain identity users support identity flows  

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

Keep this document grounded in the actual Frank.Identity domain user implementation.  
Whenever identity‑provider behavior, claim mapping rules, or identity invariants evolve, update this section to reflect the current platform architecture.
