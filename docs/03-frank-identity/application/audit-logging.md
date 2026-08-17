# Frank.Identity.Application — Audit Logging

The Audit Logging subsystem records significant identity‑related events across authentication, session management, lockout enforcement, and identity‑provider interactions. Audit logs provide a durable, structured history of identity activity, enabling security reviews, incident response, compliance reporting, and operational debugging. Audit logging is **pure identity logic** and contains no domain‑specific behavior.

This document describes the Audit Logging subsystem under:

```
docs/03-frank-identity/application
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

Audit logging exists to:

- record identity‑critical events (login, logout, lockout, session issuance)  
- provide traceability for authentication flows  
- support security monitoring and incident response  
- ensure compliance with identity‑related operational requirements  
- maintain a structured, environment‑aware record of identity activity  

Audit logs are consumed by observability systems, security tooling, and operational dashboards.

---

## Responsibilities of the Subsystem

### Authentication Event Logging  
Identity logs all authentication‑related events.  
See: [Authentication Logging](ca://s?q=Explain_identity_authentication_logging)

Events include:

- OIDC login initiated  
- OIDC callback received  
- authentication success  
- authentication failure  
- identity‑provider error conditions  

Each event includes correlation IDs, provider metadata, and environment context.

---

### Session Lifecycle Logging  
Session services emit audit logs for all session‑related operations.  
See: [Session Logging](ca://s?q=Describe_identity_session_logging)

Events include:

- session token issued  
- session token validated  
- session token expired  
- session revoked  
- session rejected due to tampering or invalid signature  

Session logs support debugging and security analysis.

---

### Lockout Logging  
Lockout services record lockout‑related events (US‑133).  
See: [Lockout Logging](ca://s?q=Explain_identity_lockout_logging)

Events include:

- failed login attempt  
- lockout threshold reached  
- lockout activated  
- lockout reset after successful login  

Lockout logs are essential for detecting brute‑force attacks.

---

### Rate‑Limiting Logging  
Identity logs rate‑limit decisions for sensitive endpoints (US‑132).  
See: [Rate‑Limit Logging](ca://s?q=Explain_identity_rate_limit_logging)

Events include:

- rate‑limit threshold exceeded  
- request throttled  
- retry‑after metadata emitted  
- environment‑specific rate‑limit tuning applied  

Rate‑limit logs help identify abusive traffic patterns.

---

### Identity‑Provider Interaction Logging  
Audit logs capture interactions with external identity providers.  
See: [Provider Logging](ca://s?q=Describe_identity_provider_logging)

Events include:

- provider request initiated  
- provider response received  
- provider error or malformed token  
- claim extraction and mapping  

These logs support troubleshooting identity‑provider issues.

---

## How Audit Logging Connects to the Broader Platform

Audit logging collaborates with:

- **Frank.Identity.Application**  
  - authentication services  
  - session services  
  - lockout services  
  - rate‑limiting evaluators  

- **Frank.Core.Infrastructure**  
  - observation context (correlation IDs, causation chains)  
  - environment detection  
  - structured logging pipeline  

- **Frank.Core.Api**  
  - middleware that enriches audit logs with request metadata  

Audit logs flow into the platform’s unified observability system.

---

## Runtime Collaboration Points

Audit logging interacts with the runtime by:

- emitting structured identity events  
- attaching correlation and causation metadata  
- recording environment‑specific identity behavior  
- supporting security monitoring and anomaly detection  
- enabling post‑incident reconstruction of identity flows  

Audit logs are written at key identity boundaries to ensure full traceability.

---

## Composition Flow (Identity Event → Audit Log → Observability)

```
Identity Application Service
    ↓
Audit Logging (structured event)
    ↓
Observation Pipeline
    ↓
Logging Sink (Seq, ELK, Render logs, etc.)
```

Audit logging ensures identity flows are visible, traceable, and secure.

---

## What Belongs in This Document

- audit logging responsibilities  
- identity event categories  
- how audit logs integrate with identity services  
- how audit logs collaborate with infrastructure  
- how audit logging fits into the vertical slice lifecycle  

This document does **not** include:

- domain audit logs  
- business‑rule audit events  
- persistence‑level audit trails  
- customer onboarding audit behavior  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity audit‑logging implementation.  
Whenever identity flows, session behavior, or provider integration evolves, update this section to reflect the current platform architecture.
