# Frank.Identity.Api — Middleware

Identity middleware provides the cross‑cutting runtime behaviors that wrap every identity‑related request. These components enforce session validation, apply rate‑limiting, attach correlation metadata, and ensure identity endpoints remain pure and domain‑free. Middleware sits between the transport layer and the identity application layer, shaping how identity requests enter and exit the system.

This document describes the Identity middleware under:

```
docs/03-frank-identity/api
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

Identity middleware exists to:

- validate session tokens before requests reach application handlers  
- enforce lockout and rate‑limiting protections  
- attach identity‑specific observation metadata  
- ensure identity endpoints behave consistently across environments  
- centralize identity‑level concerns outside domain and application logic  

Middleware is the first identity component to execute on every request.

---

## Responsibilities of the Subsystem

### [Session Validation Middleware](ca://s?q=Explain_identity_session_validation_middleware)
This middleware validates the caller’s session token (US‑111):

- rejects expired or invalid tokens  
- rejects revoked sessions  
- attaches session metadata to the request context  
- returns `401 Unauthorized` for invalid identity state  
- ensures only authenticated owners reach protected endpoints  

Session validation is **pure identity logic** — no domain access.

---

### [Lockout Enforcement Middleware](ca://s?q=Describe_identity_lockout_middleware)
Lockout middleware enforces account lockout rules (US‑133):

- checks lockout state before login attempts  
- prevents brute‑force login attempts  
- avoids account enumeration  
- returns blame‑free lockout responses  
- logs lockout events for observability  

Lockout enforcement runs early in the pipeline to prevent repeated failures.

---

### [Rate‑Limiting Middleware](ca://s?q=Explain_identity_rate_limiting_middleware)
Identity endpoints are subject to stricter rate limits (US‑132):

- login  
- callback  
- session validation  
- lockout checks  

Rate‑limiting middleware:

- applies configured thresholds  
- returns `429 Too Many Requests` with `Retry-After`  
- protects against credential‑stuffing attacks  
- integrates with environment detection for tuning  

Rate limiting is identity‑specific and does not rely on domain rules.

---

### [Observation & Correlation Middleware](ca://s?q=Explain_identity_observation_middleware)
Identity middleware integrates with the platform’s observation system:

- attaches correlation IDs  
- records identity‑specific traces  
- logs authentication and session events  
- enriches logs with identity metadata (session ID, owner ID, provider ID)  

This ensures identity flows are fully observable across the platform.

---

### [Environment‑Aware Identity Middleware](ca://s?q=Describe_environment_aware_identity_middleware)
Identity middleware adapts based on environment:

- debug endpoints enabled only in development  
- verbose logging in non‑production  
- stricter error semantics in production  
- optional diagnostic claims inspection in development  

Environment detection is provided by `IEnvironment`.

---

## How Middleware Connects to the Broader Platform

Identity middleware collaborates with:

- **Frank.Identity.Application**  
  - session services  
  - lockout services  
  - OIDC handlers  

- **Frank.Core.Infrastructure**  
  - observations  
  - exception handling  
  - environment detection  
  - rate‑limiting infrastructure  

- **Frank.Core.Api**  
  - routing  
  - endpoint filters  
  - authorization middleware  

Middleware is the bridge between transport and identity logic.

---

## Runtime Collaboration Points

Identity middleware interacts with the runtime by:

- validating identity state before handlers execute  
- enforcing lockout and rate limits  
- attaching correlation metadata  
- logging identity events  
- shaping identity error responses  
- ensuring identity purity rules are upheld  

Middleware ensures identity behavior is secure, predictable, and consistent.

---

## Composition Flow (Request → Middleware → Identity Services → Application)

```
Incoming Request
    ↓
Identity Middleware
        - Rate limiting
        - Lockout enforcement
        - Session validation
        - Observation enrichment
    ↓
Identity Application Services
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

Identity middleware is the first line of defense for all identity flows.

---

## What Belongs in This Document

- identity middleware responsibilities  
- how middleware integrates with identity services  
- how middleware collaborates with infrastructure  
- how middleware fits into the vertical slice lifecycle  
- environment‑specific middleware behavior  

This document does **not** include:

- domain middleware  
- business‑rule authorization  
- persistence logic  
- customer onboarding flows  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity middleware implementation.  
Whenever identity flows, session validation rules, or rate‑limiting behavior evolve, update this section to reflect the current platform architecture.
