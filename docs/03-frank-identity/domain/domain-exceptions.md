# Frank.Identity.Domain — Domain Exceptions

Domain Exceptions define the identity‑specific error types used throughout the Frank Identity subsystem. They represent **pure identity failures** — not HTTP errors, not infrastructure faults, and not domain‑business rule violations. These exceptions enforce identity invariants, ensure predictable error semantics, and provide structured failure information to the Identity Application layer.

This document describes the Domain Exceptions subsystem under:

```
docs/03-frank-identity/domain
```

and maps it back to its implementation in:

```
src/Frank/Identity/Domain
```

---

## Purpose

Domain Exceptions exist to:

- represent identity‑specific validation failures  
- enforce invariants in identity value objects and entities  
- provide structured error types for application services  
- prevent invalid identity state from entering authentication flows  
- ensure identity errors remain pure and domain‑free  

Identity Domain Exceptions are the foundation for consistent identity error semantics.

---

## Responsibilities of the Subsystem

### Identity Validation Exceptions  
Identity validation exceptions enforce invariants in identity value objects.  
See: [Identity Value Objects](ca://s?q=Explain_identity_value_objects)

Examples:

- **InvalidProviderSubjectException** — provider subject is missing or malformed  
- **InvalidIssuerException** — issuer URL is invalid  
- **InvalidIdentityIdException** — platform identity identifier is malformed  
- **InvalidClaimSetException** — required claims missing or invalid  

These exceptions prevent invalid identity state from propagating.

---

### Token & Claim Exceptions  
Token exceptions represent failures in identity token validation.  
See: [Token Services](ca://s?q=Explain_identity_token_services)

Examples:

- **TokenSignatureException** — token signature invalid  
- **TokenExpiredException** — token past expiration  
- **TokenIssuerMismatchException** — issuer does not match expected provider  
- **TokenMalformedException** — token structure invalid  

These exceptions are thrown during OIDC callback and session validation flows.

---

### Lockout Exceptions  
Lockout exceptions represent identity‑level lockout violations (US‑133).  
See: [Lockout Services](ca://s?q=Explain_identity_lockout_services)

Examples:

- **AccountLockedException** — account currently locked  
- **LockoutStateInvalidException** — lockout counters or timestamps invalid  

Lockout exceptions ensure lockout state remains consistent and secure.

---

### Session Exceptions  
Session exceptions represent failures in session issuance or validation.  
See: [Sessions](ca://s?q=Describe_identity_session_services)

Examples:

- **SessionExpiredException** — session past expiration  
- **SessionRevokedException** — session explicitly revoked  
- **SessionInvalidException** — session token malformed or unverifiable  

Session exceptions are consumed by identity middleware.

---

### Provider Interaction Exceptions  
Provider exceptions represent failures interacting with the identity provider.

Examples:

- **ProviderCommunicationException** — provider unreachable  
- **ProviderResponseException** — provider returned invalid or unexpected data  
- **ProviderClaimMappingException** — claims cannot be mapped to platform identity  

These exceptions ensure provider failures are captured cleanly.

---

## How Domain Exceptions Connect to the Broader Platform

Domain Exceptions collaborate with:

- **Frank.Identity.Application**  
  - application services catch domain exceptions  
  - exceptions shape identity error semantics  
  - exceptions trigger audit logging  

- **Frank.Identity.Api**  
  - API layer converts domain exceptions into structured HTTP responses  
  - API never throws domain exceptions directly  

- **Frank.Core.Infrastructure**  
  - exception handling middleware logs identity exceptions  
  - environment detection may influence error detail levels  

- **Frank.Core.Api**  
  - authorization middleware consumes session exceptions  

Domain exceptions are the backbone of identity error handling.

---

## Runtime Collaboration Points

Domain Exceptions interact with the runtime by:

- enforcing identity invariants  
- shaping identity error semantics  
- preventing invalid identity state from entering flows  
- supporting audit logging  
- supporting environment‑specific error behavior  
- ensuring identity failures remain predictable and secure  

Exceptions ensure identity flows fail safely and consistently.

---

## Composition Flow (Domain Exception → Application → API)

```
Identity Domain
    ↓
Exception Thrown
    ↓
Identity Application
        - Catch exception
        - Log audit event
        - Convert to identity error result
    ↓
Identity API
        - Convert to HTTP response
```

Domain exceptions ensure identity failures are handled cleanly across layers.

---

## What Belongs in This Document

- identity domain exception categories  
- validation, token, session, lockout, and provider exception types  
- how exceptions integrate with application services  
- how exceptions shape identity error semantics  
- how exceptions support identity invariants  

This document does **not** include:

- HTTP error codes  
- application‑level error mapping  
- infrastructure exceptions  
- domain‑business rule exceptions  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity domain exception implementation.  
Whenever identity invariants, token validation rules, or session behavior evolve, update this section to reflect the current platform architecture.
