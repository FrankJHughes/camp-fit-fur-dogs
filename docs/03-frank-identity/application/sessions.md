# Frank.Identity.Application — Sessions

The Sessions subsystem manages the full lifecycle of authenticated identity sessions within the Frank platform. It is responsible for issuing, validating, refreshing, and revoking session tokens, and for ensuring that identity flows remain secure, predictable, and environment‑aware. Session logic is **pure identity application logic** — it contains no HTTP routing, no domain rules, and no business logic.

This document describes the Sessions subsystem under:

```
docs/03-frank-identity/application
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

The Sessions subsystem exists to:

- issue secure session tokens after successful authentication  
- validate session tokens on every protected request  
- enforce expiration and sliding‑expiration rules  
- revoke sessions on logout or security events  
- integrate with lockout and rate‑limiting services  
- provide identity‑only session metadata to middleware and API layers  

Sessions are the mechanism by which authenticated owners remain recognized across requests.

---

## Responsibilities of the Subsystem

### Session Issuance  
Session issuance occurs after successful OIDC authentication.  
See: [Session Services](ca://s?q=Describe_identity_session_services)

Responsibilities:

- generate secure session tokens  
- embed identity claims and metadata  
- apply configured expiration rules  
- attach environment‑specific flags (e.g., secure cookies in production)  
- emit audit logs for session creation  

Session issuance is triggered by the OIDC callback flow.

---

### Session Validation  
Session validation ensures that only authenticated owners reach protected endpoints.  
See: [Session Validation](ca://s?q=Explain_identity_session_validation_middleware)

Responsibilities:

- validate token signature and issuer  
- check expiration and sliding‑expiration windows  
- verify session has not been revoked  
- extract identity claims for downstream authorization  
- return structured identity errors on failure  

Validation is performed by identity middleware before application handlers execute.

---

### Session Revocation  
Session revocation terminates an active session.  
See: [Session Revocation](ca://s?q=Explain_identity_session_revocation)

Responsibilities:

- invalidate session tokens  
- remove or mark session state as revoked  
- support logout flows  
- revoke sessions after lockout or security anomalies  
- emit audit logs for revocation events  

Revocation ensures compromised or stale sessions cannot be reused.

---

### Sliding Expiration  
Sliding expiration extends session lifetime during active use.

Responsibilities:

- refresh expiration timestamp when session is actively used  
- prevent indefinite session extension by applying maximum lifetime rules  
- integrate with environment‑specific expiration policies  

Sliding expiration improves usability while maintaining security.

---

### Session Metadata  
Session services provide identity metadata to middleware and API layers.

Metadata includes:

- owner identifier  
- identity‑provider subject  
- issued‑at timestamp  
- expiration timestamp  
- environment flags  
- lockout state (if applicable)  

Metadata is used for authorization, logging, and observability.

---

## How Sessions Connect to the Broader Platform

Sessions collaborate with:

- **Frank.Identity.Api**  
  - endpoints for session status and logout  
  - middleware that consumes session validation results  

- **Frank.Identity.Application**  
  - authentication services (session issuance)  
  - lockout services (session reset after successful login)  
  - rate‑limiting services (session‑aware throttling)  

- **Frank.Core.Infrastructure**  
  - environment detection  
  - observations (correlation IDs, causation chains)  
  - exception handling  
  - configuration binding  

- **Frank.Core.Api**  
  - authorization middleware  
  - request pipeline integration  

Sessions are the identity backbone for all authenticated flows.

---

## Runtime Collaboration Points

Sessions interact with the runtime by:

- validating identity state before handlers execute  
- enforcing expiration and revocation rules  
- shaping identity error semantics  
- attaching identity metadata to the request context  
- emitting audit logs for session lifecycle events  
- supporting environment‑specific behavior (e.g., secure cookies in production)  

Sessions ensure authentication remains secure, predictable, and observable.

---

## Composition Flow (Authentication → Session Issuance → Validation → Application)

```
OIDC Callback (Authentication)
    ↓
Session Issued
    ↓
Identity Middleware
        - Validate session
        - Apply sliding expiration
        - Enforce revocation
        - Attach identity metadata
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

Sessions are the mechanism that carries identity forward across requests.

---

## What Belongs in This Document

- session issuance behavior  
- session validation rules  
- session revocation behavior  
- sliding‑expiration logic  
- session metadata structure  
- audit logging for session events  
- how session services integrate with identity middleware  

This document does **not** include:

- HTTP endpoint routing  
- domain authorization rules  
- persistence logic  
- identity‑provider configuration  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity session implementation.  
Whenever session behavior, token validation rules, or expiration policies evolve, update this section to reflect the current platform architecture.
