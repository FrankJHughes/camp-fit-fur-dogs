# Frank.Identity.Api — Endpoints

The Identity API exposes a small, well‑defined set of **pure identity endpoints** responsible for authentication, session lifecycle, lockout behavior, and identity‑level protections. These endpoints do not contain domain logic and do not mutate aggregates. They operate strictly within the identity subsystem and serve as the entry point for all authenticated owner interactions.

This document describes the Identity API endpoints under:

```
docs/03-frank-identity/api
```

and maps them back to their implementation in:

```
src/Frank/Identity
```

---

## Purpose

Identity endpoints exist to:

- authenticate owners using OIDC  
- issue, validate, and revoke session tokens  
- enforce lockout and rate‑limiting protections  
- provide secure, minimal identity‑only HTTP surfaces  
- support downstream vertical slices that require authenticated owners  

Endpoints are intentionally narrow, predictable, and domain‑free.

---

## Endpoint Categories

### [Authentication Endpoints](ca://s?q=Explain_identity_authentication_endpoints)
Authentication endpoints initiate and complete OIDC login flows.

Typical endpoints include:

- **`GET /identity/login`** — begins OIDC login  
- **`GET /identity/callback`** — handles identity‑provider callback  
- **`POST /identity/logout`** — revokes the active session  

Responsibilities:

- redirect to identity provider  
- validate provider response  
- issue session tokens  
- return correct HTTP semantics for failed login attempts  

These endpoints do not perform domain validation or business logic.

---

### [Session Endpoints](ca://s?q=Describe_identity_session_endpoints)
Session endpoints manage the lifecycle of authenticated sessions.

Common endpoints:

- **`GET /identity/session`** — returns session status  
- **`DELETE /identity/session`** — revokes the current session  

Responsibilities:

- validate session tokens  
- reject expired or invalid sessions  
- provide session metadata for clients  
- integrate with session middleware (US‑111)  

Session endpoints are pure identity operations.

---

### [Lockout Endpoints](ca://s?q=Explain_identity_lockout_endpoints)
Lockout endpoints enforce account lockout rules (US‑133).

Examples:

- **`GET /identity/lockout`** — returns lockout status  
- **`POST /identity/lockout/reset`** — resets lockout after successful login  

Responsibilities:

- prevent brute‑force login attempts  
- avoid account enumeration  
- return blame‑free lockout messages  
- log lockout events for observability  

Lockout endpoints do not expose internal counters or sensitive metadata.

---

### [Rate‑Limited Endpoints](ca://s?q=Explain_rate_limited_identity_endpoints)
Certain identity endpoints are subject to stricter rate limits (US‑132):

- login  
- callback  
- session validation  
- lockout checks  

Responsibilities:

- apply rate‑limit thresholds  
- return `429 Too Many Requests` with `Retry-After`  
- protect against credential‑stuffing attacks  

Rate limiting is configured via infrastructure middleware.

---

### [Identity Health & Debug Endpoints](ca://s?q=Describe_identity_debug_endpoints)
In non‑production environments, identity may expose limited diagnostic endpoints:

- **`GET /identity/debug/session`** — inspect session payload  
- **`GET /identity/debug/claims`** — inspect identity claims  

Responsibilities:

- assist development and troubleshooting  
- remain disabled in production (US‑184 environment rules)  

These endpoints never expose secrets or raw identity‑provider tokens.

---

## How Endpoints Connect to the Broader Platform

Identity endpoints collaborate with:

- **Frank.Identity.Application**  
  - OIDC handlers  
  - session token services  
  - lockout services  

- **Frank.Core.Infrastructure**  
  - observations (correlation IDs, tracing)  
  - exception handling  
  - environment detection  
  - rate‑limiting middleware  

- **Frank.Core.Api**  
  - routing  
  - authentication/authorization middleware  

Identity endpoints are the gateway into authenticated owner flows.

---

## Runtime Collaboration Points

Identity endpoints interact with the runtime by:

- initiating authentication flows  
- validating session tokens  
- enforcing lockout and rate limits  
- returning structured identity‑level responses  
- logging identity events with correlation metadata  

They ensure identity behavior is secure, predictable, and isolated.

---

## Composition Flow (Endpoint → Identity Services → Session → Application)

```
Identity Endpoint
    ↓
Identity Application Services
    ↓
Session Token Issuance / Validation
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

Identity endpoints never bypass session or authorization middleware.

---

## What Belongs in This Document

- identity endpoint responsibilities  
- endpoint categories and behaviors  
- how endpoints integrate with identity services  
- how endpoints fit into the vertical slice lifecycle  
- environment‑specific endpoint behavior  

This document does **not** include:

- domain endpoints  
- customer onboarding flows  
- business‑rule authorization  
- persistence logic  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity endpoint implementation.  
Whenever identity flows, OIDC integration, or session behavior evolves, update this section to reflect the current platform architecture.
