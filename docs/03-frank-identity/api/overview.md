# Frank.Identity.Api — Overview

The Identity API provides authentication, authorization, and session‑management capabilities for the Frank platform. It exposes pure identity endpoints, middleware, and supporting services that authenticate owners, validate sessions, enforce lockout and rate‑limiting protections, and integrate with OIDC identity providers. This layer contains **no domain logic** and serves as the secure entry point for all authenticated owner interactions.

This document describes the Identity API under:

```
docs/03-frank-identity/api
```

and maps it back to its implementation in:

```
src/Frank/Identity
```

---

## Purpose

The Identity API exists to:

- authenticate owners using OIDC  
- issue, validate, and revoke session tokens  
- enforce account lockout and rate‑limiting protections  
- provide secure, minimal identity‑only HTTP surfaces  
- support downstream vertical slices that require authenticated owners  
- maintain strict separation between identity concerns and domain logic  

Identity is a **pure infrastructure subsystem**, not a domain subsystem.

---

## Responsibilities of the Subsystem

### Authentication
Identity handles OIDC login flows, callback processing, and session issuance.  
See: [Authentication](ca://s?q=Explain_identity_authentication_endpoints)

### Session Management
Identity validates session tokens, rejects invalid sessions, and provides logout and session‑status endpoints.  
See: [Session Management](ca://s?q=Describe_identity_session_endpoints)

### Authorization
Identity enforces identity‑based access control using claims and session state.  
See: [Authorization](ca://s?q=Explain_identity_authorization)

### Lockout Enforcement
Identity prevents brute‑force login attempts and manages lockout state.  
See: [Lockout](ca://s?q=Explain_identity_lockout_behavior)

### Rate Limiting
Identity applies stricter rate limits to authentication‑related endpoints.  
See: [Rate Limiting](ca://s?q=Explain_rate_limited_identity_endpoints)

### Middleware
Identity middleware validates sessions, enforces lockout, applies rate limits, and enriches observation context.  
See: [Middleware](ca://s?q=Explain_identity_middleware)

### Endpoint Surface
Identity exposes a minimal, predictable set of endpoints for login, logout, session status, lockout, and identity debugging (non‑production only).  
See: [Endpoints](ca://s?q=Explain_identity_endpoints)

---

## How the Identity API Connects to the Broader Platform

Identity collaborates with:

- **Frank.Identity.Application**  
  - OIDC handlers  
  - session token services  
  - lockout services  
  - identity provider integrations  

- **Frank.Core.Infrastructure**  
  - observations (correlation IDs, tracing)  
  - exception handling  
  - environment detection  
  - rate‑limiting infrastructure  

- **Frank.Core.Api**  
  - routing  
  - authentication/authorization middleware  

Identity is the authentication gateway for all owner‑facing vertical slices.

---

## Runtime Collaboration Points

Identity interacts with the runtime by:

- validating identity tokens on every request  
- enforcing lockout and rate limits  
- attaching correlation metadata  
- logging identity events  
- shaping identity error responses  
- ensuring identity purity rules are upheld  

Identity middleware is the first line of defense for all identity flows.

---

## Composition Flow (Authentication → Session → Authorization → Application)

```
Owner Login (OIDC)
    ↓
Identity Provider Callback
    ↓
Session Token Issued
    ↓
Identity Middleware
        - Rate limiting
        - Lockout enforcement
        - Session validation
        - Observation enrichment
    ↓
Authorization Middleware
    ↓
Application Handler (if authorized)
```

Identity ensures authentication is secure, predictable, and isolated.

---

## What Belongs in This Folder

- identity endpoint documentation  
- authentication and OIDC flow documentation  
- session token issuance & validation  
- lockout & rate‑limiting behavior  
- identity purity rules  
- request/response contracts for identity endpoints  
- identity middleware behavior  

This folder does **not** include:

- domain endpoints  
- customer onboarding flows  
- business‑rule authorization  
- persistence logic  

Those belong in other vertical slices.

---

## Notes

Keep this document grounded in the actual Frank.Identity implementation.  
Whenever identity flows, OIDC integration, or session behavior evolves, update this section to reflect the current platform architecture.
